//
//  VIMessenger.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 02.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import <Foundation/Foundation.h>

@class VIMessenger;
@class VIConversation;
@class VIMessage;
@class VIUser;
@class VIPayload;


@class VIConversationParticipant;
@class VISubscribeEvent;
@class VIErrorEvent;
@class VIUserEvent;
@class VIUserStatusEvent;
@class VIConversationEvent;
@class VIMessageEvent;
@class VIRetransmitEvent;
@class VIConversationServiceEvent;

typedef NS_ENUM(NSInteger,VIMessengerNotification) {
    VIMessengerNotificationEditMessage,
    VIMessengerNotificationSendMessage
};

@protocol VIMessengerDelegate <NSObject>
- (void)messenger:(VIMessenger*)messenger didGetUser:(VIUserEvent*)event;
- (void)messenger:(VIMessenger*)messenger didEditUser:(VIUserEvent*)event;
- (void)messenger:(VIMessenger*)messenger didSubscribe:(VISubscribeEvent*)event;
- (void)messenger:(VIMessenger*)messenger didUnsubscribe:(VISubscribeEvent*)event;
- (void)messenger:(VIMessenger*)messenger didReceiveError:(VIErrorEvent*)event;
- (void)messenger:(VIMessenger*)messenger didSetStatus:(VIUserStatusEvent*)event;
- (void)messenger:(VIMessenger*)messenger didCreateConversation:(VIConversationEvent*)event;
- (void)messenger:(VIMessenger*)messenger didEditConversation:(VIConversationEvent*)event;
- (void)messenger:(VIMessenger*)messenger didRemoveConversation:(VIConversationEvent*)event;
- (void)messenger:(VIMessenger*)messenger didGetConversation:(VIConversationEvent*)event;
- (void)messenger:(VIMessenger*)messenger didSendMessage:(VIMessageEvent*)event;
- (void)messenger:(VIMessenger*)messenger didEditMessage:(VIMessageEvent*)event;
- (void)messenger:(VIMessenger*)messenger didRemoveMessage:(VIMessageEvent*)event;
- (void)messenger:(VIMessenger*)messenger didReceiveTypingNotification:(VIConversationServiceEvent*)event;
- (void)messenger:(VIMessenger*)messenger didReceiveReadConfirmation:(VIConversationServiceEvent*)event;
- (void)messenger:(VIMessenger*)messenger didReceiveDeliveryConfirmation:(VIConversationServiceEvent*)event;

- (void)messenger:(VIMessenger*)messenger didRetransmitEvents:(VIRetransmitEvent*)event;
@end

@interface VIMessenger : NSObject
- (void)addDelegate:(id<VIMessengerDelegate>)delegate;
- (void)removeDelegate:(id<VIMessengerDelegate>)delegate;

- (void)createConversation:(NSArray<VIConversationParticipant*>*)participants
                moderators:(NSArray<NSString*>*)moderators
                     title:(NSString*)title
                  distinct:(BOOL)distinct
          enablePublicJoin:(BOOL)enablePublicJoin
                customData:(NSDictionary*)customData;

- (void)leaveConversation:(NSString*)uuid;
- (void)joinConversation:(NSString*)uuid;
- (void)removeConversation:(NSString*)uuid;
- (NSString*)getMe;
- (void)getUser:(NSString*)user;
- (void)getUsers:(NSArray<NSString*>*)users;
- (void)editUserWithCustomData:(NSDictionary*)customData privateCustomData:(NSDictionary*)privateCustomData;
- (void)subscribe:(NSArray<NSString*>*)users;
- (void)unsubscribe:(NSArray<NSString*>*)users;
- (void)getConversation:(NSString*)uuid;
- (void)getConversations:(NSArray<NSString*>*)uuids;
- (void)setStatus:(BOOL)online;
- (void)managePushNotifications:(NSArray<NSNumber*>*) notifications;

- (VIConversation*)recreateConversation:(NSArray<VIConversationParticipant*>*)participants
                                  title:(NSString*)title
                               distinct:(BOOL)distinct
                       enablePublicJoin:(BOOL)enablePublicJoin
                             customData:(NSDictionary*)customData
                                   uuid:(NSString*)uuid
                               sequence:(NSNumber*)sequnce
                             moderators:(NSArray<NSString*>*)moderators
                             lastUpdate:(NSNumber*)lastUpdate
                               lastRead:(NSNumber*)lastRead
                              createdAt:(NSNumber*)createdAt;

- (VIMessage*)recreateMessage:(NSString*)uuid
                 conversation:(NSString*)conversationUUID
                       sender:(NSString*)sender
                         text:(NSString*)text
                      payload:(NSArray<VIPayload*>*)payload
                     sequence:(NSNumber*)sequence;


@end
