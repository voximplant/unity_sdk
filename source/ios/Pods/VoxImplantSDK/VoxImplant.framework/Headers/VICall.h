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

/**
@protocol VICallDelegate
*/
@protocol VICallDelegate <NSObject>

@optional

/**
Triggered if the call is failed.
@method call:didFailWithError:headers:
@param {VICall*} call Call that triggered the event
@param {NSError*} error Error that contains status code and status message of the call failure
@param {NSDictionary*} headers Optional headers passed with event
*/
- (void)call:(VICall *)call didFailWithError:(NSError *)error headers:(NSDictionary*)headers;

/**
Triggered after call was successfully connected.
@method call:didConnectWithHeaders:
@param {VICall*} call Call that triggered the event
@param {NSDictionary*} headers Optional headers passed with event
*/
- (void)call:(VICall*)call didConnectWithHeaders:(NSDictionary *)headers;

/**
Triggered after the call was disconnected.
@method call:didDisconnectWithHeaders:answeredElsewhere:
@param {VICall*} call Call that triggered the event
@param {NSDictionary*} headers Optional headers passed with event
@param {NSNumber*} answeredElsewhere true if call was answered on another device
*/
- (void)call:(VICall*)call didDisconnectWithHeaders:(NSDictionary *)headers answeredElsewhere:(NSNumber*)answeredElsewhere;

/**
Triggered if the call is ringing. You should start playing call progress tone now.
@method call:startRingingWithHeaders:
@param {VICall*} call Call that triggered the event
@param {NSDictionary*} headers Optional headers passed with event
*/
- (void)call:(VICall *)call startRingingWithHeaders:(NSDictionary *)headers;

/**
Triggered after audio is started in the call. You should stop playing progress tone when event is received
@method callDidStartAudio:
@param {VICall*} call Call that triggered the event
*/
- (void)callDidStartAudio:(VICall*)call;


/**
Triggered when message is received within the call. Implemented atop SIP INFO for communication between call endpoint and Voximplant cloud, and is separated from Voximplant messaging API.
@method call:didReceiveMessage:headers:
@param {VICall*} call Call that triggered the event
@param {NSString*} message Content of the message
@param {NSDictionary*} headers Optional headers passed with event
*/
- (void)call:(VICall*)call didReceiveMessage:(NSString*)message headers:(NSDictionary*)headers;

/**
Triggered when INFO message is received within the call.
@method call:withType:didReceiveInfo:type:headers:
@param {VICall*} call Call that triggered the event
@param {NSString*} body Body of INFO message
@param {NSString*} type MIME type of INFO message
@param {NSDictionary*} headers Optional headers passed with event
*/
- (void)call:(VICall*)call didReceiveInfo:(NSString*)body type:(NSString*)type headers:(NSDictionary*)headers;

/**
Triggered when call statistics are available for the call.
@method call:didReceiveStatistics:
@param {VICall*} call Call that triggered the event
@param {VICallStat*} stat Call statistics
*/
- (void)call:(VICall *)call didReceiveStatistics:(VICallStat*)stat;

/**
Triggered when local video stream is added to the call. The event is triggered on the main thread.
@method call:didAddLocalVideoStream:
@param {VICall*} call Call that triggered the event
@param {VIVideoStream*} videoStream Local video stream that is added to the call
*/
- (void)call:(VICall *)call didAddLocalVideoStream:(VIVideoStream*)videoStream;

/**
Triggered when local video stream is removed from the call. The event is triggered on the main thread.
@method call:didRemoveLocalVideoStream:
@param {VICall*} call Call that triggered the event
@param {VIVideoStream*} videoStream Local video stream that is removed to the call
*/
- (void)call:(VICall *)call didRemoveLocalVideoStream:(VIVideoStream*)videoStream;

@end



typedef void (^VICompletionBlock)(NSError* error); // error == nil -> means success

@protocol RTCVideoRenderer;
@class UIView;
@class VIVideoSource;

/**
@class VICall
*/
@interface VICall : NSObject

- (instancetype)init NS_UNAVAILABLE;

/**
 Sets preferred video codec. Nil by default. 
 Must be set before startWithVideo:headers: if needed
 @param {NSString*} preferredVideoCodec for example: preferredVideoCodec = @"H264"
 */
@property(nonatomic,strong) NSString* preferredVideoCodec;

/**
 Sets video source.
 Must be set before startWithVideo:headers: if needed
 @param {NSString*} videoSource, By default videoSource = [VICameraManager sharedCameraManager] (gets video from back or front camera)
 */
@property(nonatomic,strong) VIVideoSource* videoSource;

/**
Add call delegate to handle call events.
@method addDelegate:
@param {id<VICallDelegate>} delegate Call delegate
*/
- (void)addDelegate:(id<VICallDelegate>)delegate;

/**
Remove previously added call delegate.
@method removeDelegate:
@param {id<VICallDelegate>} delegate Call delegate
*/
- (void)removeDelegate:(id<VICallDelegate>)delegate;

/**
The call id.
@property callId
@type {NSString*}
@readonly
*/
@property(nonatomic,strong,readonly) NSString* callId;

/**
Array of the endpoints associated with the call.
@property endPoints
@type {NSArray<VIEndPoint*>*}
@readonly
*/
@property(nonatomic,strong,readonly) NSArray<VIEndPoint*>* endPoints;

/**
Call statistics. Updated every 5 seconds.
@property stat
@type {VICallStat*}
@readonly
*/
@property(nonatomic,strong,readonly) VICallStat* stat;

/**
Enables or disables audio transfer from microphone into the call.
@property sendAudio
@type {BOOL}
*/
@property(nonatomic,assign) BOOL sendAudio;

/**
Get the call duration
@method duration
@return {NSTimeInterval} Call duration
*/
- (NSTimeInterval)duration;

/**
Start outgoing or answers incoming alerting call
@method startWithVideo:headers:
@param {BOOL} video True for video call, false for audio call
@param {NSDictionary*} headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
*/
- (void)startWithVideo:(BOOL)video headers:(NSDictionary*)headers;

/**
Terminate established or outgoing processing call, or reject incoming processing call.
@method stopWithHeaders:headers:
@param {NSDictionary*} headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
*/
- (void)stopWithHeaders:(NSDictionary*)headers;

/**
Start or stop sending video for the call.
@method setSendVideo:completion:
@param {BOOL} video  True if video should be sent, false otherwise
@param {VICompletionBlock} completion Completion block to handle the result of the operation
*/
- (void)setSendVideo:(BOOL)video completion:(VICompletionBlock)completion;

/**
Hold or unhold the call
@method setHold:completion:
@param {BOOL} hold True if the call should be put on hold, false for unhold
@param {VICompletionBlock} completion Completion block to handle the result of the operation
*/
- (void)setHold:(BOOL)hold completion:(VICompletionBlock)completion;

/**
Send message within the call. Implemented atop SIP INFO for communication between call endpoint and Voximplant cloud, and is separated from Voximplant messaging API.
@method sendMessage:headers:
@param {NSString*} message Message text
@param {NSDictionary*} headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
*/
- (void)sendMessage:(NSString*)message headers:(NSDictionary*)headers;

/**
Send INFO message within the call
@method sendInfo:mimeType:headers:
@param {NSString*} body Custom string data
@param {NSString*} mimeType MIME type of info
@param {NSDictionary*} headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
*/
- (void)sendInfo:(NSString*)body mimeType:(NSString*)mimeType headers:(NSDictionary*)headers;

/**
Send DTMF within the call
@method sendDTMF:
@param {NSString*} dtmf DTMFs
@return {BOOL} True if DTMFs are sent successfully, false otherwise
*/
- (BOOL)sendDTMF:(NSString*)dtmf;

@end

@interface VICall(Convinence)

/**
Answer incoming call. Recommend to use startWithVideo:headers instead.
@method answerWithVideo:headers:
@param {BOOL} video True to answer with audio and video, false to answer with audio only
@param {NSDictionary*} headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
*/
- (void)answerWithVideo:(BOOL)video headers:(NSDictionary*)headers;

/**
Reject incoming call. Recommend to use startWithVideo:headers instead.
@method rejectWithHeaders:headers:
@param {NSDictionary*} headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
*/
- (void)rejectWithHeaders:(NSDictionary*)headers;

/**
Terminates call. Call should be either established, or outgoing progressing
@method hangupWithHeaders:headers:
@param {NSDictionary*} headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
*/
- (void)hangupWithHeaders:(NSDictionary*)headers;

@end


@interface VICall(Streams)

/**
Local video streams associated with the call
@property localVideoStreams
@type {NSArray<VIVideoStream*>*}
@readonly
*/
@property (nonatomic, strong, readonly) NSArray<VIVideoStream*>* localVideoStreams;

@end
