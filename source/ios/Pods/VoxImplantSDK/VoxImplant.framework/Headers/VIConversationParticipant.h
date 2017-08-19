//
//  VIConversationParticipants.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 06.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface VIConversationParticipant : NSObject

@property(nonatomic,assign)  BOOL canManageParticipants;
@property(nonatomic,assign)  BOOL canWrite;
@property(nonatomic,copy,readonly)  NSString* userId;

- (instancetype)initWithUserId:(NSString*)userId canWrite:(BOOL)canWrite canManageParticipants:(BOOL)canManageParticipants;


@end
