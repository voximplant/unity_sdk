//
//  VIConversationEvent.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 16.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import "VIMessengerEvent.h"

@class VIConversation;
@interface VIConversationEvent : VIMessengerEventSeq
@property(nonatomic,strong) VIConversation* conversation;
@end

