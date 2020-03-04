/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

#import "VICallModule.h"
#import "VIEmitter.h"
#import "VIVideoStreamModule.h"
#import "VIEndpointModule.h"

@interface VICallModule () <VICallDelegate, VIEndpointDelegate>

@property(nonatomic, strong, readonly) NSMutableDictionary<NSString *, VICall *> *callsHolder;
@property(nonatomic, strong, readonly) NSMutableDictionary<NSString *, VICustomVideoSource *> *customSources;
@property(nonatomic, strong, readonly) NSMutableDictionary<NSString *, VIVideoFormat *> *customSourceFormats;

@end

@implementation VICallModule

+ (instancetype)sharedModule {
    static VICallModule *instance;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        instance = [[VICallModule alloc] init];
    });
    return instance;
}

- (instancetype)init {
    self = [super init];
    if (self) {
        _callsHolder = [NSMutableDictionary dictionary];
        _customSources = [NSMutableDictionary dictionary];
        _customSourceFormats = [NSMutableDictionary dictionary];
    }
    return self;
}

- (void)addCall:(VICall *)call {
    _callsHolder[call.callId] = call;
    [call addDelegate:self];
}

- (void)removeCall:(NSString *)callId {
    [_callsHolder[callId] removeDelegate:self];
    [_callsHolder removeObjectForKey:callId];
}

#pragma mark - VICall

- (NSString *)endpointsForCall:(NSString *)callId {
    VICall *call = _callsHolder[callId];
    if (!call) return nil;

    NSMutableArray<NSString *> *endpoints = [NSMutableArray arrayWithCapacity:call.endpoints.count];
    for (VIEndpoint *endpoint in call.endpoints) {
        [endpoints addObject:endpoint.endpointId];
    }

    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:endpoints options:(NSJSONWritingOptions) 0 error:nil];
    return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
}

- (NSString *)videoStreamsForCall:(NSString *)callId {
    VICall *call = _callsHolder[callId];
    if (!call) return nil;

    NSMutableArray<NSString *> *videoStreams = [NSMutableArray arrayWithCapacity:call.localVideoStreams.count];
    for (VIVideoStream *videoStream in call.localVideoStreams) {
        [videoStreams addObject:videoStream.streamId];
    }

    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:videoStreams options:0 error:nil];
    return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
}

- (NSTimeInterval)durationForCall:(NSString *)callId {
    VICall *call = _callsHolder[callId];
    return call.duration;
}

- (void)setSendAudio:(BOOL)sendAudio forCall:(NSString *)callId {
    VICall *call = _callsHolder[callId];
    call.sendAudio = sendAudio;
}

- (BOOL)startReceiveVideoForCall:(NSString *)callId requestGuid:(NSString *)guid {
    VICall *call = _callsHolder[callId];
    if (!call) {return NO;}

    [call startReceiveVideoWithCompletion:^(NSError *error) {
        NSMutableDictionary *payload = [NSMutableDictionary dictionary];
        payload[@"requestGuid"] = guid;
        NSString *event;
        if (error) {
            event = @"ActionFailed";
            payload[@"code"] = @(error.code);
            payload[@"error"] = error.localizedDescription;
        } else {
            event = @"ActionCompleted";
        }
        [VIEmitter sendCallMessage:callId event:event payload:payload];
    }];

    return YES;
}

- (BOOL)setSendVideo:(BOOL)sendVideo forCall:(NSString *)callId requestGuid:(NSString *)guid {
    VICall *call = _callsHolder[callId];
    if (!call) {return NO;}

    [call setSendVideo:sendVideo completion:^(NSError *error) {
        NSMutableDictionary *payload = [NSMutableDictionary dictionary];
        payload[@"requestGuid"] = guid;
        NSString *event;
        if (error) {
            event = @"ActionFailed";
            payload[@"code"] = @(error.code);
            payload[@"error"] = error.localizedDescription;
        } else {
            event = @"ActionCompleted";
        }
        [VIEmitter sendCallMessage:callId event:event payload:payload];
    }];

    return YES;
}

- (BOOL)setHold:(BOOL)hold forCall:(NSString *)callId requestGuid:(NSString *)guid {
    VICall *call = _callsHolder[callId];
    if (!call) {return NO;}

    [call setHold:hold completion:^(NSError *error) {
        NSMutableDictionary *payload = [NSMutableDictionary dictionary];
        payload[@"requestGuid"] = guid;
        NSString *event;
        if (error) {
            event = @"ActionFailed";
            payload[@"code"] = @(error.code);
            payload[@"error"] = error.localizedDescription;
        } else {
            event = @"ActionCompleted";
        }
        [VIEmitter sendCallMessage:callId event:event payload:payload];
    }];

    return YES;
}

- (void)sendMessage:(NSString *)message toCall:(NSString *)callId {
    VICall *call = _callsHolder[callId];
    [call sendMessage:message];
}

- (void)sendInfo:(NSString *)content mimeType:(NSString *)mimeType headers:(NSString *)headers toCall:(NSString *)callId {
    VICall *call = _callsHolder[callId];
    NSDictionary *hdrs = nil;
    if (headers) {
        NSError *error;
        hdrs = [NSJSONSerialization JSONObjectWithData:[headers dataUsingEncoding:NSUTF8StringEncoding] options:0 error:&error];
        if (error) {
            NSLog(@"Failed to process headers: %@", error.localizedDescription);
        }
    }
    [call sendInfo:content mimeType:mimeType headers:hdrs];
}

- (void)sendDTMF:(NSString *)dtmf toCall:(NSString *)callId {
    VICall *call = _callsHolder[callId];
    [call sendDTMF:dtmf];
}

- (void)answerCall:(NSString *)callId withReceiveVideo:(BOOL)receiveVideo sendVideo:(BOOL)sendVideo videoCodec:(NSInteger)videoCodec customData:(NSString *)customData headers:(NSString *)headers {
    VICall *call = _callsHolder[callId];

    VICallSettings *callSettings = [VICallSettings new];
    callSettings.videoFlags = [VIVideoFlags videoFlagsWithReceiveVideo:receiveVideo sendVideo:sendVideo];
    callSettings.preferredVideoCodec = (VIVideoCodec) videoCodec;
    callSettings.customData = customData;
    if (headers) {
        NSError *error;
        callSettings.extraHeaders = [NSJSONSerialization JSONObjectWithData:[headers dataUsingEncoding:NSUTF8StringEncoding] options:0 error:&error];
        if (error) {
            NSLog(@"Failed to process headers: %@", error.localizedDescription);
        }
    }

    [call answerWithSettings:callSettings];
}

- (void)rejectCall:(NSString *)callId mode:(NSInteger)mode headers:(NSString *)headers {
    VICall *call = _callsHolder[callId];

    NSDictionary *hdrs = nil;
    if (headers) {
        NSError *error;
        hdrs = [NSJSONSerialization JSONObjectWithData:[headers dataUsingEncoding:NSUTF8StringEncoding] options:0 error:&error];
        if (error) {
            NSLog(@"Failed to process headers: %@", error.localizedDescription);
        }
    }

    [call rejectWithMode:(VIRejectMode) mode headers:hdrs];
}

- (void)startCall:(NSString *)callId {
    VICall *call = _callsHolder[callId];
    [call start];
}

- (void)hangupCall:(NSString *)callId headers:(NSString *)headers {
    VICall *call = _callsHolder[callId];

    NSDictionary *hdrs = nil;
    if (headers) {
        NSError *error;
        hdrs = [NSJSONSerialization JSONObjectWithData:[headers dataUsingEncoding:NSUTF8StringEncoding] options:0 error:&error];
        if (error) {
            NSLog(@"Failed to process headers: %@", error.localizedDescription);
        }
    }

    [call hangupWithHeaders:hdrs];
}

- (NSString *)createCustomVideoSource:(NSString *)callId frameSize:(CGSize)frameSize fps:(NSUInteger)fps {
    VICall *call = _callsHolder[callId];
    NSString *uuid = [NSUUID UUID].UUIDString;

    VIVideoFormat *videoFormat = [[VIVideoFormat alloc] initWithFrame:frameSize fps:fps];
    _customSourceFormats[uuid] = videoFormat;
    VICustomVideoSource *videoSource = [[VICustomVideoSource alloc] initWithVideoFormats:@[videoFormat]];
    call.videoSource = videoSource;
    _customSources[uuid] = videoSource;

    return uuid;
}

- (void)sendVideoFrame:(void *)bytes forSource:(NSString *)sourceId {
    VICustomVideoSource *customVideoSource = _customSources[sourceId];
    if (customVideoSource) {
        CVPixelBufferRef buffer = NULL;
        NSDictionary *options = @{
                (id) kCVPixelBufferCGImageCompatibilityKey: @YES,
                (id) kCVPixelBufferCGBitmapContextCompatibilityKey: @YES,
        };

        VIVideoFormat *format = _customSourceFormats[sourceId];
        size_t width = (size_t) format.frame.width;
        size_t height = (size_t) format.frame.height;

        CVPixelBufferCreateWithBytes(kCFAllocatorDefault,
                width, height,
                kCVPixelFormatType_32BGRA, bytes,
                width * sizeof(int32_t),
                NULL, NULL, (__bridge CFDictionaryRef) options, &buffer);
        [customVideoSource sendVideoFrame:buffer rotation:VIRotation_0];
        CVPixelBufferRelease(buffer);
    }
}

- (void)releaseCustomVideoSource:(NSString *)sourceId {
//    VICustomVideoSource *customVideoSource = _customSources[sourceId];
//    [customVideoSource release];
    [_customSources removeObjectForKey:sourceId];
    [_customSourceFormats removeObjectForKey:sourceId];
}

#pragma mark - VICallDelegate

- (void)call:(VICall *)call didFailWithError:(NSError *)error headers:(nullable NSDictionary *)headers {
    NSMutableDictionary *payload = [NSMutableDictionary dictionary];
    payload[@"code"] = @(error.code);
    payload[@"error"] = error.localizedDescription;
    if (headers) {
        payload[@"headers"] = [VIEmitter flatten:headers];
    }
    [VIEmitter sendCallMessage:call.callId event:@"Failed" payload:payload];
}

- (void)call:(VICall *)call didConnectWithHeaders:(nullable NSDictionary *)headers {
    NSDictionary *payload = nil;
    if (headers) {
        payload = @{@"headers": [VIEmitter flatten:headers]};
    }
    [VIEmitter sendCallMessage:call.callId event:@"Connected" payload:payload];
}

- (void)call:(VICall *)call didDisconnectWithHeaders:(nullable NSDictionary *)headers answeredElsewhere:(NSNumber *)answeredElsewhere {
    NSMutableDictionary *payload = [NSMutableDictionary dictionary];
    payload[@"answeredElsewhere"] = answeredElsewhere;
    if (headers) {
        payload[@"headers"] = [VIEmitter flatten:headers];
    }
    [VIEmitter sendCallMessage:call.callId event:@"Disconnected" payload:payload];
}

- (void)call:(VICall *)call startRingingWithHeaders:(nullable NSDictionary *)headers {
    NSMutableDictionary *payload = [NSMutableDictionary dictionary];
    if (headers) {
        payload[@"headers"] = [VIEmitter flatten:headers];
    }
    [VIEmitter sendCallMessage:call.callId event:@"Ringing" payload:payload];
}

- (void)callDidStartAudio:(VICall *)call {
    [VIEmitter sendCallMessage:call.callId event:@"AudioStarted"];
}

- (void)call:(VICall *)call didReceiveMessage:(NSString *)message headers:(nullable NSDictionary *)headers {
    NSMutableDictionary *payload = [NSMutableDictionary dictionary];
    payload[@"text"] = message;
    if (headers) {
        payload[@"headers"] = [VIEmitter flatten:headers];
    }
    [VIEmitter sendCallMessage:call.callId event:@"MessageReceived" payload:payload];
}

- (void)call:(VICall *)call didReceiveInfo:(NSString *)body type:(NSString *)type headers:(nullable NSDictionary *)headers {
    NSMutableDictionary *payload = [NSMutableDictionary dictionary];
    payload[@"type"] = type;
    payload[@"body"] = body;
    if (headers) {
        payload[@"headers"] = [VIEmitter flatten:headers];
    }
    [VIEmitter sendCallMessage:call.callId event:@"SIPInfoReceived" payload:payload];
}

- (void)call:(VICall *)call didAddLocalVideoStream:(VIVideoStream *)videoStream {
    [[VIVideoStreamModule sharedModule] addVideoStream:videoStream];
    [VIEmitter sendCallMessage:call.callId event:@"LocalVideoStreamAdded" payload:@{@"streamId": videoStream.streamId}];
}

- (void)call:(VICall *)call didRemoveLocalVideoStream:(VIVideoStream *)videoStream {
    [[VIVideoStreamModule sharedModule] removeVideoStream:videoStream.streamId];
    [VIEmitter sendCallMessage:call.callId event:@"LocalVideoStreamRemoved" payload:@{@"streamId": videoStream.streamId}];
}

- (void)call:(VICall *)call didAddEndpoint:(VIEndpoint *)endpoint {
    [[VIEndpointModule sharedModule] addEndpoint:endpoint];
    [VIEmitter sendCallMessage:call.callId event:@"EndpointAdded" payload:@{@"endpointId": endpoint.endpointId}];
}

- (void)iceCompleteForCall:(VICall *)call {
    [VIEmitter sendCallMessage:call.callId event:@"ICECompleted"];
}

- (void)iceTimeoutForCall:(VICall *)call {
    [VIEmitter sendCallMessage:call.callId event:@"ICETimeout"];
}


@end
