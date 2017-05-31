//
//  VIAudioManager.h
//  Pods
//
//  Created by Andrey Syvrachev (asyvrachev@zingaya.com) on 04.04.17.
//  Copyright © 2017 VoxImplant (www.voximplant.com). All rights reserved.
//

#import <Foundation/Foundation.h>

/**
@class VIAudioManager
*/
@interface VIAudioManager : NSObject

- (instancetype)init NS_UNAVAILABLE;

/**
Obtain VIAudioManager instance
@method sharedAudioManager
@static
@return {instancetype} VIAudioManager instance
*/
+ (instancetype)sharedAudioManager;

/**
Enable or disable loudspeaker
@property useLoudSpeaker
@type {BOOL}
*/
@property(nonatomic,assign) BOOL useLoudSpeaker;

@end
