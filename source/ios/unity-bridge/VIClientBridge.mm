#import <VoxImplant/VoxImplant.h>

#import "VIJsonDic.h"
#import "VIEAGLRenderer.h"
#import "VIClientBridge.h"
#import "iOSNativeRenderer.h"
#import "VIMetalRenderer.h"

extern "C" void UnitySendMessage(const char *obj, const char *method, const char *msg);

static NSString *s_unityObjName;

void CallUnityMethodWithString(NSString *methodName, NSString *parameters) {
    parameters = parameters ?: @"";
    UnitySendMessage(s_unityObjName.UTF8String, methodName.UTF8String, parameters.UTF8String);
}

void CallUnityMethod(NSString *methodName, id parameters) {
    NSError *writeError;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:parameters
                                                       options:0
                                                         error:&writeError];
    NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];

    if (writeError != nil) {
        NSLog(@"Failed to call method %@ with parameter %@", methodName, parameters);
        return;
    }

    CallUnityMethodWithString(methodName, jsonString);
}

@interface VIClientBridge() <VIClientSessionDelegate, VICallDelegate, VIClientCallManagerDelegate, VIEndPointDelegate>

@property(nonatomic, strong, readwrite) VIClient *client;
@property(nonatomic, strong, readwrite) UIAlertView *currentAlertView;

@property(nonatomic, strong, readonly) NSMutableDictionary<NSString *, id<RTCVideoRenderer>> *localRenderers;
@property(nonatomic, strong, readonly) NSMutableDictionary<NSString *, id<RTCVideoRenderer>> *remoteRenderers;

@end

@implementation VIClientBridge {
}

- (instancetype)init {
    self = [super init];
    if (self) {
        _client = [[VIClient alloc] initWithDelegateQueue:dispatch_get_main_queue()];
        self.client.sessionDelegate = self;

        _localRenderers = [NSMutableDictionary new];
        _remoteRenderers = [NSMutableDictionary new];
    }

    return self;
}

#pragma mark - VIClientSessionDelegate

- (void)clientSessionDidConnect:(VIClient *)client {
    [self callMethod:@"fiosonConnectionSuccessful"];
}

- (void)clientSessionDidDisconnect:(VIClient *)client {
    [self callMethod:@"fiosonConnectionClosed"];
}

- (void)client:(VIClient *)client sessionDidFailConnectWithError:(NSError *)error {
    [self callMethod:@"fiosonConnectionFailedWithError" withJSONParameter:@[[error localizedDescription]]];
}

#pragma mark - VIClientCallManagerDelegate

- (void)        client:(VIClient *)client
didReceiveIncomingCall:(VICall *)call
                 video:(BOOL)video
               headers:(NSDictionary *)headers {
    VIEndPoint *otherParty = [[call endPoints] firstObject];
    [call addDelegate:self];
    call.endPoints.firstObject.delegate = self;

    [self callMethod:@"fiosonIncomingCall" withJSONParameter:@[
            call.callId,
            otherParty.user,
            otherParty.userDisplayName,
            video ? @"true" : @"false",
            headers]];
}


#pragma mark - VICallDelegate

- (void)call:(VICall *)call didFailWithError:(NSError *)error headers:(NSDictionary *)headers {
    [self callMethod:@"fiosonCallFailed" withJSONParameter:@[call.callId, [error localizedDescription], headers]];
}

- (void)call:(VICall *)call didConnectWithHeaders:(NSDictionary *)headers {
    [self callMethod:@"fiosonCallConnected" withJSONParameter:@[call.callId, headers]];
}

- (void)            call:(VICall *)call
didDisconnectWithHeaders:(NSDictionary *)headers
       answeredElsewhere:(NSNumber *)answeredElsewhere {
    if (_currentAlertView != Nil) {
        UIAlertView *v = _currentAlertView;
        _currentAlertView = Nil;
        [v dismissWithClickedButtonIndex:0 animated:false];
    }

    [self callMethod:@"fiosonCallDisconnected" withJSONParameter:@[call.callId, headers]];
}

- (void)call:(VICall *)call startRingingWithHeaders:(NSDictionary *)headers {
    [self callMethod:@"fiosonCallRinging" withJSONParameter:@[call.callId, headers]];
}

- (void)callDidStartAudio:(VICall *)call {
    [self callMethod:@"fiosonCallAudioStarted" withJSONParameter:@[call.callId]];
}

- (void)call:(VICall *)call didReceiveMessage:(NSString *)message headers:(NSDictionary *)headers {
    [self callMethod:@"fiosonMessageReceivedInCall" withJSONParameter:@[call.callId, message, headers]];
}

- (void)call:(VICall *)call didReceiveInfo:(NSString *)body type:(NSString *)type headers:(NSDictionary *)headers {
    [self callMethod:@"fiosonSIPInfoReceivedInCall" withJSONParameter:@[call.callId, type, body, headers]];
}

- (void)call:(VICall *)call didReceiveStatistics:(VICallStat *)stat {
    //TODO: change API to report packet loss
    [self callMethod:@"fiosonNetStatsReceived" withJSONParameter:@[call.callId, @(0)]];
}

- (void)call:(VICall *)call didAddLocalVideoStream:(VIVideoStream *)videoStream {
    id<RTCVideoRenderer> renderer = self.localRenderers[call.callId];
    if (renderer != nil) {
        [videoStream addRenderer:renderer];
    }
}

- (void)call:(VICall *)call didRemoveLocalVideoStream:(VIVideoStream *)videoStream {
    [self.localRenderers removeObjectForKey:call.callId];
}

#pragma mark - VIEndPointDelegate

- (void)endPoint:(VIEndPoint *)endPoint didAddRemoteVideoStream:(VIVideoStream *)videoStream {
    id<RTCVideoRenderer> renderer = self.remoteRenderers[endPoint.call.callId];
    if (renderer != nil) {
        [videoStream addRenderer:renderer];
    }
}

- (void)endPoint:(VIEndPoint *)endPoint didRemoveRemoteVideoStream:(VIVideoStream *)videoStream {
    [self.remoteRenderers removeObjectForKey:endPoint.call.callId];
}

#pragma mark - Utils

- (void)callMethod:(NSString*) methodName {
    CallUnityMethodWithString(methodName, nil);
}

- (void)callMethod:(NSString *)string withJSONParameter:(NSArray *)parameter {
    CallUnityMethod(string, parameter);
}

#pragma mark - Renderers

- (void)addRemoteRenderer:(id <RTCVideoRenderer>)renderer toCall:(VICall *)call {
    self.remoteRenderers[call.callId] = renderer;

    [call.endPoints.firstObject.remoteVideoStreams.firstObject addRenderer:renderer];
}

- (void)addLocalRenderer:(id <RTCVideoRenderer>)renderer toCall:(VICall *)call {
    self.localRenderers[call.callId] = renderer;

    [call.localVideoStreams.firstObject addRenderer:renderer];
}

- (void)removeRemoteRendererFromCall:(VICall *)call {
    for (VIEndPoint *endPoint in call.endPoints) {
        for (VIVideoStream *stream in endPoint.remoteVideoStreams) {
            [stream removeAllRenderers];
        }
    }

    [_remoteRenderers removeObjectForKey:call.callId];
}

- (void)removeLocalRendererFromCall:(VICall *)call {
    for (VIVideoStream *stream in call.localVideoStreams) {
        [stream removeAllRenderers];
    }

    [_localRenderers removeObjectForKey:call.callId];
}

@end

VIClientBridge *s_bridge;

//C-wrapper that Unity communicates with
extern "C"
{

__used void iosSDKinit(const char *pUnityObjName) {
    s_bridge = [VIClientBridge new];
    s_unityObjName = [[NSString alloc] initWithUTF8String:pUnityObjName];
}

__used void iosSDKconnect() {
    [s_bridge.client connect];
}

__used void iosSDKlogin(const char *pLogin, const char *pPass) {
    NSString *user = [NSString stringWithUTF8String:pLogin];
    NSString *password = [NSString stringWithUTF8String:pPass];
    [s_bridge.client loginWithUser:user
                          password:password
                           success:^(NSString *displayName, NSDictionary *authParams) {
                               displayName = displayName ?: @"";
                               //TODO: authParams
                               CallUnityMethod(@"fiosonLoginSuccessful", @[displayName]);
                           }
                           failure:^(NSError *error) {
                               //TODO: properly bridge failure reason
                               CallUnityMethod(@"fiosonLoginFailed", @[[error localizedDescription]]);
                           }];
}

__used void iosSDKloginUsingOneTimeKey(const char *pUserName, const char *pOneTimeKey) {
    [s_bridge.client loginWithUser:[NSString stringWithUTF8String:pUserName]
                        oneTimeKey:[NSString stringWithUTF8String:pOneTimeKey]
                           success:^(NSString *displayName, NSDictionary *authParams) {
                               displayName = displayName ?: @"";
                               //TODO: authParams
                               CallUnityMethod(@"fiosonLoginSuccessful", @[displayName]);
                           }
                           failure:^(NSError *error) {
                               //TODO: properly bridge failure reason
                               CallUnityMethod(@"fiosonLoginFailed", @[[error localizedDescription]]);
                           }];
}

__used void iosSDKstartCall(const char *pUserId, bool pWithVideo, const char *pCustom, const char *pHeaders) {
    NSString *userString = [NSString stringWithUTF8String:pUserId];
    NSString *customData = [[NSString alloc] initWithUTF8String:pCustom];
    NSDictionary *headers = [[VIJsonDic alloc] initWithJSONString:[[NSString alloc] initWithUTF8String:pHeaders]].dic;

    VICall *call = [s_bridge.client callToUser:userString customData:customData];
    [call addDelegate:s_bridge];
    call.endPoints.firstObject.delegate = s_bridge;
    [call startWithVideo:pWithVideo headers:headers];

    CallUnityMethodWithString(@"fiosonOnStartCall", call.callId);
}

__used void iosSDKanswerCall(const char *pCallId, const char *pHeaders) {
    if (pCallId == nil) {
        NSLog(@"Internal Inconsistency: trying to answer call with nil id");
        return;
    }

    NSDictionary *headers = [[VIJsonDic alloc] initWithJSONString:[[NSString alloc] initWithUTF8String:pHeaders]].dic;
    NSString *callIdString = [NSString stringWithUTF8String:pCallId];

    VICall *call = s_bridge.client.calls[callIdString];
    NSLog(@"Answering %@, with id: %@", call, callIdString);
    [call startWithVideo:YES headers:headers];
}

__used void iosSDKDecline(const char *pCallId, const char *pHeaders) {
    if (pCallId == nil) {
        NSLog(@"Internal Inconsistency: trying to decline call with nil id");
        return;
    }

    NSDictionary *headers = [[VIJsonDic alloc] initWithJSONString:[[NSString alloc] initWithUTF8String:pHeaders]].dic;
    NSString *callIdString = [NSString stringWithUTF8String:pCallId];

    [s_bridge.client.calls[callIdString] rejectWithHeaders:headers];
}

__used void iosSDKsetMute(bool pSetMute) {
    for (VICall *call in [s_bridge.client.calls allValues]) {
        call.sendAudio = pSetMute;
    }
}

__used void iosSDKsendVideo(bool pSendVideo) {
    for (VICall *call in [s_bridge.client.calls allValues]) {
        [call setSendVideo:pSendVideo
                completion:^(NSError *error) {
                    if (error != nil) {
                        NSLog(@"Failed to enable/disable video with error: %@", [error localizedDescription]);
                    }
                }];
    }
}

__used void iosSDKsetCamera(bool pFrontCam) {
    [VICameraManager sharedCameraManager].useBackCamera = !pFrontCam;
}

__used void iosSDKrequestOneTimeKey(const char *pUserName) {
    [s_bridge.client requestOneTimeKeyWithUser:[NSString stringWithUTF8String:pUserName]
                                        result:^(NSString *oneTimeKey) {

                                        }];
}

__used void iosSDKsendDTFM(const char *pCallId, const char *pDigit) {
    if (pCallId == nil) {
        NSLog(@"Internal Inconsistency: trying to send DTFM in call with nil id");
        return;
    }

    NSString *callIdString = [NSString stringWithUTF8String:pCallId];
    NSString *dtfmString = [NSString stringWithUTF8String:pDigit];

    [s_bridge.client.calls[callIdString] sendDTMF:dtfmString];
}

__used void iosSDKsendInfo(const char *pCallId, const char *pMimeType, const char *pContent, const char *pAniHeaders) {
    if (pCallId == nil) {
        NSLog(@"Internal Inconsistency: trying to send info in call with nil id");
        return;
    }

    NSString *callIdString = [NSString stringWithUTF8String:pCallId];
    NSString *mimeType = [NSString stringWithUTF8String:pMimeType];
    NSString *content = [NSString stringWithUTF8String:pContent];
    NSDictionary *headers = [[VIJsonDic alloc] initWithJSONString:[[NSString alloc] initWithUTF8String:pAniHeaders]].dic;

    [s_bridge.client.calls[callIdString] sendInfo:content mimeType:mimeType headers:headers];
}

__used void iosSDKsendMessage(const char *pCallId, const char *pMsg, const char *pAniHeaders) {
    if (pCallId == nil) {
        NSLog(@"Internal Inconsistency: trying to send info in call with nil id");
        return;
    }

    NSString *callIdString = [NSString stringWithUTF8String:pCallId];
    NSString *message = [NSString stringWithUTF8String:pMsg];
    NSDictionary *headers = [[VIJsonDic alloc] initWithJSONString:[[NSString alloc] initWithUTF8String:pAniHeaders]].dic;

    [s_bridge.client.calls[callIdString] sendMessage:message headers:headers];
}

__used void beginSendingVideoForStream(const char* pCallId, int stream) {
    if (pCallId == nil) {
        NSLog(@"Internal Inconsistency: trying to render video stream to texture in call with nil id");
        return;
    }

    NSString *callIdString = [NSString stringWithUTF8String:pCallId];
    VICall *call = s_bridge.client.calls[callIdString];

    id<RTCVideoRenderer> renderer;
    if (s_unityGFXRenderer == kUnityGfxRendererMetal) {
        renderer = [[VIMetalRenderer alloc] initWithStream:stream
                                         nativeTextureReport:^(id <MTLTexture> texture, void *pVoid, int width, int height) {
                                             CallUnityMethod(@"fonNewNativeTexture", @[callIdString, @((long long)texture), @((long long)pVoid), @(width), @(height), @(stream)]);
                                         }];
    } else {
        renderer = [[VIEAGLRenderer alloc] initWithStream:stream
                                        nativeTextureReport:^(GLuint textureId, void *context, int width, int height) {
                                            CallUnityMethod(@"fonNewNativeTexture", @[callIdString, @(textureId), @((long long)context), @(width), @(height), @(stream)]);
                                        }];
    }

    if (stream == 0) {
        [s_bridge removeRemoteRendererFromCall:call];
        [s_bridge addRemoteRenderer:renderer toCall:call];
    } else if (stream == 1) {
        [s_bridge removeLocalRendererFromCall:call];
        [s_bridge addLocalRenderer:renderer toCall:call];
    } else {
        NSLog(@"Internal Inconsistency: unknown stream");
    }
}

//__used void

__used void iosSDKsetUseLoudspeaker(bool pUseLoudspeaker) {
    [VIAudioManager sharedAudioManager].useLoudSpeaker = pUseLoudspeaker;
}

__used void iosSDKdisconnectCall(const char *pCallId, const char *pHeaders) {
    if (pCallId == nil) {
        NSLog(@"Internal Inconsistency: trying to hungup call with nil id");
        return;
    }

    NSDictionary *headers = [[VIJsonDic alloc] initWithJSONString:[[NSString alloc] initWithUTF8String:pHeaders]].dic;
    NSString *callIdString = [NSString stringWithUTF8String:pCallId];

    [s_bridge.client.calls[callIdString] hangupWithHeaders:headers];
}

__used void iosSDKCloseConnection() {
    [s_bridge.client disconnect];
}

}
