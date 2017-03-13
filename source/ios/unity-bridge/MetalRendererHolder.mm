//
// Created by Aleksey Zinchenko on 10/03/2017.
// Copyright (c) 2017 voximplant. All rights reserved.
//

#import <WebRTC/WebRTC.h>

#import "LockGuard.hpp"
#import "MetalRendererHolder.h"
#import "iOSNativeRenderer.h"
#import "MetalVideoRenderer.h"
#import "BlocksThread.h"

@interface MetalRendererHolder () <RTCVideoRenderer>

@property(nonatomic, copy, readonly) MetalTextureReportBlock block;
@property (nonatomic, strong, readonly) BlocksThread *thread;

@end

@implementation MetalRendererHolder {
    int _stream;
}

- (instancetype)initWithStream:(int)stream nativeTextureReport:(MetalTextureReportBlock)block {
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
        [self renderFrameInternal:frame];
    }];
}

- (void)renderFrameInternal:(nullable RTCVideoFrame *)frame {
    LockGuard lock(s_renderersMutex);

    const int width = (int) frame.width;
    const int height = (int) frame.height;

    bool newRendererCreated = false;
    MetalVideoRenderer *renderer = (MetalVideoRenderer *) s_renderers[_stream];
    if (renderer != NULL && !renderer->IsValidForSize(width, height)) {
        s_destroyList->AddObject((__bridge void *)renderer->GetMetalTexture(), renderer->GetMetalContext(), renderer);

        renderer = NULL;
    }
    if (renderer == NULL) {
        s_renderers[_stream] = new MetalVideoRenderer(width, height);
        renderer = (MetalVideoRenderer *) s_renderers[_stream];

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
        self.block(renderer->GetMetalTexture(), renderer->GetMetalContext(), width, height);
    }
}


@end