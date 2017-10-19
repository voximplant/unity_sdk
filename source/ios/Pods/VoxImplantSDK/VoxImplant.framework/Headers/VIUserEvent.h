//
//  VIUserEvent.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 16.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import "VIMessengerEvent.h"

@class VIUser;

/**
 Interface that represents messenger events related to user, such as get or edit user.
 */
@interface VIUserEvent : VIMessengerEvent

/**
 <VIUser> instance with user information
 */
@property(nonatomic,strong,readonly) VIUser* user;
@end

