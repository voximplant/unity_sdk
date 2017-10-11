//
//  VIUser.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 07.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import <Foundation/Foundation.h>

/**
 Interface that represents user description.
 */
@interface VIUser : NSObject

/**
 @warning NS_UNAVAILABLE
 */
- (instancetype)init NS_UNAVAILABLE;

/**
 Array of UUIDs for the conversations the user has joined.
 */
@property(nonatomic,strong,readonly) NSArray<NSString*>* conversationList;

/**
 Public custom data available to all users
 */
@property(nonatomic,strong,readonly) NSDictionary* customData;

/**
 Private custom data available only to the user himself.
 */
@property(nonatomic,strong,readonly) NSDictionary* privateCustomData;

/**
  Voximplant user identifier, ex _username@appname.accname_.
 */
@property(nonatomic,strong,readonly) NSString* userId;

/**
 Array of messenger notifications that current user is subscribed to
 
 @see <VIMessengerNotification>
 */
@property(nonatomic,strong,readonly) NSArray<NSNumber*>* notifications;

@end
