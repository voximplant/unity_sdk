/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

#import <Foundation/Foundation.h>
#import <VoxImplant/VoxImplant.h>

@interface VIVideoStreamWrapper : NSObject

+ (instancetype)withVideoStream:(VIVideoStream *)nativeStream;

- (NSInteger)width;

- (NSInteger)height;

@property(nonatomic, assign, readonly) RTCVideoRotation rotation;

- (long long)texture;

- (void)updateTexture;
@end
