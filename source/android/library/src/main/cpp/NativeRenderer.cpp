//
// Created by Aleksey Zinchenko on 02/03/2017.
//

#include <stdint.h>
#include <cstdlib>
#include <GLES2/gl2.h>
#include <jni.h>
#include <vector>

#include "IUnityGraphics.h"
#include "VideoRenderer.h"

static VideoRenderer **s_renderers;
static Mutex *s_renderersMutex;
static EGLContext s_unityContext;
static EGLSurface s_unitySurface;

static Mutex *s_destroyListMutex;
static std::vector<VideoRenderer *> *s_destroyList;

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
        VideoRenderer *renderer = s_renderers[i];
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
    VideoRenderer *renderer = s_renderers[stream];
    if (renderer != NULL && !renderer->IsValidForSize(width, height)) {
        LockGuard destroyGuard(s_destroyListMutex);

        renderer->Detach();
        s_destroyList->push_back(renderer);
        renderer = NULL;
    }
    if (renderer == NULL) {
        s_renderers[stream] = new VideoRenderer(width, height, s_unityContext);
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
DestroyRenderer(long long textureId, EGLContext oglContext) {
    LockGuard lock(s_destroyListMutex);

    std::vector<VideoRenderer *> *newList = new std::vector<VideoRenderer *>();

    for (std::vector<VideoRenderer *>::iterator it = s_destroyList->begin();
            it != s_destroyList->end();
            it++) {
        VideoRenderer *renderer = *it;
        if (renderer->GetTargetTextureId() == textureId
            && renderer->GetOGLContext() == oglContext) {
            delete renderer;
        } else {
            newList->push_back(renderer);
        }
    }

    delete s_destroyList;
    s_destroyList = newList;
};

typedef void	(UNITY_INTERFACE_API *PluginLoadFunc)(IUnityInterfaces* unityInterfaces);
typedef void	(UNITY_INTERFACE_API *PluginUnloadFunc)();

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
InitializeVoximplant() {
    s_renderers = (VideoRenderer **) calloc(2, sizeof(VideoRenderer *));
    s_renderersMutex = new Mutex();
    s_destroyListMutex = new Mutex();
    s_destroyList = new std::vector<VideoRenderer *>();
    s_unityContext = eglGetCurrentContext();
    s_unitySurface = eglGetCurrentSurface(EGL_DRAW);
};