//
// Created by Aleksey Zinchenko on 08/03/2017.
// Copyright (c) 2017 voximplant. All rights reserved.
//

#import <WebRTC/WebRTC.h>

#import "EAGLRendererHolder.h"
#import "LockGuard.hpp"
#import "EAGLVideoRenderer.h"
#import "iOSNativeRenderer.h"
#import "BlocksThread.h"

@interface EAGLRendererHolder () <RTCVideoRenderer>

@property(nonatomic, copy, readonly) NativeTextureReportBlock block;
@property (nonatomic, strong, readonly) BlocksThread *thread;

@end

@implementation EAGLRendererHolder {
    int _stream;
}

- (instancetype)initWithStream:(int)stream nativeTextureReport:(NativeTextureReportBlock)block {
    self = [super init];
    if (self == nil)
        return self;

    _stream = stream;
    _block = block;
    _thread = [BlocksThread new];

    return self;
}

- (id <RTCVideoRenderer>)renderer {
    return self;
}

- (void)start {
    // NO OP
}

- (void)stop {
    // NO OP
}

#pragma mark - RTCVideoRenderer

- (void)setSize:(CGSize)size {
    // NO OP
}

- (void)renderFrame:(nullable RTCVideoFrame *)frame {
    if (frame == nil) {
        return;
    }

    [self.thread enqueueBlockAndWait:^{
        [self renderInternalFrame:frame];
    }];
}

- (void)renderInternalFrame:(RTCVideoFrame *) frame {
    LockGuard lock(s_renderersMutex);

    if (s_unityContext == nil) {
        return;
    }

    const int width = (int) frame.width;
    const int height = (int) frame.height;

    bool newRendererCreated = false;
    EAGLVideoRenderer *renderer = (EAGLVideoRenderer *) s_renderers[_stream];
    if (renderer != NULL && !renderer->IsValidForSize(width, height)) {
        renderer->Detach();
        if (renderer->GetEAGLContext() != nil) {
            s_destroyList->AddObject((void *) renderer->GetTargetTextureId(), (__bridge void *) renderer->GetEAGLContext(), renderer);
        } else {
            delete renderer;
        }
        renderer = NULL;
    }
    if (renderer == NULL) {
        s_renderers[_stream] = new EAGLVideoRenderer(width, height, s_unityContext.sharegroup);
        renderer = (EAGLVideoRenderer *) s_renderers[_stream];

        newRendererCreated = true;
    }

    renderer->RenderBuffer(
            frame.yPlane,
            frame.yPitch,
            frame.uPlane,
            frame.uPitch,
            frame.vPlane,
            frame.vPitch,
            width,
            height,
            frame.rotation
    );

    if (newRendererCreated) {
        self.block(renderer->GetTargetTextureId(), (__bridge void *) renderer->GetEAGLContext(), width, height);
    }
}


@end
