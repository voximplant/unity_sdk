//
// Created by Aleksey Zinchenko on 08/03/2017.
// Copyright (c) 2017 voximplant. All rights reserved.
//

#import <WebRTC/WebRTC.h>

#import "VIEAGLRenderer.h"
#import "LockGuard.hpp"
#import "EAGLVideoRenderer.h"
#import "iOSNativeRenderer.h"
#import "VIBlocksThread.h"

@interface VIEAGLRenderer () <RTCVideoRenderer>

@property(nonatomic, copy, readonly) NativeTextureReportBlock block;
@property (nonatomic, strong, readonly) VIBlocksThread *thread;

@end

@implementation VIEAGLRenderer {
    int _stream;
    EAGLVideoRenderer *_renderer;
}

- (instancetype)initWithStream:(int)stream nativeTextureReport:(NativeTextureReportBlock)block {
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
    if (_renderer != NULL && !_renderer->IsValidForSize(width, height)) {
        [self destroyCurrentRenderer];

    }
    if (_renderer == NULL) {
        _renderer = new EAGLVideoRenderer(width, height, s_unityContext.sharegroup);
        s_renderers->push_back(_renderer);

        newRendererCreated = true;
    }

    _renderer->RenderBuffer(
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
        self.block(_renderer->GetTargetTextureId(), (__bridge void *) _renderer->GetEAGLContext(), width, height);
    }
}

- (void)destroyCurrentRenderer {
    _renderer->Detach();

    auto pos = find(s_renderers->begin(), s_renderers->end(), _renderer);
    s_renderers->erase(pos);

    if (_renderer->GetEAGLContext() != nil) {
            s_destroyList->AddObject((void *) _renderer->GetTargetTextureId(), (__bridge void *) _renderer->GetEAGLContext(), _renderer);
        } else {
            delete _renderer;
        }
    _renderer = NULL;
}

- (void)dealloc {
    [self destroyCurrentRenderer];
}


@end
