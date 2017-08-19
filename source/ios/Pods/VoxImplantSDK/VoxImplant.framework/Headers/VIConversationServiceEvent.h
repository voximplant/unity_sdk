//
//  VIConversationServiceEvent.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 16.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import "VIMessengerEvent.h"

@interface VIConversationServiceEvent : VIMessengerEvent

@property(nonatomic,strong,readonly) NSString* conversationUUID;
@property(nonatomic,strong,readonly) NSNumber* seq;
@property(nonatomic,strong,readonly) NSNumber* timestamp;

@end
