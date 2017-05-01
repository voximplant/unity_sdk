//
//  VICall.h
//  VoxSDK
//
//  Created by Andrey Syvrachev (asyvrachev@zingaya.com) on 18.12.16.
//  Copyright © 2017 VoxImplant (www.voximplant.com). All rights reserved.
//

#import <Foundation/Foundation.h>

@class VICall;
@class VIVideoStream;
@class VICallStat;
@class VIEndPoint;

/*!
 @protocol VICallDelegate
 */
@protocol VICallDelegate <NSObject>

@optional

/*!
 Triggered if the call has failed
 @method call:didFailWithError:headers:
 @param call instance
 @param error Error
 @param headers Optional headers passed with event
 */
- (void)call:(VICall *)call didFailWithError:(NSError *)error headers:(NSDictionary*)headers;

/**
 Triggered if the call was successfully connected
 @method call:didConnectWithHeaders:
 @param call call instance
 @param headers Optional headers passed with event
 */
- (void)call:(VICall*)call didConnectWithHeaders:(NSDictionary *)headers;

/**
 Triggerred if the call was disconnected
 @method call:didDisconnectWithHeaders:answeredElsewhere:
 @param call call instance
 @param headers Optional headers passed with event
 @param answeredElsewhere True if answered else where
 */
- (void)call:(VICall*)call didDisconnectWithHeaders:(NSDictionary *)headers answeredElsewhere:(NSNumber*)answeredElsewhere;

/**
 Triggered if the call is ringing. You should start playing call progress tone now
 @method call:startRingingWithHeaders:
 @param call call instance
 @param headers Optional headers passed with event
 */
- (void)call:(VICall *)call startRingingWithHeaders:(NSDictionary *)headers;

/**
 Triggered if call audio is started. You should stop playing progress tone when event is received
 @method callDidStartAudio:
 @param call call instance
 */
- (void)callDidStartAudio:(VICall*)call;


/**
 Triggered if the instant message is received within a call
 @method call:didReceiveMessage:headers:
 @param call call instance
 @param headers Optional headers passed with event
 */
- (void)call:(VICall*)call didReceiveMessage:(NSString*)message headers:(NSDictionary*)headers;

/**
 Triggered if info is received within a call
 @method call:withType:didReceiveInfo:type:headers:
 @param call call instance
 @param body Body of info message
 @param type MIME type of info
 @param headers Optional headers passed with event
 */
- (void)call:(VICall*)call didReceiveInfo:(NSString*)body type:(NSString*)type headers:(NSDictionary*)headers;

/**
 Triggered if the statistics data is ready
 @method onNetStatsReceivedInCall:withStats:
 @param call call instance
 @param stat statistics
 */
- (void)call:(VICall *)call didReceiveStatistics:(VICallStat*)stat;


// following functions called only on main thread (not on delegateQueue!) 

- (void)call:(VICall *)call didAddLocalVideoStream:(VIVideoStream*)videoStream;

- (void)call:(VICall *)call didRemoveLocalVideoStream:(VIVideoStream*)videoStream;

@end



typedef void (^VICompletionBlock)(NSError* error); // error == nil -> means success

@protocol RTCVideoRenderer;
@class UIView;
@interface VICall : NSObject

- (instancetype)init NS_UNAVAILABLE;

- (void)addDelegate:(id<VICallDelegate>)delegate;
- (void)removeDelegate:(id<VICallDelegate>)delegate;

@property(nonatomic,strong,readonly) NSString* callId;
@property(nonatomic,strong,readonly) NSArray<VIEndPoint*>* endPoints;

@property(nonatomic,strong,readonly) VICallStat* stat; // last updated statistics, update period 5 seconds

@property(nonatomic,assign) BOOL sendAudio;

- (NSTimeInterval)duration;

/**
 Start outgoing or answers incoming alerting call
 @method startWithVideo:headers:
 @param video Call with video or not
 @param headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
 */
- (void)startWithVideo:(BOOL)video headers:(NSDictionary*)headers;

/**
 Terminates call. Call must be either established, or outgoing progressing.
 Or rejects incoming call waiting answer.
 @method stopWithHeaders:headers:
 @param  headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
 */
- (void)stopWithHeaders:(NSDictionary*)headers;


- (void)setSendVideo:(BOOL)video completion:(VICompletionBlock)completion;
- (void)setHold:(BOOL)hold completion:(VICompletionBlock)completion;

- (void)sendMessage:(NSString*)message headers:(NSDictionary*)headers;
- (void)sendInfo:(NSString*)body mimeType:(NSString*)mimeType headers:(NSDictionary*)headers;

- (BOOL)sendDTMF:(NSString*)dtmf;

@end

@interface VICall(Convinence)

/**
 Answers incoming alerting call. Recommend to use startWithVideo:headers instead.
 @method answerWithVideo:headers:
 @param video Answer with video or not
 @param headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
 */
- (void)answerWithVideo:(BOOL)video headers:(NSDictionary*)headers; // can use startWithHeaders instead

/**
 Rejects incoming alerting call. Recommend to use startWithVideo:headers instead.
 @method rejectWithHeaders:headers:
 @param  headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
 */
- (void)rejectWithHeaders:(NSDictionary*)headers; // can use stopWithHeaders instead

/**
 Terminates call. Call must be either established, or outgoing progressing
 @method hangupWithHeaders:headers:
 @param headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
 */
- (void)hangupWithHeaders:(NSDictionary*)headers; // can use stopWithHeaders instead

@end


@interface VICall(Streams)

@property (nonatomic, strong, readonly) NSArray<VIVideoStream*>* localVideoStreams;

@end
