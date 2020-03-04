/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

#import "IUnityInterface.h"
@class EAGLContext;

typedef void    (*UnityPluginLoadFunc)(struct IUnityInterfaces *unityInterfaces);

typedef void    (*UnityPluginUnloadFunc)(void);

#if __cplusplus
extern "C"
{
#endif

void UnityRegisterRenderingPluginV5(UnityPluginLoadFunc loadPlugin, UnityPluginUnloadFunc unloadPlugin);

void    UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API VoximplantPluginLoad(IUnityInterfaces *unityInterfaces);
void    UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API VoximplantPluginUnload(void);

#if __cplusplus
}
#endif

