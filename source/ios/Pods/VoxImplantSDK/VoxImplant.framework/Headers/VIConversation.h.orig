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


/**
 Interface that may be used to manage conversation.
 */
@interface VIConversation : NSObject

/**
 UNIX timestamp that specifies the time of the conversation creation
 */
@property(nonatomic,strong,readonly) NSNumber* createdAt;

/**
 UNIX timestamp that specifies the time of the last event in the conversation
 */
@property(nonatomic,strong,readonly) NSNumber* lastUpdate;

/**
 Dictionary with custom data, up to 5kb.
 */
@property(nonatomic,strong) NSDictionary* customData;

/**
 Check if the conversation is distinct.
 If two conversations are created with same set of users and moderators and both have 'distinct' flag, 
 second create call will fail with the UUID of conversation already created. 
 Note that changing users or moderators list will clear 'distinct' flag. 
 Note that setting this property does not send changes to the server. 
 Use the <-update> to send all changes at once.
 */
@property(nonatomic,assign) BOOL distinct;

/**
 Check if public join is enabled
 */
@property(nonatomic,assign) BOOL publicJoin;

/**
 Sequence of last event that was read by user
 */
@property(nonatomic,strong,readonly) NSNumber* lastRead;

/**
 Last event sequence for this conversation.
 */
@property(nonatomic,strong,readonly) NSNumber* lastSeq;

/**
 Universally unique identifier of current conversation.
 */
@property(nonatomic,strong,readonly) NSString* uuid;

/**
 Current conversation title
 */
@property(nonatomic,strong) NSString* title;

/**
  Array of <VIConversationParticipant> conversation participants alongside with their rights
 */
@property(nonatomic,strong,readonly) NSArray<VIConversationParticipant*>* participants;

/**
 Array of conversation moderator names
 */
@property(nonatomic,strong,readonly) NSArray<NSString*>* moderators;


/**
 Add new participants to the conversation. 
 Duplicated users are ignored. Will fail if any user does not exist.
 Triggers the <[VIMessengerDelegate messenger:didEditConversation:]> event for all messenger objects on all clients, including this one. 
 Clients that are not connected will receive it later.

 @param participants Array of <VIConversationParticipant> to be added to the conversation
 */
- (void)addParticipants:(NSArray<VIConversationParticipant*>*) participants;

/**
 Add new moderators to the conversation. 
 Duplicated users are ignored. Will fail if any user does not exist. 
 Triggers the <[VIMessengerDelegate messenger:didEditConversation:]> event for all messenger objects on all clients, including this one. 
 Clients that are not connected will receive it later.

 @param moderators  Array of moderators to be added to the conversation
 */
- (void)addModerators:(NSArray<NSString*>*) moderators;

/**
 Edit participants' access rights. 
 Duplicated users are ignored. Participant list must contain all participants. 
 Triggers the <[VIMessengerDelegate messenger:didEditConversation:]> event for all messenger objects on all clients, including this one.
 Clients that are not connected will receive it later.

 @param participants Array of <VIConversationParticipant>
 */
- (void)editParticipants:(NSArray<VIConversationParticipant*>*) participants;

/**
 Remove participants from the conversation. 
 Duplicated users are ignored. Will fail if any user does not exist. 
 Triggers the <[VIMessengerDelegate messenger:didEditConversation:]> event for all messenger objects on all clients, including this one.
 Clients that are not connected will receive it later.

 @param participants Array of <VIConversationParticipant> to be removed from conversation
 */
- (void)removeParticipants:(NSArray<VIConversationParticipant*>*) participants;

/**
 Remove moderators from the conversation. 
 Duplicated users are ignored. Will fail if any user does not exist. 
 Triggers the <[VIMessengerDelegate messenger:didEditConversation:]> event for all messenger objects on all clients, including this one.
 Clients that are not connected will receive it later.

 @param moderators Array of moderators to be removed from the conversation
 */
- (void)removeModerators:(NSArray<NSString*>*) moderators;

/**
 Mark the event with the specified sequence as 'read'. This affects 'lastRead' and is used to display unread messages and events.

 @param seq Sequence number of the event to be marked as read
 */
- (void)markAsRead:(NSNumber*)seq;

/**
 Mark event with the specified sequence as handled by current logged-in device. 
 If single user is logged in on multiple devices, this can be used to display delivery status.

 @param seq Sequence number of the event to be marked as delivered
 */
- (void)markAsDelivered:(NSNumber*)seq;

/**
 Send message to the conversation. 
 Triggers the <[VIMessengerDelegate messenger:didSendMessage:]> event for all messenger objects on all clients, including this one.

 @param message Message text
 @param payload Message payload
 @return <VIMessage> instance
 */
- (VIMessage*)sendMessage:(NSString*)message payload:(NSArray<VIPayload*>*)payload;

/**
 Calling this method will inform backend that user is typing some text. 
 Calls within 10s interval from the last call are discarded.
 */
- (void)typing;

/**
 Request events in the specified sequence range to be sent from server into this client. 
 Used to get history or get missed events in case of network disconnect.
 Please note that server will not push any events that was missed due to the client being offline. 
 Client should use this method to request all events based on the last event sequence received from the server 
 and last event sequence saved locally (if any).

 @param from First event in range sequence, inclusive
 @param to   Last event in range sequence, inclusive
 */
- (void)retransmitEventsFrom:(NSNumber*)from to:(NSNumber*)to;

/**
 Send conversation changes to the server. 
 The changes sent are: title, public join flag, distinct flag and custom data.
 */
- (void)update;

/**
 Remove current conversation. 
 All participants, including this one, will receive the <[VIMessengerDelegate messenger:didRemoveConversation:]> event.
 */
- (void)remove;
@end
