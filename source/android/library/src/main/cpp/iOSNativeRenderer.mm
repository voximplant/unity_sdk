//
// Created by Aleksey Zinchenko on 07/03/2017.
// Copyright (c) 2017 voximplant. All rights reserved.#import "BaseImports.h"
//

//
// Created by Aleksey Zinchenko on 02/03/2017.
//

#include <stdint.h>
#include <cstdlib>

#include "BaseImports.h"

#include "IUnityGraphics.h"
#include "EAGLVideoRenderer.h"
#include "DestroyList.h"
#include "voxImplantIosSDK.h"

EAGLVideoRenderer **s_renderers;
Mutex *s_renderersMutex;
EAGLContext *s_unityContext;

DestroyList<EAGLVideoRenderer *> *s_destroyList;

void invalidateRenderers() {
    EAGLVideoRenderer *renderer = s_renderers[0];
    if (renderer) {
        renderer->Invalidate();
    }
    renderer = s_renderers[1];
    if (renderer) {
        renderer->Invalidate();
    }
}

static void UNITY_INTERFACE_API OnRenderEvent(int eventID) {
    if (eventID == 41) {
        s_renderersMutex->Acquire();
    } else if (eventID == 42) {
        bool invalidationEvent = false;
        EAGLContext *context = [EAGLContext currentContext];
        if (s_unityContext != context) {
            s_unityContext = context;
            invalidationEvent = true;
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

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
InitializeVoximplant() {
    s_renderers = (EAGLVideoRenderer **) calloc(2, sizeof(EAGLVideoRenderer *));
    s_renderersMutex = new Mutex();
    s_destroyList = new DestroyList<EAGLVideoRenderer *>();
    s_unityContext = [EAGLContext currentContext];
};
