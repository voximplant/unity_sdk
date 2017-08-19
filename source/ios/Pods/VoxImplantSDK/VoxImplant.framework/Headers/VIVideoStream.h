//
// Created by Andrey Syvrachev (asyvrachev@zingaya.com) on 05.04.17.
// Copyright © 2017 VoxImplant (www.voximplant.com). All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol RTCVideoRenderer;
@class VIStreamStat;

/**
Interface representing local and remote video streams. It may be used to add or remove video renderers.
@class VIVideoStream
*/
@interface VIVideoStream : NSObject

/**
Video renderers associated with the stream.
@property renderers
@type {NSSet<id<RTCVideoRenderer>> *}
@readOnly
*/
@property (nonatomic,strong,readonly) NSSet<id<RTCVideoRenderer>> *renderers;

/**
Statistics for the video stream. Updated every 5 seconds.
@property stat
@type {VIStreamStat*}
@readOnly
*/
@property (nonatomic,strong,readonly) VIStreamStat* stat;

/**
The video stream id.
@property streamId
@type {NSString*}
@readOnly
*/
@property (nonatomic,copy, readonly)  NSString* streamId;

- (instancetype)init NS_UNAVAILABLE;

/**
Add new video renderer to the video stream.
@method addRenderer:
@param {id<RTCVideoRenderer>} New video renderer to be added
*/
- (void)addRenderer:(id<RTCVideoRenderer>)renderer;

/**
Remove previously added video renderer from the video stream.
@method removeRenderer:
@param {id<RTCVideoRenderer>} Previously added video renderer
*/
- (void)removeRenderer:(id<RTCVideoRenderer>)renderer;

/**
Remove all video renderers associated with the video stream
@method removeAllRenderers
*/
- (void)removeAllRenderers;

@end
