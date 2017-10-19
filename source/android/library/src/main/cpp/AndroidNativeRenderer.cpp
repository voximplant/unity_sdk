//
// Created by Aleksey Zinchenko on 02/03/2017.
//

#include <cstdlib>
#include <vector>
#include <memory>

#include <GLES2/gl2.h>
#include <jni.h>

#include "IUnityGraphics.h"
#include "EGLVideoRenderer.h"
#include "DestroyList.h"

static std::vector<std::unique_ptr<EGLVideoRenderer>> *s_renderers;
static Mutex *s_renderersMutex;
static EGLContext s_unityContext;
static EGLSurface s_unitySurface;

static DestroyList<std::unique_ptr<EGLVideoRenderer>> *s_destroyList;

void invalidateAllRenderers();

static JNIEnv* env;

void invalidateAllRenderers() {
    for (auto &renderer: *s_renderers) {
        if (renderer) {
            renderer->Invalidate();
        }
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

jint JNI_OnLoad(JavaVM* vm, void* reserved)
{
    if (vm->GetEnv(reinterpret_cast<void**>(&env), JNI_VERSION_1_6) != JNI_OK) {
        return -1;
    }

    return JNI_VERSION_1_6;
}

JNIEXPORT static void JNICALL
Java_acquireRenderLock(JNIEnv *env,
                       jobject thiz) {
    s_renderersMutex->Acquire();
}
JNIEXPORT static void JNICALL
Java_releaseRenderLock(JNIEnv *env,
                       jobject thiz) {
    s_renderersMutex->Release();
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

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
InitializeVoximplant() {
    s_renderers = new std::vector<std::unique_ptr<EGLVideoRenderer>>();
    s_renderers->emplace(s_renderers->begin(), nullptr);
    s_renderers->emplace(s_renderers->begin(), nullptr);

    s_renderersMutex = new Mutex();
    s_destroyList = new DestroyList<std::unique_ptr<EGLVideoRenderer>>();

    s_unityContext = eglGetCurrentContext();
    s_unitySurface = eglGetCurrentSurface(EGL_DRAW);
};