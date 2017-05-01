//
// Created by Andrey Syvrachev (asyvrachev@zingaya.com) on 05.04.17.
// Copyright © 2017 VoxImplant (www.voximplant.com). All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol RTCVideoRenderer;

@class VIStreamStat;
@interface VIVideoStream : NSObject

@property (nonatomic,strong,readonly) NSSet<id<RTCVideoRenderer>> *renderers;
@property (nonatomic,strong,readonly) VIStreamStat* stat; // updated every 5 seconds, may be nil before first update
@property (nonatomic,copy, readonly)  NSString* streamId;

- (instancetype)init NS_UNAVAILABLE;

- (void)addRenderer:(id<RTCVideoRenderer>)renderer;
- (void)removeRenderer:(id<RTCVideoRenderer>)renderer;
- (void)removeAllRenderers;

@end
