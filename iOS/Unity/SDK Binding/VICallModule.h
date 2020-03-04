/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

#import <Foundation/Foundation.h>
#import <VoxImplant/VoxImplant.h>

@interface VICallModule : NSObject

+ (instancetype)sharedModule;

- (void)addCall:(VICall *)call;

- (void)removeCall:(NSString *)callId;

#pragma mark - VICall

- (NSString *)endpointsForCall:(NSString *)callId;

- (NSString *)videoStreamsForCall:(NSString *)callId;

- (NSTimeInterval)durationForCall:(NSString *)callId;

- (void)setSendAudio:(BOOL)sendAudio forCall:(NSString *)callId;

- (BOOL)startReceiveVideoForCall:(NSString *)callId requestGuid:(NSString *)guid;

- (BOOL)setSendVideo:(BOOL)sendVideo forCall:(NSString *)callId requestGuid:(NSString *)guid;

- (BOOL)setHold:(BOOL)hold forCall:(NSString *)callId requestGuid:(NSString *)guid;

- (void)sendMessage:(NSString *)message toCall:(NSString *)callId;

- (void)sendInfo:(NSString *)content mimeType:(NSString *)mimeType headers:(NSString *)headers toCall:(NSString *)callId;

- (void)sendDTMF:(NSString *)dtmf toCall:(NSString *)callId;

- (void)startCall:(NSString *)callId;

- (void)answerCall:(NSString *)callId withReceiveVideo:(BOOL)receiveVideo sendVideo:(BOOL)sendVideo videoCodec:(NSInteger)videoCodec customData:(NSString *)customData headers:(NSString *)headers;

- (void)rejectCall:(NSString *)callId mode:(NSInteger)mode headers:(NSString *)headers;

- (void)hangupCall:(NSString *)callId headers:(NSString *)headers;

- (NSString *)createCustomVideoSource:(NSString *)callId frameSize:(CGSize)frameSize fps:(NSUInteger)fps;

- (void)sendVideoFrame:(void *)bytes forSource:(NSString *)sourceId;

- (void)releaseCustomVideoSource:(NSString *)sourceId;

@end
