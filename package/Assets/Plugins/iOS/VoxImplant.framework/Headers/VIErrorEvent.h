//
//  VIErrorEvent.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 16.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import "VIMessengerEvent.h"

@interface VIErrorEvent : VIMessengerEvent

@property(nonatomic,strong,readonly) NSString* descr; // "description" name busy by [NSObject description];
@property(nonatomic,assign,readonly) NSUInteger code;

@end
