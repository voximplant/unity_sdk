//
//  VIEndpoint.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 25.04.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import <Foundation/Foundation.h>

@class VIEndpoint;
@class VIVideoStream;
@class VICall;
@class VIEndpointStat;

/**
 Interface that mey be used to handle endpoint events
 */
@protocol VIEndpointDelegate<NSObject>

@optional

/**
 Triggered after endpoint added video stream to the call.
 Triggered always on the main thread, even delegateQueue is not the main thread
 
 @param endpoint Endpoint triggered this event
 @param videoStream Remote video stream added to the call
 */
- (void)endpoint:(VIEndpoint*)endpoint didAddRemoteVideoStream:(VIVideoStream*)videoStream;

/**
 Triggered after endpoint removed video stream from the call.
 Triggered always on the main thread, even delegateQueue is not the main thread
 
 @param endpoint Endpoint triggered this event
 @param videoStream Remote video stream removed from the call
 */
- (void)endpoint:(VIEndpoint*)endpoint didRemoveRemoteVideoStream:(VIVideoStream*)videoStream;

@end

/** VIEndpoint */
@interface VIEndpoint : NSObject

/**
 Delegate to handle the endpoint events.
 */
@property(nonatomic,weak) id<VIEndpointDelegate> delegate;


/**
 Call associated with the endpoint.
 */
@property(nonatomic,weak,readonly) VICall* call;

/**
 Statistics for the endpoint.
 */
@property(nonatomic,strong,readonly) VIEndpointStat* stat;

/**
 Video streams associated with the endpoint.
 */
@property(nonatomic,strong,readonly) NSArray<VIVideoStream*>* remoteVideoStreams;

/**
 The endpoint id.
 */
@property(nonatomic,strong,readonly) NSString* endpointId;

/**
 User name of the endpoint
 */
@property(nonatomic,strong,readonly) NSString* user;

/**
 SIP URI of the endpoint.
 */
@property(nonatomic,strong,readonly) NSString* sipURI;

/**
 User display name of the endpoint.
 */
@property(nonatomic,strong,readonly) NSString* userDisplayName;

@end
