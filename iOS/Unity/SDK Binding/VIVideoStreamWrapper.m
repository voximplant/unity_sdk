/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

#import "VIVideoStreamWrapper.h"
#import "VIRenderer.h"
#import "VIVideoStreamModule.h"

@interface VIVideoStreamWrapper () <VIVideoRenderer>

@property(strong, nonatomic) VIRenderer *renderer;
@property(weak, nonatomic) VIVideoStream *nativeStream;
@property(strong, nonatomic) id <RTCI420Buffer> lastFrame;

@end

@implementation VIVideoStreamWrapper
+ (instancetype)withVideoStream:(VIVideoStream *)nativeStream {
    return [[VIVideoStreamWrapper alloc] initWithVideoStream:nativeStream];
}

- (instancetype)initWithVideoStream:(VIVideoStream *)nativeStream {
    self = [super init];
    if (self) {
        _nativeStream = nativeStream;
        [_nativeStream addRenderer:self];
    }
    return self;
}

- (NSInteger)width {
    if (_rotation == RTCVideoRotation_90 || _rotation == RTCVideoRotation_270) {
        return _lastFrame.height;
    }  else {
        return _lastFrame.width;
    }
}

- (NSInteger)height {
    if (_rotation == RTCVideoRotation_90 || _rotation == RTCVideoRotation_270) {
        return _lastFrame.width;
    }  else {
        return _lastFrame.height;
    }
}

- (long long)texture {
    long long textureId = _renderer.texture;
    return textureId;
}

- (void)setSize:(CGSize)size {
}

- (void)renderFrame:(nullable RTCVideoFrame *)frame {
    if (![frame.buffer conformsToProtocol:@protocol(RTCI420Buffer)]) {
        frame = frame.newI420VideoFrame;
    }

    _rotation = frame.rotation;
    _lastFrame = (id <RTCI420Buffer>) frame.buffer;
}


- (void)updateTexture {
//    NSLog(@"%s", __PRETTY_FUNCTION__);
    if (_lastFrame && _renderer) {
        @synchronized ([VIVideoStreamModule sharedModule]) {
            [_renderer setupRenderer:CGSizeMake(_lastFrame.width, _lastFrame.height)];
            [_renderer renderBuffer:_lastFrame rotation:_rotation];
        }
    }
}

- (void)didStart {
    _renderer = [VIRenderer rendererInstance];
}

- (void)didStop {
    [_renderer cleanup];
    _renderer = nil;
}

@end
