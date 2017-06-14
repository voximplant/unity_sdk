//
// Created by Aleksey Zinchenko on 10/05/2017.
// Copyright (c) 2017 voximplant. All rights reserved.
//

#import <Foundation/Foundation.h>

@class VICall;
@class VICustomVideoSource;

NS_ASSUME_NONNULL_BEGIN

@interface VIBaseVideoStreamSource : NSObject

@property(nonatomic, strong, readonly) VICustomVideoSource *videoSource;
@property(nonatomic, assign, readonly) BOOL isRunning;

- (instancetype)initWithWidth:(NSUInteger)width height:(NSUInteger)height NS_DESIGNATED_INITIALIZER;
- (instancetype)init NS_UNAVAILABLE;

- (void)sendVideoFrameFromTexture:(intptr_t)texturePtr width:(NSUInteger)width height:(NSUInteger)height;

@end

NS_ASSUME_NONNULL_END