//
//  VIMessageEvent.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 16.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import "VIMessengerEvent.h"

@class VIMessage;

/**
 Interface that represents messenger events related to messages.
 */
@interface VIMessageEvent : VIMessengerEventSeq

/**
 <VIMessage> instance with message information
 */
@property(nonatomic,strong,readonly) VIMessage* message;
@end
