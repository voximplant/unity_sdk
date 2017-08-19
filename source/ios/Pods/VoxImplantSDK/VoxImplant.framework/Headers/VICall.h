//
//  VICall.h
//  VoxImplant
//
//  Created by Andrey Syvrachev (asyvrachev@zingaya.com) on 18.12.16.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import <Foundation/Foundation.h>

@class VICall;
@class VIVideoStream;
@class VICallStat;
@class VIEndpoint;

/** VICallDelegate */
@protocol VICallDelegate <NSObject>

@optional

/**
 Triggered if the call is failed.
 
 @param call Call that triggered the event
 @param error Error that contains status code and status message of the call failure
 @param headers Optional headers passed with event
 */
- (void)call:(VICall *)call didFailWithError:(NSError *)error headers:(NSDictionary*)headers;

/**
 Triggered after call was successfully connected.
 
 @param call Call that triggered the event
 @param headers Optional headers passed with event
 */
- (void)call:(VICall*)call didConnectWithHeaders:(NSDictionary *)headers;

/**
 Triggered after the call was disconnected.
 
 @param call Call that triggered the event
 @param headers Optional headers passed with event
 @param answeredElsewhere true if call was answered on another device
 */
- (void)call:(VICall*)call didDisconnectWithHeaders:(NSDictionary *)headers answeredElsewhere:(NSNumber*)answeredElsewhere;

/**
 Triggered if the call is ringing. You should start playing call progress tone now.
 
 @param call Call that triggered the event
 @param headers Optional headers passed with event
 */
- (void)call:(VICall *)call startRingingWithHeaders:(NSDictionary *)headers;

/**
 Triggered after audio is started in the call. You should stop playing progress tone when event is received
 
 @param call Call that triggered the event
 */
- (void)callDidStartAudio:(VICall*)call;

/**
 Triggered when message is received within the call. Implemented atop SIP INFO for communication between call endpoint and Voximplant cloud, and is separated from Voximplant messaging API.
 
 @param call Call that triggered the event
 @param message Content of the message
 @param headers Optional headers passed with event
 */
- (void)call:(VICall*)call didReceiveMessage:(NSString*)message headers:(NSDictionary*)headers;

/**
 Triggered when INFO message is received within the call.
 
 @param call Call that triggered the event
 @param body Body of INFO message
 @param type MIME type of INFO message
 @param headers Optional headers passed with event
 */
- (void)call:(VICall*)call didReceiveInfo:(NSString*)body type:(NSString*)type headers:(NSDictionary*)headers;

/**
 Triggered when call statistics are available for the call.
 
 @param call Call that triggered the event
 @param stat Call statistics
 */
- (void)call:(VICall *)call didReceiveStatistics:(VICallStat*)stat;

/**
 Triggered when local video stream is added to the call. The event is triggered on the main thread.
 
 @param call Call that triggered the event
 @param videoStream Local video stream that is added to the call
 */
- (void)call:(VICall *)call didAddLocalVideoStream:(VIVideoStream*)videoStream;

/**
 Triggered when local video stream is removed from the call. The event is triggered on the main thread.
 
 @param call Call that triggered the event
 @param videoStream Local video stream that is removed to the call
 */
- (void)call:(VICall *)call didRemoveLocalVideoStream:(VIVideoStream*)videoStream;

/**
 Triggered when ICE connection is complete

 @param call Call that triggered the event
 */
- (void)iceCompleteForCall:(VICall*)call;

/**
 Triggered if connection was not established due to network connection problem between 2 peers

 @param call Call that triggered the event
 */
- (void)iceTimeoutForCall:(VICall*)call;

@end


/**
 Completion callback.
 @param error If set to 'nil' this means success.
 */
typedef void (^VICompletionBlock)(NSError* error);

@protocol RTCVideoRenderer;
@class UIView;
@class VIVideoSource;

/** VICall */
@interface VICall : NSObject

- (instancetype)init NS_UNAVAILABLE;

/**
 Preferred video codec, for example: @"H264". Nil by default.  Must be set before using "startWithHeaders:", if needed
 */
@property(nonatomic,strong) NSString* preferredVideoCodec;

/**
 Video source.By default "[VICameraManager sharedCameraManager]" (gets video from back or front camera). Must be set before using "startWithHeaders:", if needed
 */
@property(nonatomic,strong) VIVideoSource* videoSource;

/**
 Add call delegate to handle call events.
 
 @param delegate Call delegate
 */
- (void)addDelegate:(id<VICallDelegate>)delegate;

/**
 Remove previously added call delegate.
 
 @param delegate Call delegate
 */
- (void)removeDelegate:(id<VICallDelegate>)delegate;

/**
 The call id.
 */
@property(nonatomic,strong,readonly) NSString* callId;

/**
 Array of the endpoints associated with the call.
 */
@property(nonatomic,strong,readonly) NSArray<VIEndpoint*>* endpoints;

/**
 Call statistics. Updated every 5 seconds.
 */
@property(nonatomic,strong,readonly) VICallStat* stat;

/**
 Enables or disables audio transfer from microphone into the call.
 */
@property(nonatomic,assign) BOOL sendAudio;

/**
 Get the call duration
 
 @return Call duration
 */
- (NSTimeInterval)duration;

/**
 Start outgoing call
 
 @param headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
 */
- (void)startWithHeaders:(NSDictionary*)headers;

/**
 Terminate established or outgoing processing call, or reject incoming processing call.
 
 @param headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
 */
- (void)stopWithHeaders:(NSDictionary*)headers;

/**
 Start or stop sending video for the call.
 
 @param video  True if video should be sent, false otherwise
 @param completion Completion block to handle the result of the operation
 */
- (void)setSendVideo:(BOOL)video completion:(VICompletionBlock)completion;

/**
 Hold or unhold the call
 
 @param hold True if the call should be put on hold, false for unhold
 @param completion Completion block to handle the result of the operation
 */
- (void)setHold:(BOOL)hold completion:(VICompletionBlock)completion;

/**
 Start receive video if video receive was not enabled before. Stop receiving video during the call is not supported.

 @param completion Completion block to handle the result of operation
 */
- (void)startReceiveVideoWithCompletion:(VICompletionBlock)completion;

/**
 Send message within the call. Implemented atop SIP INFO for communication between call endpoint and Voximplant cloud, and is separated from Voximplant messaging API.
 
 @param message Message text
 @param headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
 */
- (void)sendMessage:(NSString*)message headers:(NSDictionary*)headers;

/**
 Send INFO message within the call
 
 @param body Custom string data
 @param mimeType MIME type of info
 @param headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
 */
- (void)sendInfo:(NSString*)body mimeType:(NSString*)mimeType headers:(NSDictionary*)headers;

/**
 Send DTMF within the call
 
 @param dtmf DTMFs
 @return True if DTMFs are sent successfully, false otherwise
 */
- (BOOL)sendDTMF:(NSString*)dtmf;

/**
 Answer incoming call.
 
 @param sendVideo Specify if video receive is enabled for a call
 @param receiveVideo Specify if video receive is enabled for a call
 @param customData Optinal custom data passed with call. Will be available in VoxEngine scenario
 @param headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
 */
- (void)answerWithSendVideo:(BOOL)sendVideo
               receiveVideo:(BOOL)receiveVideo
                 customData:(NSString*)customData
                    headers:(NSDictionary*)headers;

/**
 Reject incoming call.
 
 @param headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
 */
- (void)rejectWithHeaders:(NSDictionary*)headers;

/**
 Terminates call. Call should be either established, or outgoing progressing
 
 @param headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
 */
- (void)hangupWithHeaders:(NSDictionary*)headers;
@end


@interface VICall(Streams)

/**
 Local video streams associated with the call
 */
@property (nonatomic, strong, readonly) NSArray<VIVideoStream*>* localVideoStreams;

@end
