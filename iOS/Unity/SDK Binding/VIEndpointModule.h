/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

#import <Foundation/Foundation.h>
#import <VoxImplant/VoxImplant.h>

@interface VIEndpointModule : NSObject

+ (instancetype)sharedModule;

- (void)addEndpoint:(VIEndpoint *)endpoint;

- (void)removeEndpoint:(NSString *)endpointId;

#pragma mark - VIEndpoint

- (NSString *)callIdForEndpoint:(NSString *)endpointId;
- (NSString *)remoteVideoStreamsForEndpoint:(NSString *)endpointId;
//- (NSString *)remoteAudioStreamsForEndpoint:(NSString *)endpointId;
- (NSString *)userForEndpoint:(NSString *)endpointId;
- (NSString *)sipURIForEndpoint:(NSString *)endpointId;
- (NSString *)userDisplayNameForEndpoint:(NSString *)endpointId;

@end
