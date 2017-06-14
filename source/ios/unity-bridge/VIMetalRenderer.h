//
// Created by Aleksey Zinchenko on 10/03/2017.
// Copyright (c) 2017 voximplant. All rights reserved.
//

#import <Foundation/Foundation.h>

#import <VoxImplant/Voximplant.h>

#import "VIEAGLRenderer.h"

NS_ASSUME_NONNULL_BEGIN

typedef void(^MetalTextureReportBlock)(id<MTLTexture>, void *, int, int);

@interface VIMetalRenderer : NSObject <RTCVideoRenderer>

- (instancetype)init NS_UNAVAILABLE;
- (instancetype)initWithStream:(int)stream nativeTextureReport:(MetalTextureReportBlock) block NS_DESIGNATED_INITIALIZER;

@end

NS_ASSUME_NONNULL_END