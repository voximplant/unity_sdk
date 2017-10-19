//
//  VIConversationServiceEvent.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 16.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import "VIMessengerEvent.h"

/**
 Interface that represents messenger events such as typing, isRead, isDelivered.
 */
@interface VIConversationServiceEvent : VIMessengerEvent

/**
 Conversation UUID associated with this event.
 */
@property(nonatomic,strong,readonly) NSString* conversationUUID;

/**
 Sequence number of event
 */
@property(nonatomic,strong,readonly) NSNumber* seq;

/**
 UNIX timestamp that specifies the time a method was called
 */
@property(nonatomic,strong,readonly) NSNumber* timestamp;

@end
