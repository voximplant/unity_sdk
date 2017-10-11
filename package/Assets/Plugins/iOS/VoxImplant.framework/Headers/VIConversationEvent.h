//
//  VIConversationEvent.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 16.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import "VIMessengerEvent.h"

@class VIConversation;

/**
 Interface that represents messenger events related to conversation such as create, edit, remove and others.
 */
@interface VIConversationEvent : VIMessengerEventSeq
/**
 <VIConversation> instance with conversation information
 */
@property(nonatomic,strong) VIConversation* conversation;
@end

