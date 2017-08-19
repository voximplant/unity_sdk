//
//  VIMessengerPushNotificationProcessing.h
//  VoxImplant
//
//  Created by Yulia Grigorieva (grigorieva@zingaya.com) on 28/07/2017.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import <Foundation/Foundation.h>

@class VIMessengerEvent;

@interface VIMessengerPushNotificationProcessing : NSObject

- (instancetype)init NS_UNAVAILABLE;

+ (instancetype)sharedMessengerPushNotificationProcessing;

- (VIMessengerEvent*)processPushNotification:(NSDictionary*)notification;

@end
