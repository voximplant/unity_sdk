//
//  VIUserStatusEvent.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 16.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import "VIMessengerEvent.h"

@interface VIUserStatusEvent : VIMessengerEvent

@property(nonatomic,assign,readonly) BOOL online;
@property(nonatomic,strong,readonly) NSNumber* timestamp;

@end
