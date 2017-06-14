//
// Created by Aleksey Zinchenko on 10/05/2017.
// Copyright (c) 2017 voximplant. All rights reserved.
//

#import "VIBaseVideoStreamSource.h"

#import <VoxImplantSDK/VoxImplant/VoxImplant.h>

@interface VIBaseVideoStreamSource () <VICustomVideoSourceDelegate>

@property(nonatomic, assign, readwrite) BOOL isRunning;

@end

@implementation VIBaseVideoStreamSource

- (instancetype)initWithWidth:(NSUInteger)width height:(NSUInteger)height {
    self = [super init];
    if (self == nil) {
        return self;
    }

    VIVideoFormat *format = [[VIVideoFormat alloc] initWithFrame:CGSizeMake(width, height) fps:60];
    _videoSource = [[VICustomVideoSource alloc] initWithVideoFormats:@[format]];
    self.videoSource.delegate = self;

    return self;
}

#pragma mark - Public

- (void)sendVideoFrameFromTexture:(intptr_t)texturePtr width:(NSUInteger)width height:(NSUInteger)height {
    @throw [NSException exceptionWithName:@"Not implemented" reason:@"Internal error" userInfo:nil];
}

#pragma mark - VICustomVideoSourceDelegate

- (void)startWithVideoFormat:(VIVideoFormat *)format {
    self.isRunning = YES;
}

- (void)stop {
    self.isRunning = NO;
}

@end
