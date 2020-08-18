/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

#import "VIClientModule.h"
#import "VIEmitter.h"
#import "VICallModule.h"
#import "Version.h"
#import "VIUnityGraphicsBridge.h"

@interface VIClient (subVersion)

+ (void)setVersionExtension:(NSString *)versionExtension;

@end

@interface VIClientModule () <VIClientSessionDelegate, VIClientCallManagerDelegate>

@property(nonatomic, strong) VIClient *client;
@property(nonatomic, strong) VILoginSuccess loginSuccess;
@property(nonatomic, strong) VILoginFailure loginFailure;

@end

@implementation VIClientModule

+ (instancetype)sharedModule {
    static VIClientModule *instance;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        instance = [[VIClientModule alloc] init];
    });
    return instance;
}

- (void)initializeModule:(NSString *)clientConfigJson {
    NSDictionary *clientConfig = [NSJSONSerialization JSONObjectWithData:[clientConfigJson dataUsingEncoding:NSUTF8StringEncoding] options:0 error:nil];
    [VIClient setLogLevel:(VILogLevel) ((NSNumber *) clientConfig[@"logLevel"]).unsignedIntegerValue];

    [VIClient setVersionExtension:[NSString stringWithFormat:@"unity-%s", UNITY_BRIDGE_VERSION]];
    dispatch_queue_t delegateQueue = dispatch_queue_create("com.voximplant.unity", DISPATCH_QUEUE_SERIAL);

    _client = [[VIClient alloc] initWithDelegateQueue:delegateQueue];
    _client.sessionDelegate = self;
    _client.callManagerDelegate = self;

    _loginSuccess = ^(NSString *displayName, VIAuthParams *authParams) {
        [VIEmitter sendClientMessage:@"LoginSuccess" payload:@{
                @"displayName": displayName,
                @"authParams": @{
                        @"accessExpired": @(authParams.accessExpire),
                        @"accessToken": authParams.accessToken,
                        @"refreshExpired": @(authParams.refreshExpire),
                        @"refreshToken": authParams.refreshToken
                }
        }];
    };
    _loginFailure = ^(NSError *error) {
        [VIEmitter sendClientMessage:@"LoginFailed" payload:@{
                @"code": @(error.code),
                @"error": [VIClientModule localizedDescriptionForCode:(VILoginErrorCode) error.code
                                                          description:error.localizedDescription]
        }];
    };

    UnityRegisterRenderingPluginV5(&VoximplantPluginLoad, &VoximplantPluginUnload);
}

+ (NSString *)localizedDescriptionForCode:(VILoginErrorCode)code description:(NSString *)description{
    if (description && ![[description stringByTrimmingCharactersInSet:[NSCharacterSet whitespaceAndNewlineCharacterSet]] isEqualToString:@""])
        return description;
    switch (code) {
        case VILoginErrorCodeInvalidPassword:
            return @"Invalid login or password";
        case VILoginErrorCodeMAUAccessDenied:
            return @"Monthly Active Users (MAU) limit is reached. Payment is required.";
        case VILoginErrorCodeAccountFrozen:
            return @"Account frozen";
        case VILoginErrorCodeInvalidUsername:
            return @"Invalid username";
        case VILoginErrorCodeTimeout:
            return @"Timeout";
        case VILoginErrorCodeConnectionClosed:
            return @"Connection to the Voximplant Cloud is closed as a result of disconnect method call.";
        case VILoginErrorCodeInvalidState:
            return @"Invalid state";
        case VILoginErrorCodeNetworkIssues:
            return @"Network issues";
        case VILoginErrorCodeTokenExpired:
            return @"Token expired";
        case VILoginErrorCodeInternalError:
        default:
            return @"Internal error";
    }
}

#pragma mark - VIClient

- (void)connect:(BOOL)connectivityCheck gateways:(NSArray<NSString *> *)gateways {
    [_client connectWithConnectivityCheck:connectivityCheck gateways:gateways];
}

- (void)login:(NSString *)user password:(NSString *)password {
    [_client loginWithUser:user password:password success:_loginSuccess failure:_loginFailure];
}

- (void)login:(NSString *)user token:(NSString *)token {
    [_client loginWithUser:user token:token success:_loginSuccess failure:_loginFailure];
}

- (void)login:(NSString *)user hash:(NSString *)hash {
    [_client loginWithUser:user oneTimeKey:hash success:_loginSuccess failure:_loginFailure];
}

- (void)refreshTokenWithUser:(NSString *)user token:(NSString *)token {
    [_client refreshTokenWithUser:user token:token result:^(VIAuthParams *authParams, NSError *error) {
        if (error) {
            [VIEmitter sendClientMessage:@"RefreshTokenFailed" payload:@{
                    @"code": @(error.code),
                    @"error": [VIClientModule localizedDescriptionForCode:(VILoginErrorCode) error.code
                                                              description:error.localizedDescription]
            }];
        } else {
            [VIEmitter sendClientMessage:@"RefreshTokenSuccess" payload:@{@"authParams": @{
                    @"accessExpired": @(authParams.accessExpire),
                    @"accessToken": authParams.accessToken,
                    @"refreshExpired": @(authParams.refreshExpire),
                    @"refreshToken": authParams.refreshToken
            }}];
        }
    }];
}

- (void)requestOneTimeKey:(NSString *)user {
    [_client requestOneTimeKeyWithUser:user result:^(NSString *oneTimeKey, NSError *error) {
        if (error) {
            [VIEmitter sendClientMessage:@"LoginFailed" payload:@{
                    @"code": @(error.code),
                    @"error": [VIClientModule localizedDescriptionForCode:(VILoginErrorCode) error.code
                                                              description:error.localizedDescription]
            }];
        } else {
            [VIEmitter sendClientMessage:@"OneTimeKeyGenerated" payload:@{@"oneTimeKey": oneTimeKey}];
        }
    }];
}

- (void)disconnect {
    [_client disconnect];
}

- (NSUInteger)state {
    return _client.clientState;
}

- (NSString *)call:(NSString *)username receiveVideo:(BOOL)receiveVideo sendVideo:(BOOL)sendVideo videoCodec:(NSInteger)videoCodec customData:(NSString *)customData headers:(NSString *)headers {
    VICallSettings *callSettings = [VICallSettings new];
    callSettings.videoFlags = [VIVideoFlags videoFlagsWithReceiveVideo:receiveVideo sendVideo:sendVideo];
    callSettings.preferredVideoCodec = (VIVideoCodec) videoCodec;
    callSettings.customData = customData;
    if (headers) {
        NSError *error;
        callSettings.extraHeaders = [NSJSONSerialization JSONObjectWithData:[headers dataUsingEncoding:NSUTF8StringEncoding] options:0 error:&error];
        if (error) {
            NSLog(@"Failed to process headers: %@", error.localizedDescription);
        }
    }

    VICall *call;
    if ((call = [_client call:username settings:callSettings])) {
        [[VICallModule sharedModule] addCall:call];
    }

    return call.callId;
}

- (NSString *)callConference:(NSString *)conference receiveVideo:(BOOL)receiveVideo sendVideo:(BOOL)sendVideo videoCodec:(NSInteger)videoCodec customData:(NSString *)customData headers:(NSString *)headers {
    VICallSettings *callSettings = [VICallSettings new];
    callSettings.videoFlags = [VIVideoFlags videoFlagsWithReceiveVideo:receiveVideo sendVideo:sendVideo];
    callSettings.preferredVideoCodec = (VIVideoCodec) videoCodec;
    callSettings.customData = customData;
    if (headers) {
        NSError *error;
        callSettings.extraHeaders = [NSJSONSerialization JSONObjectWithData:[headers dataUsingEncoding:NSUTF8StringEncoding] options:0 error:&error];
        if (error) {
            NSLog(@"Failed to process headers: %@", error.localizedDescription);
        }
    }

    VICall *call;
    if ((call = [_client callConference:conference settings:callSettings])) {
        [[VICallModule sharedModule] addCall:call];
    }

    return call.callId;
}

#pragma mark - VIClientSessionDelegate

- (void)clientSessionDidConnect:(VIClient *)client {
    [VIEmitter sendClientMessage:@"Connected"];
}

- (void)clientSessionDidDisconnect:(VIClient *)client {
    [VIEmitter sendClientMessage:@"Disconnected"];
}

- (void)client:(VIClient *)client sessionDidFailConnectWithError:(NSError *)error {
    [VIEmitter sendClientMessage:@"ConnectionFailed" payload:@{
            @"code": @(error.code),
            @"error": error.localizedDescription
    }];
}

#pragma mark - VIClientCallManagerDelegate

- (void)client:(VIClient *)client didReceiveIncomingCall:(VICall *)call withIncomingVideo:(BOOL)video headers:(nullable NSDictionary *)headers {
    [[VICallModule sharedModule] addCall:call];

    NSMutableDictionary *payload = [NSMutableDictionary dictionaryWithCapacity:3];
    payload[@"callId"] = call.callId;
    payload[@"incomingVideo"] = @(video);
    if (headers) {
        payload[@"headers"] = [VIEmitter flatten:headers];
    }

    [VIEmitter sendClientMessage:@"IncomingCall" payload:payload];
}


@end
