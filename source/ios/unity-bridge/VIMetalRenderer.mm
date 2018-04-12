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

    if (![frame.buffer conformsToProtocol:@protocol(RTCI420Buffer)]) {
        frame = [frame newI420VideoFrame];
    }

    id<RTCI420Buffer> buffer = static_cast<id <RTCI420Buffer>>(frame.buffer);
    _renderer->RenderBuffer(
            buffer.dataY,
            buffer.strideY,
            buffer.dataU,
            buffer.strideU,
            buffer.dataV,
            buffer.strideV,
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
