/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

#import <Foundation/Foundation.h>
#import <VoxImplant/VoxImplant.h>

@interface VIAudioManagerModule : NSObject

+ (instancetype)sharedInstance;

- (void)selectAudioDevice:(VIAudioDeviceType)device;

- (VIAudioDeviceType)currentAudioDevice;

- (NSString *)availableAudioDevices;

@end
