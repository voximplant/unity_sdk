//
//  VIClient.h
//  VoxSDK
//
//  Created by Andrey Syvrachev (asyvrachev@zingaya.com) on 07.12.16.
//  Copyright © 2017 VoxImplant (www.voximplant.com). All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreGraphics/CoreGraphics.h>

@class VIClient;
@class VICall;


/*!
 @enum VILogLevel
 @abstract Log level
 */
typedef NS_ENUM(NSUInteger, VILogLevel) {
    VILogLevelDisabled,
    VILogLevelError,
    VILogLevelWarning,
    VILogLevelInfo,
    VILogLevelDebug,
    VILogLevelVerbose,
    VILogLevelMax
};

/*!
 Main VoxImplant SDK class.
 @class VIClient
 */
@interface VIClient : NSObject

+ (NSString*)clientVersion;

/**
 Sets a verbosity level for log messages. Note that this method must be
 called before creating SDK object instance.
 @method setLogLevel:
 @static
 @param logLevel verbosity level
 */
+ (void)setLogLevel:(VILogLevel)logLevel;


/**
 Create VIClient instance
 @method initWithDelegateQueue:
 @param queue All delegates methods will be called in this queue
 Warning! Queue must be ONLY SERIAL! not concurrent!!! (main queue possible)
 */
- (instancetype)initWithDelegateQueue:(dispatch_queue_t)queue;

@end

@protocol VIClientSessionDelegate <NSObject>

/**
 Triggered if Voximplant cloud was connected
 @method clientSessionDidConnect:
 @param client client instance
 */
- (void)clientSessionDidConnect:(VIClient*)client;

/**
 Triggered if Voximplant cloud was disconnected
 @method clientSessionDidDisconnect:
 @param  client client instance
 */
- (void)clientSessionDidDisconnect:(VIClient*)client;

/**
 Triggered if Voximplant cloud was not successfully connected
 @method client:sessionDidFailConnectWithError:
 @param client client instance
 @param error error description
 */
- (void)client:(VIClient*)client sessionDidFailConnectWithError:(NSError*)error;

@end

@interface VIClient(Session)

@property (nonatomic, weak) id<VIClientSessionDelegate> sessionDelegate;

/**
 Connect to VoxImplant cloud
 @method connect
 */
- (void)connect;

/**
 Connect to VoxImplant cloud
 @method connectWithConnectivityCheck:gateways:
 @param connectivityCheck Checks whether UDP traffic will flow correctly between device and VoxImplant cloud. This check reduces connection speed.
 @param gateways error description
 */
- (void)connectWithConnectivityCheck:(BOOL)connectivityCheck gateways:(NSArray*)gateways;

/**
 Disconnect from VoxImplant cloud
 @method disconnect
 */
- (void)disconnect;

@end


/**
 Completion handler, called than logged in succefully
 @param displayName  DisplayName
 @param authParams   Auth params. Contains token strings and expire durations in seconds as <b>accessExpire</b>, <b>accessToken</b>, <b>refreshExpire</b> and <b>refreshToken</b>.
 */
typedef void (^VILoginSuccess)(NSString * displayName, NSDictionary* authParams);


/**
 Completion handler, called than error occurs during login
 @param  error  Error
 */
typedef void (^VILoginFailure)(NSError * error);

/**
 Completion handler, called than one time key generated
 @param oneTimeKey  oneTimeKey
 */
typedef void (^VIOneTimeKeyResult)(NSString * oneTimeKey);

/**
 Completion handler, called than logged in succefully
 @param error  may be nil if success
 @param authParams   Auth params. Contains token strings and expire durations in seconds as <b>accessExpire</b>, <b>accessToken</b>, <b>refreshExpire</b> and <b>refreshToken</b>.
 */
typedef void (^VIRefreshTokenResult)(NSError *error, NSDictionary* authParams);


@interface VIClient(Login)

/**
 Login to VoxImplant cloud
 @method loginWithUser:password:success:failure
 @param user       Full user name, including app and account name, like <i>someuser@someapp.youraccount.voximplant.com</i>
 @param password   Password
 @param success called than logged in succeffuly
 @param failure called than failed to login
 */
- (void)loginWithUser:(NSString*)user
             password:(NSString*)password
              success:(VILoginSuccess)success
              failure:(VILoginFailure)failure;

/**
 Login to VoxImplant cloud
 @method loginWithUser:token:success:failure
 @param  user  Full user name, including app and account name, like <i>someuser@someapp.youraccount.voximplant.com</i>
 @param  token Token
 @param  success called than logged in succeffuly
 @param  failure called than failed to login
 */
- (void)loginWithUser:(NSString*)user
                token:(NSString*)token
              success:(VILoginSuccess)success
              failure:(VILoginFailure)failure;

/**
 Login to VoxImplant cloud
 @method loginWithUser:token:success:failure:
 @param  user  Full user name, including app and account name, like <i>someuser@someapp.youraccount.voximplant.com</i>
 @param  oneTimeKey oneTimeKey
 @param  success called than logged in succeffuly
 @param  failure called than failed to login
 */
- (void)loginWithUser:(NSString*)user
           oneTimeKey:(NSString*)oneTimeKey
              success:(VILoginSuccess)success
              failure:(VILoginFailure)failure;

/**
 Perform refresh of login tokens required for login using access token
 @method refreshTokenWithUser:token:result
 @param  user Full user name, including app and account name, like <i>someuser@someapp.youraccount.voximplant.com</i>
 @param  token refresh token that was obtained in the <b>onLoginSuccessfulWithDisplayName</b> callback as the <b>refreshToken</b> property of callback parameter object.
 @param  result Completion handler.
 */
- (void)refreshTokenWithUser:(NSString *)user token:(NSString*)token result:(VIRefreshTokenResult)result;

/**
 Generates one time login key to be used for automated login process. See <a href="http://voximplant.com/docs/quickstart/24/automated-login/">Information about automated login on VoxImplant website</a> and <b>loginUsingOneTimeKey</b>
 @method requestOneTimeKeyWithUser:result:
 @param  user Full user name, including app and account name, like <i>someuser@someapp.youraccount.voximplant.com</i>
 @param  result Completion handler.
 */
- (void)requestOneTimeKeyWithUser:(NSString *)user result:(VIOneTimeKeyResult)result;

@end


@protocol VIClientCallManagerDelegate <NSObject>

/**
 Triggered on incoming call
 @method client:didReceiveIncomingCall:video:headers:
 @param  client VIClient instance
 @param  call Call instance
 @param  video incoming call offers video if true
 @param  headers Optional headers passed with event
 */
- (void)client:(VIClient*)client didReceiveIncomingCall:(VICall*)call video:(BOOL)video headers:(NSDictionary*)headers;

@end

@interface VIClient(CallManager)

@property (nonatomic, weak) id<VIClientCallManagerDelegate> callManagerDelegate;

@property (nonatomic,strong,readonly) NSDictionary<NSString*,VICall*>*  calls; // callId to call dictionary

- (VICall*)callToUser:(NSString*)user customData:(NSString*)customData;

@end

@interface VIClient(Push)

/** Register Apple Push Notifications token,
 after calling this function, application will receive push notifications from Voximplant Server
 @param token The APNS token which comes from:
 - (void)pushRegistry:(PKPushRegistry *)registry didUpdatePushCredentials:(PKPushCredentials *)credentials forType:(PKPushType)type;
 */
- (void)registerPushNotificationsToken:(NSData *)token;

/** Unregister Apple Push Notifications token,
 after calling this function, application stops receive push notifications from Voximplant Server
 @param token The APNS token which comes from:
 - (void)pushRegistry:(PKPushRegistry *)registry didUpdatePushCredentials:(PKPushCredentials *)credentials forType:(PKPushType)type;
 credentials.token
 */
- (void)unregisterPushNotificationsToken:(NSData *)token;

/** Handle incoming push notification
 @param notification The incomming notification which comes from:
 - (void)pushRegistry:(PKPushRegistry *)registry didReceiveIncomingPushWithPayload:(PKPushPayload *)payload forType:(PKPushType)type {
 payload.dictionaryPayload
 */
- (void)handlePushNotification:(NSDictionary *)notification;

@end


