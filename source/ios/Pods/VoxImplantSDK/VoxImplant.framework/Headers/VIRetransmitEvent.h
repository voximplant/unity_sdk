//
//  VIRetransmitEvent.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 16.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import "VIMessengerEvent.h"

/**
 Interface that represents messenger event for <[VIConversation retransmitEventsFrom:to:]> method call result.
 */
@interface VIRetransmitEvent : VIMessengerEvent

/**
 Array of event objects that was retransmitted.
 */
@property(nonatomic,strong,readonly) NSArray<VIMessengerEventSeq*>* events;
@end
