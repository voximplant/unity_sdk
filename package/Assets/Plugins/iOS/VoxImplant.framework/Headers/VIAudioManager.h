//
//  VIAudioManager.h
//  VoxImplant
//
//  Created by Andrey Syvrachev (asyvrachev@zingaya.com) on 04.04.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import <Foundation/Foundation.h>

/** VIAudioManager interface */
@interface VIAudioManager : NSObject

- (instancetype)init NS_UNAVAILABLE;

/** Obtain VIAudioManager instance */
+ (instancetype)sharedAudioManager;

/** Enable or disable loudspeaker */
@property(nonatomic,assign) BOOL useLoudSpeaker;

@end

