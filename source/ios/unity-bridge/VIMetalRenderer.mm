//
// Created by Aleksey Zinchenko on 10/03/2017.
// Copyright (c) 2017 voximplant. All rights reserved.
//

#import <WebRTC/WebRTC.h>

#import "LockGuard.hpp"
#import "VIMetalRenderer.h"
#import "iOSNativeRenderer.h"
#import "MetalVideoRenderer.h"
#import "VIBlocksThread.h"

@interface VIMetalRenderer ()

@property(nonatomic, copy, readonly) MetalTextureReportBlock block;
@property (nonatomic, strong, readonly) VIBlocksThread *thread;

@end

@implementation VIMetalRenderer {
    int _stream;
    MetalVideoRenderer *_renderer;
}

- (instancetype)initWithStream:(int)stream nativeTextureReport:(MetalTextureReportBlock)block {
    self = [super init];
    if (self == nil)
        return self;

    _stream = stream;
    _block = block;
    _thread = [VIBlocksThread new];

    return self;
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
    if (_renderer != NULL && !_renderer->IsValidForSize(width, height)) {
        [self destroyCurrentRenderer];

    }
    if (_renderer == NULL) {
        _renderer = new MetalVideoRenderer(width, height);

        newRendererCreated = true;
    }

    if (frame.dataY == nil) {
        frame = [frame newI420VideoFrame];
    }

    _renderer->RenderBuffer(
            frame.dataY,
            frame.strideY,
            frame.dataU,
            frame.strideU,
            frame.dataV,
            frame.strideV,
            width,
            height,
            frame.rotation
    );

    if (newRendererCreated) {
        self.block(_renderer->GetMetalTexture(), _renderer->GetMetalContext(), width, height);
    }
}

- (void)dealloc {
    [self destroyCurrentRenderer];
}

- (void)destroyCurrentRenderer {
    if (_renderer != NULL) {
        s_destroyList->AddObject((__bridge void *) _renderer->GetMetalTexture(), _renderer->GetMetalContext(), _renderer);
        
        _renderer = NULL;
    }
}


@end
