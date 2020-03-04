/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

#import "VIVideoStreamModule.h"
#import "VIVideoStreamWrapper.h"

@interface VIVideoStreamModule ()

@property(strong, nonatomic) NSMutableDictionary<NSString *, VIVideoStreamWrapper *> *streamHolder;
@property(strong, nonatomic) NSMutableArray<NSString *> *streamIds;

@end

@implementation VIVideoStreamModule

+ (instancetype)sharedModule {
    static VIVideoStreamModule *instance;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        instance = [[VIVideoStreamModule alloc] init];
    });
    return instance;
}

- (instancetype)init {
    self = [super init];

    if (self) {
        _streamHolder = [NSMutableDictionary dictionary];
        _streamIds = [NSMutableArray array];
    }

    return self;
}

- (long long)textureForStream:(unsigned int)stream {
    NSString *streamId = _streamIds[stream];
    VIVideoStreamWrapper *videoStream = _streamHolder[streamId];
    return (long long) videoStream.texture;
}

- (uint32_t)idForVideoStream:(NSString *)streamId {
    return (uint32_t) [_streamIds indexOfObject:streamId];
}

- (void)addVideoStream:(VIVideoStream *)videoStream {
    _streamHolder[videoStream.streamId] = [VIVideoStreamWrapper withVideoStream:videoStream];
    [_streamIds addObject:videoStream.streamId];
}

- (void)removeVideoStream:(NSString *)streamId {
    [_streamHolder removeObjectForKey:streamId];
    [_streamIds removeObject:streamId];
}

- (VIVideoStreamWrapper *)videoStreamForId:(NSString *)streamId {
    return _streamHolder[streamId];
}

@end
