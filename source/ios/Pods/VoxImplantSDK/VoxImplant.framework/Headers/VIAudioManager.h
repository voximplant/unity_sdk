//
//  VIAudioManager.h
//  Pods
//
//  Created by Andrey Syvrachev (asyvrachev@zingaya.com) on 04.04.17.
//  Copyright © 2017 VoxImplant (www.voximplant.com). All rights reserved.
//

#import <Foundation/Foundation.h>

@interface VIAudioManager : NSObject

- (instancetype)init NS_UNAVAILABLE;

+ (instancetype)sharedAudioManager;

@property(nonatomic,assign) BOOL useLoudSpeaker;

@end
