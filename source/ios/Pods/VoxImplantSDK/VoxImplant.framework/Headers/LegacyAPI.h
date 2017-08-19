//
//  Legacy.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 12.04.17.
//  Copyright © 2017 VoxImplant (www.voximplant.com). All rights reserved.
//

#import <AVFoundation/AVFoundation.h>

/**
 Enum of supported video resize modes
 
 @warning Deprecated.
 */
enum VoxImplantVideoResizeMode
{
    /** Clip video to fill the whole container */
    VI_VIDEO_RESIZE_MODE_CLIP,
    /** Shrink video to fit in container */
    VI_VIDEO_RESIZE_MODE_FIT
};

/**
 Enum of log level
 
 @warning Deprecated.
 */
enum VoxImplantLogLevel
{
    /** Log verbosity level, to include only error messages. */
    ERROR_LOG_LEVEL,
    /** Default log verbosity level, to include informational messages. */
    INFO_LOG_LEVEL,
    /** Log verbosity level to include debug messages */
    DEBUG_LOG_LEVEL,
    /** Log verbosity level to include trace messages */
    TRACE_LOG_LEVEL
};

@protocol VoxImplantDelegate;

/**
 Main VoxImplant SDK class. Should not be instantiated directly, use <-getInstance> instead.
 
 @warning Deprecated. Use <VIClient>, <VICall> instead.
 */
@interface VoxImplant: NSObject

/**
 Set delegate object for SDK
 @warning Deprecated. Use <VIClientSessionDelegate> delegate to handle connection to VoxImplant Cloud events, <VICallDelegate> delegate to handle call events and <VIClientCallManagerDelegate> delegate to handle incoming calls.
 */
@property (nonatomic,weak) id<VoxImplantDelegate> voxDelegate;

/**
 Returns current delegate
 
 @return  Previously set <VoxImplantDelegate> delegate
 @warning Deprecated. Use <[VIClient sessionDelegate]> to get connection delegate, <[VIClient callManagerDelegate]> to get incoming call delegate instead.
 */
-(id<VoxImplantDelegate>)getVoxDelegate;

/**
 Sets a verbosity level for log messages. Note that this method must be called before creating SDK object instance.
 
 @param logLevel Log verbosity level
 @warning        Deprecated. Use <+[VIClient setLogLevel:]> instead.
 */
+(void) setLogLevel: (enum VoxImplantLogLevel) logLevel;

/**
 Enables save logs to file. Log files located at: Library/Caches/Logs. Note that this method must be called before creating SDK object instance.
 
 @warning Deprecated. Use <+[VIClient saveLogToFileEnable]> instead.
 */
+(void) saveLogToFileEnable;

/**
 Returns single SDK object instance
 
 @return Single <VoxImplant> SDK object instance.
 @warning Deprecated. Use <[VIClient initWithDelegateQueue:]> instead.
 */
+(VoxImplant*) getInstance;

///----------------------------------
/// @name Connect to VoxImplant Cloud
///----------------------------------

/**
 Connect to VoxImplant cloud
 
 @warning Deprecated. Use <[VIClient connect]> intead.
 */
-(void) connect;

/**
 Connect to VoxImplant cloud
 
 @param connectivityCheck Checks whether UDP traffic will flow correctly between device and VoxImplant cloud. This check reduces connection speed.
 @warning                 Deprecated. Use <[VIClient connectWithConnectivityCheck:gateways:]> instead.
 */
-(void) connect: (bool)connectivityCheck;

/**
 Disable TLS encryption for signalling connection. Media data will be encrypted anyway
 
 @warning Deprecated.
 */
-(void) disableTLS;

/**
 Closes connection with media server
 
 @warning Deprecated. Use <[VIClient disconnect]> instead.
 */
-(void)closeConnection;

///----------------------------------
/// @name Login to VoxImplant Cloud
///----------------------------------

/**
 Login to VoxImplant using specified username and password
 
 @param user     Username combined with application name, for example _testuser@testapp.testaccount.voximplant.com_
 @param password Password in plain text
 @warning        Deprecated. Use <[VIClient loginWithUser:password:success:failure:]> instead.
 */
-(void)loginWithUsername: (NSString *)user andPassword:(NSString*) password;

/**
 Perform login using one time key that was generated before
 
 @param user Full user name, including app and account name, like _someuser@someapp.youraccount.voximplant.com_
 @param hash Hash that was generated using following formula:
 
     MD5(oneTimeKey+"|"+MD5(user+":voximplant.com:"+password))
 
 Please note that here user is just a user name, without app name,
 account name or anything else after "@". So if you pass _myuser@myapp.myacc.voximplant.com_ as a *username*,
 you should only use _myuser_ while computing this hash.
 
 @warning    Deprecated. Use <[VIClient loginWithUser:oneTimeKey:success:failure:]> instead.
 */
-(void)loginWithUsername: (NSString *)user andOneTimeKey:(NSString*) hash;

/**
 Login to VoxImplant using specified username and password
 
 @param user  Username combined with application name, for example testuser\@testapp.testaccount.voximplant.com
 @param token Access token, received in callback: <-[VoxImplantDelegate onLoginSuccessfulWithDisplayName:andAuthParams:]>
 @warning     Deprecated. Use <[VIClient loginWithUser:token:success:failure:]> instead.
 */
-(void)loginWithUsername: (NSString *)user andToken:(NSString*) token;

/**
 Perform refresh of login tokens required for login using access token
 
 @param user  Username combined with application name, for example testuser@testapp.testaccount.voximplant.com
 @param token Refresh token
 @warning     Deprecated. Use <[VIClient refreshTokenWithUser:token:result:]> instead.
 */
-(void)refreshTokenWithUsername: (NSString *)user andToken:(NSString*) token;

/**
 Generates one time login key to be used for automated login process.
 
 @param user Full user name, including app and account name, like _someuser@someapp.youraccount.voximplant.com_
 @see [Information about automated login on VoxImplant website](http://voximplant.com/docs/quickstart/24/automated-login/)
 @see -loginWithUsername:andOneTimeKey:
 @warning    Deprecated. Use <[VIClient requestOneTimeKeyWithUser:result:]> instead.
 */
-(void)requestOneTimeKeyWithUsername: (NSString *)user;

///----------------------------------
/// @name Call Management
///----------------------------------

/**
 Create new call instance. Call must be then started using <-startCall:withHeaders:> method.
 
 @param to         SIP URI, username or phone number to make call to. Actual routing is then performed by VoxEngine scenario
 @param video      Enable video support in call
 @param customData Custom data passed with call. Will be available in VoxEngine senario
 @return           Pointer to a string representation of the call id
 @warning          Deprecated. Use <[VIClient callToUser:customData:]> instead.
 */
-(NSString *)createCall: (NSString *) to withVideo: (bool) video  andCustomData: (NSString *) customData;

/**
 Send start call request.
 
 @param callId  id of previously created call
 @param headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
 @return        True in case of success, false otherwise (f.ex. if call with specified id is not found)
 @warning       Deprecated. Use <[VICall startWithHeaders:]> instead.
 */
-(bool)startCall: (NSString *) callId withHeaders: (NSDictionary*) headers;

/**
 Attach audio and video to specified call
 
 @param callId id of previously created call
 @return       True in case of success, false otherwise (f.ex. if call with specified id is not found)
 @warning      Deprecated.
 */
-(bool)attachAudioTo: (NSString *) callId;

/**
 Sends DTMF digit in specified call.
 
 @param callId id of previously created call
 @param digit  Digit can be 0-9 for 0-9, 10 for * and 11 for #
 @warning      Deprecated. Use <[VICall sendDTMF:]> instead.
 */
-(void)sendDTMF: (NSString *) callId digit: (int) digit;

/**
 Terminates specified call. Call must be either established, or outgoing progressing
 
 @param callId  id of previously created call
 @param headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
 @return        True in case of success, false otherwise (f.ex. if call with specified id is not found)
 @warning       Deprecated. Use <[VICall hangupWithHeaders:]> instead.
 */
-(bool)disconnectCall: (NSString *) callId withHeaders: (NSDictionary*) headers;

/**
 Rejects incoming alerting call
 
 @param callId  id of previously created call
 @param headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
 @warning       Deprecated. Use <[VICall rejectWithHeaders:]> instead.
 */
-(void)declineCall: (NSString *) callId withHeaders: (NSDictionary*) headers;

/**
 Answers incoming alerting call
 
 @param callId     id of previously created call
 @param customData Optinal custom data passed with call. Will be available in VoxEngine scenario
 @param headers    Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
 @warning          Deprecated. Use <[VICall answerWithVideo:customData:headers:]> instead.
 */
-(void)answerCall:(NSString*)callId withCustomData:(NSString*)customData headers: (NSDictionary*)headers;

/**
 Sends instant message within established call
 
 @param callId  id of previously created call
 @param text    Message text
 @param headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
 @warning       Deprecated. Use <[VICall sendMessage:headers:]> instead.
 */
-(void)sendMessage: (NSString *) callId withText: (NSString *) text andHeaders: (NSDictionary *) headers;

/**
 Sends info within established call
 
 @param callId   id of previously created call
 @param mimeType MIME type of info
 @param content  Custom string data
 @param headers  Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
 @warning        Deprecated. Use <[VICall sendInfo:mimeType:headers:]> instead.
 */
-(void)sendInfo: (NSString *) callId withType: (NSString *)mimeType content: (NSString *) content andHeaders: (NSDictionary *) headers;

/**
 Get call duration for established call
 
 @param callId id of previously created call
 @return       Call duration. 0 if call already disconnected.
 @warning      Deprecated. Use <[VICall duration]> instead.
 */
-(NSTimeInterval)getCallDuration: (NSString *) callId;

/**
 Mute or unmute microphone. This is reset after audio interruption
 
 @param b Enable/disable flag
 @warning Deprecated. Use <[VICall sendAudio]> instead.
 */
-(void)setMute: (bool) b;

/**
 Enable/disable loudspeaker (doesn't make sence for iPad, since it has only loudspeaker)
 
 @param b Enable/disable flag
 @warning Deprecated. Use <[VIAudioManager useLoudSpeaker]> instead.
 */
-(bool)setUseLoudspeaker: (bool) b;

/**
 Set video display mode. Applies to both incoming and outgoing stream
 
 @param mode Resize mode
 @warning    Deprecated. Use <[VIVideoRendererView resizeMode]> instead.
 */
-(void)setVideoResizeMode:(enum VoxImplantVideoResizeMode) mode;

/**
 Get video display mode. Applies to both incoming and outgoing streams
 
 @return  Resize mode
 @warning Deprecated.
 */
-(enum VoxImplantVideoResizeMode)getVideoResizeMode;

/**
 Stop/start sending video during the call
 
 @param doSend Specify if video should be sent
 @warning      Deprecated.
 */
-(void)sendVideo:(BOOL)doSend;

/**
 Set container for local video preview
 
 @param view UIView
 @warning    Deprecated.
 */
-(void)setLocalPreview: (UIView *)view;

/**
 Set container for remote video display
 
 @param view UIView
 @warning    Deprecated.
 */
-(void)setRemoteView: (UIView *) view;

/**
 Set container for remote video display for call
 
 @param view   UIView
 @param callId id of the call
 @warning      Deprecated.
 */
-(void)setRemoteView: (UIView *) view forCall: (NSString*) callId;

/**
 Set resolution of video being sent to remote participant
 
 @param width  Camera resolution width
 @param height Camera resolution height
 @warning      Deprecated.
 */
-(void)setResolution: (int) width andHeight: (int) height;

/**
 Connect to particular VoxImplant media gateway
 
 @param host Server name of particular media gateway for connection
 @warning    Deprecated.
 */
-(void)connectTo:(NSString *)host;

/**
 Check if device has front facing camera
 
 @return  True if device has front acing camera, false otherwise
 @warning Deprecated.
 */
-(BOOL) hasFrontFacingCamera;

/**
 Switch to front/back camera
 
 @param position Capture device position
 @return         True in case of success, false otherwise
 @warning        Deprecated.
 */
-(BOOL) switchToCameraWithPosition: (AVCaptureDevicePosition) position;

@end

@interface VoxImplant (PushNotifications)

/**
 Register Apple Push Notifications token,
 after calling this function, application will receive push notifications from Voximplant Server
 
 @param token The APNS token which comes from:
 - (void)pushRegistry:(PKPushRegistry *)registry didUpdatePushCredentials:(PKPushCredentials *)credentials forType:(PKPushType)type;
 @warning     Deprecated.
 */
- (void)registerPushNotificationsToken:(NSData *)token;

/**
 Unregister Apple Push Notifications token,
 after calling this function, application stops receive push notifications from Voximplant Server
 
 @param token The APNS token which comes from:
 - (void)pushRegistry:(PKPushRegistry *)registry didUpdatePushCredentials:(PKPushCredentials *)credentials forType:(PKPushType)type;
 credentials.token
 @warning     Deprecated.
 */
- (void)unregisterPushNotificationsToken:(NSData *)token;

/**
 Handle incoming push notification
 
 @param  notification The incomming notification which comes from:
 - (void)pushRegistry:(PKPushRegistry *)registry didReceiveIncomingPushWithPayload:(PKPushPayload *)payload forType:(PKPushType)type { ... }
 @warning             Deprecated.
 */
- (void)handlePushNotification:(NSDictionary *)notification;

- (void)setHold:(BOOL)hold forCall:(NSString*)callId;
@end
