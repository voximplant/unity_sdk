//
//  VIMessageEvent.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 16.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import "VIMessengerEvent.h"

@class VIMessage;
@interface VIMessageEvent : VIMessengerEventSeq
@property(nonatomic,strong,readonly) VIMessage* message;
@end
