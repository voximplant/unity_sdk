//
//  VIEndPoint.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 25.04.17.
//  Copyright © 2017 Andrey Syvrachev. All rights reserved.
//

#import <Foundation/Foundation.h>

@class VIEndPoint;
@class VIVideoStream;
@class VICall;
@class VIEndPointStat;

/**
Interface that mey be used to handle endpoint events
@protocol VIEndPointDelegate
*/
@protocol VIEndPointDelegate<NSObject>

@optional

/**
Triggered after endpoint added video stream to the call.
Triggered always on the main thread, even delegateQueue is not the main thread
@method endPoint:didAddRemoteVideoStream:
@param {VIEndPoint*} endPoint Endpoint triggered this event
@param {VIVideoStream*} videoStream Remote video stream added to the call
*/
- (void)endPoint:(VIEndPoint*)endPoint didAddRemoteVideoStream:(VIVideoStream*)videoStream;

/**
Triggered after endpoint removed video stream from the call.
Triggered always on the main thread, even delegateQueue is not the main thre
@method endPoint:didRemoveRemoteVideoStream:
@param {VIEndPoint*} endPoint Endpoint triggered this event
@param {VIVideoStream*} videoStream Remote video stream removed from the call
*/
- (void)endPoint:(VIEndPoint*)endPoint didRemoveRemoteVideoStream:(VIVideoStream*)videoStream;

@end

/**
@class VIEndPoint
*/
@interface VIEndPoint : NSObject

/**
Delegate to handle the endpoint events.
@property delegate
@type {id<VIEndPointDelegate>}
*/
@property(nonatomic,weak) id<VIEndPointDelegate> delegate;

/**
Call associated with the endpoint.
@property call
@type {VICall*}
@readOnly
*/
@property(nonatomic,weak,readonly) VICall* call;

/**
Statistics for the endpoint.
@property stat
@type {VIEndPointStat*}
@readOnly
*/
@property(nonatomic,strong,readonly) VIEndPointStat* stat;

/**
Video streams associated with the endpoint.
@property remoteVideoStreams
@type {NSArray<VIVideoStream*>*}
@readOnly
*/
@property(nonatomic,strong,readonly) NSArray<VIVideoStream*>* remoteVideoStreams;

/**
The endpoint id.
@property endPointId
@type {NSString*}
@readOnly
*/
@property(nonatomic,strong,readonly) NSString* endPointId;

/**
User name of the endpoint
@property user
@type {NSString*}
@readOnly
*/
@property(nonatomic,strong,readonly) NSString* user;

/**
SIP URI of the endpoint.
@property sipURI
@ type {NSString*}
@readOnly
*/
@property(nonatomic,strong,readonly) NSString* sipURI;

/**
User display name of the endpoint.
@property userDisplayName
@type {NSString*}
@readOnly
*/
@property(nonatomic,strong,readonly) NSString* userDisplayName;

@end
