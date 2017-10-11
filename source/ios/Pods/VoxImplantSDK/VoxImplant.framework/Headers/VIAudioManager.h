//
//  VIAudioManager.h
//  VoxImplant
//
//  Created by Andrey Syvrachev (asyvrachev@zingaya.com) on 04.04.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import <Foundation/Foundation.h>

/** Interface that may be used to manage audio devices on iOS device. */
@interface VIAudioManager : NSObject

/**
 @warning NS_UNAVAILABLE
 */
- (instancetype)init NS_UNAVAILABLE;

/** Obtain VIAudioManager instance */
+ (instancetype)sharedAudioManager;

/** Enable or disable loudspeaker */
@property(nonatomic,assign) BOOL useLoudSpeaker;

@end

