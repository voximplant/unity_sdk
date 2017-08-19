//
//  VIRetransmitEvent.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 16.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import "VIMessengerEvent.h"

@interface VIRetransmitEvent : VIMessengerEvent
@property(nonatomic,strong,readonly) NSArray<VIMessengerEventSeq*>* events;
@end
