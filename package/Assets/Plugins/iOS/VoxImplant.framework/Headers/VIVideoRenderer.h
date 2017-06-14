//
//  VIVideoRenderer.h
//  VoxImplant
//
//  Created by Andrey Syvrachev (asyvrachev@zingaya.com) on 05.04.17.
//  Copyright © 2017 VoxImplant (www.voximplant.com). All rights reserved.
//

#import <WebRTC/WebRTC.h>

/**
@protocol VIVideoRenderer
*/
@protocol VIVideoRenderer <RTCVideoRenderer>

@optional
/**
Triggered when the video renderer is started
@method didStart
*/
- (void)didStart;

/**
Triggered when the video renderer is stopped
@method didStop
*/
- (void)didStop;

@end
