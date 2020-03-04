/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

#import "VIEmitter.h"
#include "VINativeInterface.h"

@implementation VIEmitter

+ (void)sendClientMessage:(NSString *)event {
    [self sendClientMessage:event payload:nil];
}

+ (void)sendClientMessage:(NSString *)event payload:(NSDictionary *)payload {
    NSMutableDictionary *eventPayload = [NSMutableDictionary dictionary];
    eventPayload[@"event"] = event;
    NSError *error;
    if (payload) {
        NSData *jsonPayloadData = [NSJSONSerialization dataWithJSONObject:payload options:(NSJSONWritingOptions) 0 error:&error];
        if (jsonPayloadData) {
            eventPayload[@"payload"] = [[NSString alloc] initWithData:jsonPayloadData encoding:NSUTF8StringEncoding];
        } else {
            NSLog(@"Failed to process JSON payload: %@", error.localizedDescription);
            return;
        }
    }
    NSData *jsonEventData = [NSJSONSerialization dataWithJSONObject:eventPayload options:(NSJSONWritingOptions) 0 error:&error];
    if (jsonEventData) {
        voximplant_send_message("OnClientEvent", [[NSString alloc] initWithData:jsonEventData encoding:NSUTF8StringEncoding]);
    } else {
        NSLog(@"Failed to process JSON event: %@", error.localizedDescription);
    }
}

+ (void)sendCallMessage:(NSString *)callId event:(NSString *)event {
    [self sendCallMessage:callId event:event payload:nil];
}

+ (void)sendCallMessage:(NSString *)callId event:(NSString *)event payload:(NSDictionary *)payload {
    NSMutableDictionary *eventPayload = [NSMutableDictionary dictionary];
    eventPayload[@"senderId"] = callId;
    eventPayload[@"event"] = event;
    NSError *error;
    if (payload) {
        NSData *jsonPayloadData = [NSJSONSerialization dataWithJSONObject:payload options:(NSJSONWritingOptions) 0 error:&error];
        if (jsonPayloadData) {
            eventPayload[@"payload"] = [[NSString alloc] initWithData:jsonPayloadData encoding:NSUTF8StringEncoding];
        } else {
            NSLog(@"Failed to process JSON payload: %@", error.localizedDescription);
            return;
        }
    }
    NSData *jsonEventData = [NSJSONSerialization dataWithJSONObject:eventPayload options:(NSJSONWritingOptions) 0 error:&error];
    if (jsonEventData) {
        voximplant_send_message("OnCallEvent", [[NSString alloc] initWithData:jsonEventData encoding:NSUTF8StringEncoding]);
    } else {
        NSLog(@"Failed to process JSON event: %@", error.localizedDescription);
    }
}

+ (void)sendEndpointMessage:(NSString *)endpointId event:(NSString *)event {
    [VIEmitter sendEndpointMessage:endpointId event:event payload:nil];
}

+ (void)sendEndpointMessage:(NSString *)endpointId event:(NSString *)event payload:(NSDictionary *)payload {
    NSMutableDictionary *eventPayload = [NSMutableDictionary dictionary];
    eventPayload[@"senderId"] = endpointId;
    eventPayload[@"event"] = event;
    NSError *error;
    if (payload) {
        NSData *jsonPayloadData = [NSJSONSerialization dataWithJSONObject:payload options:(NSJSONWritingOptions) 0 error:&error];
        if (jsonPayloadData) {
            eventPayload[@"payload"] = [[NSString alloc] initWithData:jsonPayloadData encoding:NSUTF8StringEncoding];
        } else {
            NSLog(@"Failed to process JSON payload: %@", error.localizedDescription);
            return;
        }
    }
    NSData *jsonEventData = [NSJSONSerialization dataWithJSONObject:eventPayload options:(NSJSONWritingOptions) 0 error:&error];
    if (jsonEventData) {
        voximplant_send_message("OnEndpointEvent", [[NSString alloc] initWithData:jsonEventData encoding:NSUTF8StringEncoding]);
    } else {
        NSLog(@"Failed to process JSON event: %@", error.localizedDescription);
    }
}

+ (void)sendAudioManagerMessage:(NSString *)event payload:(NSDictionary *)payload {
    NSMutableDictionary *eventPayload = [NSMutableDictionary dictionary];
    eventPayload[@"event"] = event;

    NSError *error;
    NSData *jsonPayloadData = [NSJSONSerialization dataWithJSONObject:payload options:(NSJSONWritingOptions) 0 error:&error];
    if (jsonPayloadData) {
        eventPayload[@"payload"] = [[NSString alloc] initWithData:jsonPayloadData encoding:NSUTF8StringEncoding];
    } else {
        NSLog(@"Failed to process JSON payload: %@", error.localizedDescription);
        return;
    }
    NSData *jsonEventData = [NSJSONSerialization dataWithJSONObject:eventPayload options:(NSJSONWritingOptions) 0 error:&error];
    if (jsonEventData) {
        voximplant_send_message("OnAudioManagerEvent", [[NSString alloc] initWithData:jsonEventData encoding:NSUTF8StringEncoding]);
    } else {
        NSLog(@"Failed to process JSON event: %@", error.localizedDescription);
    }
}

+ (NSArray *)flatten:(NSDictionary *)dictionary {
    NSMutableArray *result = [NSMutableArray array];
    for (NSObject *key in dictionary.allKeys) {
        [result addObject:key];
        [result addObject:dictionary[key]];
    }
    return result;
}

@end
