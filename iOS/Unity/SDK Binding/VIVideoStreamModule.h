/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

#import <Foundation/Foundation.h>
#import <VoxImplant/VoxImplant.h>

@class VIVideoStreamWrapper;

@interface VIVideoStreamModule : NSObject

+ (instancetype)sharedModule;

- (long long)textureForStream:(unsigned int)stream;

- (uint32_t)idForVideoStream:(NSString *)streamId;

- (void)addVideoStream:(VIVideoStream *)videoStream;

- (void)removeVideoStream:(NSString *)streamId;

- (VIVideoStreamWrapper *)videoStreamForId:(NSString *)streamId;
@end
