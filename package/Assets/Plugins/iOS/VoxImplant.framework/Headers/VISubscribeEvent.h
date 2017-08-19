//
//  VISubscribeEvent.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 16.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import "VIMessengerEvent.h"

@interface VISubscribeEvent : VIMessengerEvent
@property(nonatomic,strong,readonly) NSArray<NSString*>* users;
@end

