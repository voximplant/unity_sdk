//
//  VISubscribeEvent.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 16.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import "VIMessengerEvent.h"

/**
 Interface that represents messenger events related to subscriptions.
 */
@interface VISubscribeEvent : VIMessengerEvent

/**
  Array of Voximplant user identifiers of current (un)subscription
 */
@property(nonatomic,strong,readonly) NSArray<NSString*>* users;
@end

