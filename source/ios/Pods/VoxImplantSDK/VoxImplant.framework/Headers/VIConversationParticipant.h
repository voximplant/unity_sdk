//
//  VIConversationParticipants.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 06.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import <Foundation/Foundation.h>

/**
 Interface that represents participant of a conversation.
 */
@interface VIConversationParticipant : NSObject

/**
 If 'true', user can add, remove and edit access rights for conversation participants (but not conversation moderators)
 */
@property(nonatomic,assign)  BOOL canManageParticipants;

/**
 If 'true', user can write to the conversation
 */
@property(nonatomic,assign)  BOOL canWrite;

/**
 Voximplant user identifier, ex _username@appname.accname_
 */
@property(nonatomic,copy,readonly)  NSString* userId;

/**
 Initialize conversation participant

 @param userId                 Voximplant user identifier, ex _username@appname.accname_
 @param canWrite               If 'true', user can write to the conversation
 @param canManageParticipants  If 'true', user can add, remove and edit access rights for conversation participants (but not conversation moderators)
 @return VIConversationParticipant instance
 */
- (instancetype)initWithUserId:(NSString*)userId canWrite:(BOOL)canWrite canManageParticipants:(BOOL)canManageParticipants;

@end
