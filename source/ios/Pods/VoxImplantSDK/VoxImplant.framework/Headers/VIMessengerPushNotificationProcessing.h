//
//  VIMessengerPushNotificationProcessing.h
//  VoxImplant
//
//  Created by Yulia Grigorieva (grigorieva@zingaya.com) on 28/07/2017.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import <Foundation/Foundation.h>

@class VIMessengerEvent;


/**
 Helper to process incoming VoxImplant messenger push notifications.
 */
@interface VIMessengerPushNotificationProcessing : NSObject

/**
 @warning NS_UNAVAILABLE
 */
- (instancetype)init NS_UNAVAILABLE;

/**
 Get VIMessengerPushNotificationProcessing instance

 @return VIMessengerPushNotificationProcessing instance
 */
+ (instancetype)sharedMessengerPushNotificationProcessing;

/**
 Process incoming VoxImplant messenger push notification and return appropriate messenger event object inheriting <VIMessengerEvent>.

 @param notification Incoming push notification that comes from [this method](https://developer.apple.com/documentation/uikit/uiapplicationdelegate/1623013-application?language=objc)
 @return <VIMessengerEvent> object or nil if the notification is not VoxImplant messenger push notification
 */
- (VIMessengerEvent*)processPushNotification:(NSDictionary*)notification;

@end
