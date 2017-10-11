//
//  VIVideoRenderer.h
//  VoxImplant
//
//  Created by Andrey Syvrachev (asyvrachev@zingaya.com) on 05.04.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import <WebRTC/WebRTC.h>

@protocol VIVideoRenderer <RTCVideoRenderer>

@optional
/**
Triggered when the video renderer is started
*/
- (void)didStart;

/**
Triggered when the video renderer is stopped
*/
- (void)didStop;

@end
