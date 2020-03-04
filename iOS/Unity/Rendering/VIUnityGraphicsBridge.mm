/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

#import <Foundation/Foundation.h>
#import <OpenGLES/EAGL.h>
#import "VIUnityGraphicsBridge.h"
#import "IUnityGraphics.h"
#import "VIRenderer.h"

static void UNITY_INTERFACE_API VoximplantOnGraphicsDeviceEvent(UnityGfxDeviceEventType eventType);

static IUnityInterfaces *s_unityInterfaces;
static IUnityGraphics *s_unityGraphics;

void VoximplantPluginLoad(IUnityInterfaces *unityInterfaces) {
    s_unityInterfaces = unityInterfaces;
    s_unityGraphics = s_unityInterfaces->Get<IUnityGraphics>();
    s_unityGraphics->RegisterDeviceEventCallback(VoximplantOnGraphicsDeviceEvent);

    VoximplantOnGraphicsDeviceEvent(kUnityGfxDeviceEventInitialize);
}

void VoximplantPluginUnload() {
    s_unityGraphics->UnregisterDeviceEventCallback(VoximplantOnGraphicsDeviceEvent);
}

static void UNITY_INTERFACE_API VoximplantOnGraphicsDeviceEvent(UnityGfxDeviceEventType eventType) {
    switch (eventType) {
        case kUnityGfxDeviceEventInitialize:
            [VIRenderer initializeRenderer:s_unityGraphics->GetRenderer()];
            break;
        case kUnityGfxDeviceEventShutdown:
            [VIRenderer initializeRenderer:kUnityGfxRendererNull];
            break;
        case kUnityGfxDeviceEventBeforeReset:
            break;
        case kUnityGfxDeviceEventAfterReset:
            break;
    }
}
