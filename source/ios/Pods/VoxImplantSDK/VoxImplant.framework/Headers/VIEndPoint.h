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
@protocol VIEndPointDelegate<NSObject>

@optional

// following functions called only on main thread (even on delegateQueue is not main thread)
- (void)endPoint:(VIEndPoint*)endPoint didAddRemoteVideoStream:(VIVideoStream*)videoStream;

- (void)endPoint:(VIEndPoint*)endPoint didRemoveRemoteVideoStream:(VIVideoStream*)videoStream;

@end


@interface VIEndPoint : NSObject

@property(nonatomic,weak) id<VIEndPointDelegate> delegate;

@property(nonatomic,weak,readonly) VICall* call;
@property(nonatomic,strong,readonly) VIEndPointStat* stat;

@property(nonatomic,strong,readonly) NSArray<VIVideoStream*>* remoteVideoStreams;

@property(nonatomic,strong,readonly) NSString* endPointId;
@property(nonatomic,strong,readonly) NSString* user;
@property(nonatomic,strong,readonly) NSString* sipURI;
@property(nonatomic,strong,readonly) NSString* userDisplayName;

@end
