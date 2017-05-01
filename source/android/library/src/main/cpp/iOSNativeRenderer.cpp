//
// Created by Aleksey Zinchenko on 07/03/2017.
// Copyright (c) 2017 voximplant. All rights reserved.#import "BaseImports.h"
//

//
// Created by Aleksey Zinchenko on 02/03/2017.
//

#include <cstdlib>

#include "BaseImports.h"

#include "IUnityGraphics.h"
#include "EAGLVideoRenderer.h"
#include "DestroyList.h"
#import "iOSNativeRenderer.h"

std::vector<BaseVideoRenderer *> *s_renderers;
Mutex *s_renderersMutex;
EAGLContext *s_unityContext;
UnityGfxRenderer s_unityGFXRenderer;

DestroyList<BaseVideoRenderer *> *s_destroyList;

void invalidateRenderers() {
    for (auto &renderer: *s_renderers) {
        renderer->Invalidate();
    }
}

static void UNITY_INTERFACE_API OnRenderEvent(int eventID) {
    if (eventID == 41) {
        s_renderersMutex->Acquire();
    } else if (eventID == 42) {
        bool invalidationEvent = false;
        if (s_unityGFXRenderer == kUnityGfxRendererOpenGLES20
            || s_unityGFXRenderer == kUnityGfxRendererOpenGLES30) {
            EAGLContext *context = [EAGLContext currentContext];
            if (s_unityContext != context) {
                s_unityContext = context;
                invalidationEvent = true;
            }
        }

        if (invalidationEvent) {
            invalidateRenderers();
        }

        s_renderersMutex->Release();
    }
}

extern "C" UnityRenderingEvent UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
GetRenderEventFunc()
{
    return OnRenderEvent;
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
DestroyRenderer(void *textureId, EAGLContext *context) {
    s_destroyList->DestroyObject(textureId, (__bridge void *)context);
};

typedef void	(UNITY_INTERFACE_API *PluginLoadFunc)(IUnityInterfaces* unityInterfaces);
typedef void	(UNITY_INTERFACE_API *PluginUnloadFunc)();

extern "C" void	UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API VoximplantPluginLoad(IUnityInterfaces* unityInterfaces)
{
    s_unityGFXRenderer = unityInterfaces->Get<IUnityGraphics>()->GetRenderer();
}
extern "C" void	UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API VoximplantPluginUnload() {};

typedef void	(*UnityPluginLoadFunc)(struct IUnityInterfaces* unityInterfaces);
typedef void	(*UnityPluginUnloadFunc)();
extern "C" void	UnityRegisterRenderingPluginV5(UnityPluginLoadFunc loadPlugin, UnityPluginUnloadFunc unloadPlugin);

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
InitializeVoximplant() {
    s_renderers = new std::vector<BaseVideoRenderer *>();
    s_renderersMutex = new Mutex();
    s_destroyList = new DestroyList<BaseVideoRenderer *>();
    s_unityContext = [EAGLContext currentContext];

    UnityRegisterRenderingPluginV5(VoximplantPluginLoad, VoximplantPluginUnload);
};
