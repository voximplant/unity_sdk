/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

#import "VIRenderer.h"
#import "VIOpenGLRenderer.h"
#import "VIMetalRenderer.h"

@implementation VIRenderer

static Class kVICurrentRenderer = nil;
static EAGLContext *kUnityEAGLContext = nil;

+ (void)initializeRenderer:(UnityGfxRenderer)renderer {
    switch (renderer) {
        case kUnityGfxRendererMetal:
            kVICurrentRenderer = VIMetalRenderer.class;
            kUnityEAGLContext = nil;
            break;
        case kUnityGfxRendererOpenGLES20:
        case kUnityGfxRendererOpenGLES30:
            kVICurrentRenderer = VIOpenGLRenderer.class;
            kUnityEAGLContext = [EAGLContext currentContext];
            break;
        default:
            kVICurrentRenderer = nil;
            kUnityEAGLContext = nil;
            break;
    }
}

+ (nullable instancetype)rendererInstance {
    return static_cast<VIRenderer *>([kVICurrentRenderer new]);
}

+ (EAGLContext *)unityContext {
    return kUnityEAGLContext;
}

- (long long)texture {
    return 0;
}

- (void)setupRenderer:(CGSize)frameSize {
}

- (void)renderBuffer:(id <RTCI420Buffer>)buffer rotation:(RTCVideoRotation)rotation {
}

- (void)cleanup {
}

@end
