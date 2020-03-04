/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

#import <Foundation/Foundation.h>

@interface VIEmitter : NSObject

+ (void)sendClientMessage:(NSString *)event;
+ (void)sendClientMessage:(NSString *)event payload:(NSDictionary *)payload;

+ (void)sendCallMessage:(NSString *)callId event:(NSString *)event;
+ (void)sendCallMessage:(NSString *)callId event:(NSString *)event payload:(NSDictionary *)payload;

+ (void)sendEndpointMessage:(NSString *)endpointId event:(NSString *)event;
+ (void)sendEndpointMessage:(NSString *)endpointId event:(NSString *)event payload:(NSDictionary *)payload;

+ (void)sendAudioManagerMessage:(NSString *)event payload:(NSDictionary *)payload;

+ (NSArray *)flatten:(NSDictionary *)dictionary;

@end
