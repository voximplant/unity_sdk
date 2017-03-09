//
// Created by Aleksey Zinchenko on 02/03/2017.
//

#include <stdint.h>
#include <cstdlib>
#include <GLES2/gl2.h>
#include <jni.h>
#include <vector>

#include "IUnityGraphics.h"
#include "EGLVideoRenderer.h"
#include "DestroyList.h"

static EGLVideoRenderer **s_renderers;
static Mutex *s_renderersMutex;
static EGLContext s_unityContext;
static EGLSurface s_unitySurface;

static DestroyList<EGLVideoRenderer *> *s_destroyList;

JNIEXPORT static void JNICALL
Java_renderBufferFrame(JNIEnv *env,
                       jobject thiz,
                       jobject yPlane,
                       jint yStride,
                       jobject uPlane,
                       jint uStride,
                       jobject vPlane,
                       jint vStride,
                       jint width,
                       jint height,
                       jint stream,
                       jint degrees);

void invalidateAllRenderers();

static const char* CLIENT_CLASS = "com/voximplant/sdk/AVoImClient";
static JNINativeMethod gMethods[] = {
        {"renderBufferFrame", "(Ljava/nio/ByteBuffer;ILjava/nio/ByteBuffer;ILjava/nio/ByteBuffer;IIIII)V", (void*)Java_renderBufferFrame},
};

void invalidateAllRenderers() {
    for (int i = 0; i < 2; i++) {
        EGLVideoRenderer *renderer = s_renderers[i];
        if (renderer == NULL) {
            continue;
        }

        renderer->Invalidate();
    }
}

static void UNITY_INTERFACE_API OnRenderEvent(int eventID) {
    if (eventID == 41) {
        s_renderersMutex->Acquire();
    } else if (eventID == 42) {
        bool invalidationEvent = false;
        const EGLContext context = eglGetCurrentContext();
        if (s_unityContext != context) {
            s_unityContext = context;
            invalidationEvent = true;
        }
        const EGLSurface surface = eglGetCurrentSurface(EGL_DRAW);
        if (s_unitySurface != surface) {
            s_unitySurface = surface;
            invalidationEvent = true;
        }

        if (invalidationEvent) {
            invalidateAllRenderers();
        }

        s_renderersMutex->Release();
    }
}

JNIEXPORT static void JNICALL
Java_renderBufferFrame(JNIEnv *env,
                       jobject thiz,
                       jobject yPlane,
                       jint yStride,
                       jobject uPlane,
                       jint uStride,
                       jobject vPlane,
                       jint vStride,
                       jint width,
                       jint height,
                       jint stream,
                       jint degrees) {
    LockGuard lock(s_renderersMutex);

    if (s_unityContext == EGL_NO_CONTEXT
        || s_unitySurface == EGL_NO_SURFACE) {
        return;
    }

    bool newRendererCreated = false;
    EGLVideoRenderer *renderer = s_renderers[stream];
    if (renderer != NULL && !renderer->IsValidForSize(width, height)) {
        renderer->Detach();
        if (renderer->GetOGLContext() != EGL_NO_CONTEXT) {
            s_destroyList->AddObject((void *) renderer->GetTargetTextureId(), renderer->GetOGLContext(), renderer);
        } else {
            delete renderer;
        }
        renderer = NULL;
    }
    if (renderer == NULL) {
        s_renderers[stream] = new EGLVideoRenderer(width, height, s_unityContext);
        renderer = s_renderers[stream];

        newRendererCreated = true;
    }

    renderer->RenderBuffer((uint8_t *) env->GetDirectBufferAddress(yPlane),
                           yStride,
                           (uint8_t *) env->GetDirectBufferAddress(uPlane),
                           uStride,
                           (uint8_t *) env->GetDirectBufferAddress(vPlane),
                           vStride,
                           width,
                           height,
                           degrees
    );

    if (newRendererCreated) {
        jclass clazz = env->FindClass(CLIENT_CLASS);
        jmethodID methodID = env->GetMethodID(clazz, "reportNewNativeTexture", "(JJIII)V");
        env->CallVoidMethod(thiz, methodID, (jlong)renderer->GetTargetTextureId(), (jlong)renderer->GetOGLContext(), width, height, stream);
    }
}

jint JNI_OnLoad(JavaVM* vm, void* reserved)
{
    JNIEnv* env;
    if (vm->GetEnv(reinterpret_cast<void**>(&env), JNI_VERSION_1_6) != JNI_OK) {
        return -1;
    }

    jclass cls = env->FindClass(CLIENT_CLASS);
    if (cls == NULL)
    {
        return JNI_ERR;
    }
    jint nRes = env->RegisterNatives(cls, gMethods, sizeof(gMethods)/sizeof(gMethods[0]));
    if (nRes <0)
    {
        return JNI_ERR;
    }

    return JNI_VERSION_1_6;
}

extern "C" UnityRenderingEvent UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
GetRenderEventFunc()
{
	return OnRenderEvent;
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
DestroyRenderer(void *textureId, EGLContext oglContext) {
    s_destroyList->DestroyObject(textureId, oglContext);
};

typedef void	(UNITY_INTERFACE_API *PluginLoadFunc)(IUnityInterfaces* unityInterfaces);
typedef void	(UNITY_INTERFACE_API *PluginUnloadFunc)();

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
InitializeVoximplant() {
    s_renderers = (EGLVideoRenderer **) calloc(2, sizeof(EGLVideoRenderer *));
    s_renderersMutex = new Mutex();
    s_destroyList = new DestroyList<EGLVideoRenderer *>();
    s_unityContext = eglGetCurrentContext();
    s_unitySurface = eglGetCurrentSurface(EGL_DRAW);
};