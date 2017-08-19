//
//  VIConversation.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 01.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import <Foundation/Foundation.h>

@class VIMessage;
@class VIPayload;
@class VIConversationParticipant;
@interface VIConversation : NSObject

@property(nonatomic,strong,readonly) NSNumber* createdAt;
@property(nonatomic,strong,readonly) NSNumber* lastUpdate;

@property(nonatomic,strong) NSDictionary* customData;
@property(nonatomic,assign) BOOL distinct;
@property(nonatomic,assign) BOOL publicJoin;


@property(nonatomic,strong,readonly) NSNumber* lastRead;
@property(nonatomic,strong,readonly) NSNumber* lastSeq;


@property(nonatomic,strong,readonly) NSString* uuid;
@property(nonatomic,strong) NSString* title;

@property(nonatomic,strong,readonly) NSArray<VIConversationParticipant*>* participants;
@property(nonatomic,strong,readonly) NSArray<NSString*>* moderators;

- (void)addParticipants:(NSArray<VIConversationParticipant*>*) participants;
- (void)addModerators:(NSArray<NSString*>*) moderators;

- (void)editParticipants:(NSArray<VIConversationParticipant*>*) participants;

- (void)removeParticipants:(NSArray<VIConversationParticipant*>*) participants;
- (void)removeModerators:(NSArray<NSString*>*) moderators;

- (void)markAsRead:(NSNumber*)seq;
- (void)markAsDelivered:(NSNumber*)seq;

- (VIMessage*)sendMessage:(NSString*)message payload:(NSArray<VIPayload*>*)payload;

- (void)typing;

- (void)retransmitEventsFrom:(NSNumber*)from to:(NSNumber*)to;

- (void)update;

- (void)remove;
@end
