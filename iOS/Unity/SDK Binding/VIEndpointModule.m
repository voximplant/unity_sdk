/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

#import "VIEndpointModule.h"
#import "VIEmitter.h"
#import "VIVideoStreamModule.h"

@interface VIEndpointModule () <VIEndpointDelegate>

@property(nonatomic, strong, readonly) NSMutableDictionary<NSString *, VIEndpoint *> *endpointsHolder;

@end

@implementation VIEndpointModule

+ (instancetype)sharedModule {
    static VIEndpointModule *instance;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        instance = [[VIEndpointModule alloc] init];
    });
    return instance;
}

- (instancetype)init {
    self = [super init];
    if (self) {
        _endpointsHolder = [NSMutableDictionary dictionary];
    }
    return self;
}

- (void)addEndpoint:(VIEndpoint *)endpoint {
    _endpointsHolder[endpoint.endpointId] = endpoint;
    endpoint.delegate = self;
}

- (void)removeEndpoint:(NSString *)endpointId {
    _endpointsHolder[endpointId].delegate = nil;
    [_endpointsHolder removeObjectForKey:endpointId];
}

#pragma mark - VIEndpoint

- (NSString *)callIdForEndpoint:(NSString *)endpointId {
    VIEndpoint *endpoint = _endpointsHolder[endpointId];
    return endpoint.call.callId;
}

- (NSString *)remoteVideoStreamsForEndpoint:(NSString *)endpointId {
    VIEndpoint *endpoint = _endpointsHolder[endpointId];
    if (!endpoint) return nil;

    NSMutableArray<NSString *> *streams = [NSMutableArray arrayWithCapacity:endpoint.remoteVideoStreams.count];
    for (VIVideoStream *stream in endpoint.remoteVideoStreams) {
        [streams addObject:stream.streamId];
    }

    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:streams options:(NSJSONWritingOptions) 0 error:nil];
    return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
}

/*
- (NSString *)remoteAudioStreamsForEndpoint:(NSString *)endpointId {
    VIEndpoint *endpoint = _endpointsHolder[endpointId];
    if (!endpoint) return nil;

    NSMutableArray<NSString *> *streams = [NSMutableArray arrayWithCapacity:endpoint.remoteAudioStreams.count];
    for (VIAudioStream *stream in endpoint.remoteAudioStreams) {
        [streams addObject:stream.];
    }

    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:streams options:(NSJSONWritingOptions) 0 error:nil];
    return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
}
*/

- (NSString *)userForEndpoint:(NSString *)endpointId {
    VIEndpoint *endpoint = _endpointsHolder[endpointId];
    return endpoint.user;
}

- (NSString *)sipURIForEndpoint:(NSString *)endpointId {
    VIEndpoint *endpoint = _endpointsHolder[endpointId];
    return endpoint.sipURI;
}

- (NSString *)userDisplayNameForEndpoint:(NSString *)endpointId {
    VIEndpoint *endpoint = _endpointsHolder[endpointId];
    return endpoint.userDisplayName;
}

#pragma mark - VIEndpointDelegate

- (void)endpointInfoDidUpdate:(VIEndpoint *)endpoint {
    [VIEmitter sendEndpointMessage:endpoint.endpointId event:@"InfoUpdated"];
}

- (void)endpoint:(VIEndpoint *)endpoint didAddRemoteVideoStream:(VIVideoStream *)videoStream {
    [[VIVideoStreamModule sharedModule] addVideoStream:videoStream];
    [VIEmitter sendEndpointMessage:endpoint.endpointId event:@"RemoteVideoStreamAdded" payload:@{@"streamId": videoStream.streamId}];
}

- (void)endpoint:(VIEndpoint *)endpoint didRemoveRemoteVideoStream:(VIVideoStream *)videoStream {
    [[VIVideoStreamModule sharedModule] removeVideoStream:videoStream.streamId];
    [VIEmitter sendEndpointMessage:endpoint.endpointId event:@"RemoteVideoStreamRemoved" payload:@{@"streamId": videoStream.streamId}];
}

- (void)endpointDidRemove:(VIEndpoint *)endpoint {
    [VIEmitter sendEndpointMessage:endpoint.endpointId event:@"Removed"];
    [self removeEndpoint:endpoint.endpointId];
}

@end
