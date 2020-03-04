/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */


#import <CoreGraphics/CoreGraphics.h>
#import <WebRTC/WebRTC.h>
#import "IUnityGraphics.h"

NS_ASSUME_NONNULL_BEGIN

@interface VIRenderer : NSObject

+ (void)initializeRenderer:(UnityGfxRenderer)renderer;

+ (nullable instancetype)rendererInstance;

- (long long)texture;

- (void)setupRenderer:(CGSize)frameSize;

- (void)renderBuffer:(id <RTCI420Buffer>)buffer rotation:(RTCVideoRotation)rotation;

- (void)cleanup;


@end

NS_ASSUME_NONNULL_END
