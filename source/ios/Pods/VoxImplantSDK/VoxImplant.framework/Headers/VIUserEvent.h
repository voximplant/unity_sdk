//
//  VIUserEvent.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 16.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import "VIMessengerEvent.h"

@class VIUser;
@interface VIUserEvent : VIMessengerEvent
@property(nonatomic,strong,readonly) VIUser* user;
@end

