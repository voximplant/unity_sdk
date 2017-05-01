//
//  VIVideoRenderer.h
//  VoxImplant
//
//  Created by Andrey Syvrachev (asyvrachev@zingaya.com) on 05.04.17.
//  Copyright © 2017 VoxImplant (www.voximplant.com). All rights reserved.
//

#import <WebRTC/WebRTC.h>

@protocol VIVideoRenderer <RTCVideoRenderer>

@optional

- (void)didStart;

- (void)didStop;

@end
