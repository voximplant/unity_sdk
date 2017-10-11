//
//  VIUserStatusEvent.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 16.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import "VIMessengerEvent.h"

/**
 Interface that represents messenger events related to user presence state changes.
 */
@interface VIUserStatusEvent : VIMessengerEvent

/**
 User presence status.
 */
@property(nonatomic,assign,readonly) BOOL online;

/**
 UNIX timestamp that specifies the time event was triggered.
 */
@property(nonatomic,strong,readonly) NSNumber* timestamp;

@end
