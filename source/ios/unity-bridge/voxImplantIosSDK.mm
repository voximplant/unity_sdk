

#import "JsonDic.h"
#import "EAGLRendererHolder.h"
#import "voxImplantIosSDK.h"
#import "iOSNativeRenderer.h"
#import "MetalRendererHolder.h"

extern "C" void UnitySendMessage(const char *obj, const char *method, const char *msg);

static NSString *s_unityObjName;

void CallUnityMethodWithString(NSString *methodName, NSString *parameters) {
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

@implementation ProxyingVoxImplantDelegate {
    EAGLRendererHolder *_remoteHolder;
    EAGLRendererHolder *_localHolder;
    
@public
    UIAlertView *currentAlertView;
}

- (void)callMethod:(NSString*) methodName {
    [self callMethod:methodName withParameter:nil];
}

- (void)callMethod:(NSString *)methodName withParameter:(NSString *)parameter {
    parameter = parameter ?: @"";
    UnitySendMessage([s_unityObjName UTF8String], [methodName UTF8String], [parameter UTF8String]);
}

- (void)onConnectionSuccessful {
    [self callMethod:@"fiosonConnectionSuccessful"];
}

- (void)onConnectionFailedWithError:(NSString *)reason {
    [self callMethod:@"fiosonConnectionFailedWithError" withJSONParameter:@[reason]];
}

- (void)callMethod:(NSString *)string withJSONParameter:(NSArray *)parameter {
    CallUnityMethod(string, parameter);
}

- (void)onConnectionClosed {
    [self callMethod:@"fiosonConnectionClosed"];
}

- (void)onLoginFailedWithErrorCode:(NSNumber *)errorCode {
    [self callMethod:@"fiosonLoginFailed" withJSONParameter:@[errorCode]];
}

- (void)onLoginSuccessfulWithDisplayName:(NSString *)displayName andAuthParams:(NSDictionary *)authParams {
    displayName = displayName ?: @"";
    [self callMethod:@"fiosonLoginSuccessful" withJSONParameter:@[displayName]];
}

- (void)onCallRinging:(NSString *)callId withHeaders:(NSDictionary *)headers {
    [self callMethod:@"fiosonCallRinging" withJSONParameter:@[callId, headers]];
}

- (void)onCallConnected:(NSString *)callId withHeaders:(NSDictionary *)headers {
    [self.sdk attachAudioTo:callId];

    [self callMethod:@"fiosonCallConnected" withJSONParameter:@[callId, headers]];
}

- (void)onCallDisconnected:(NSString *)callId withHeaders:(NSDictionary *)headers {
    if (currentAlertView != Nil) {
        UIAlertView *v = currentAlertView;
        currentAlertView = Nil;
        [v dismissWithClickedButtonIndex:0 animated:false];
    }

    [self callMethod:@"fiosonCallDisconnected" withJSONParameter:@[callId, headers]];
}

- (void)onCallFailed:(NSString *)callId withCode:(int)code andReason:(NSString *)reason withHeaders:(NSDictionary *)headers {
    [self callMethod:@"fiosonCallFailed" withJSONParameter:@[callId, @(code), reason, headers]];
}

- (void)onIncomingCall:(NSString *)callId caller:(NSString *)from named:(NSString *)displayName withVideo:(bool)videoCall withHeaders:(NSDictionary *)headers {
    [self callMethod:@"fiosonIncomingCall" withJSONParameter:@[callId, from, displayName, videoCall ? @"true" : @"false", headers]];
}

- (void)onCallAudioStarted:(NSString *)callId {
    [self.sdk attachAudioTo:callId];

    [self callMethod:@"fiosonCallAudioStarted" withJSONParameter:@[callId]];
}

- (void)onNetStatsReceivedInCall:(NSString *)callId withStats:(const struct VoxImplantNetworkInfo *)stats {
    [self callMethod:@"fiosonNetStatsReceived"
   withJSONParameter:@[callId, @(stats->packetLoss)]];
}

- (void)onSIPInfoReceivedInCall:(NSString *)callId withType:(NSString *)type andContent:(NSString *)content withHeaders:(NSDictionary *)headers {
    [self callMethod:@"fiosonSIPInfoReceivedInCall" withJSONParameter:@[callId, type, content, headers]];
}

- (void)onMessageReceivedInCall:(NSString *)callId withText:(NSString *)text withHeaders:(NSDictionary *)headers {
    [self callMethod:@"fiosonMessageReceivedInCall" withJSONParameter:@[callId, text, headers]];
}

- (void)addLocalRendererHolder:(id<VXRendererHolder>)holder {
    [self.sdk addLocalRendererHolder:holder];
    _localHolder = holder;
}
- (void)removeLocalRendererHolder {
    if (_localHolder != nil) {
        [self.sdk removeLocalRendererHolder:_localHolder];
        _localHolder = nil;
    }
}

- (void)addRemoteRendererHolder:(id<VXRendererHolder>)holder {
    [self.sdk addRemoteRendererHolder:holder];
    _remoteHolder = holder;
}
- (void)removeRemoteRendererHolder {
    if (_remoteHolder != nil) {
        [self.sdk removeRemoteRendererHolder:_remoteHolder];
        _remoteHolder = nil;
    }
}

@end

ProxyingVoxImplantDelegate *singleton;

//C-wrapper that Unity communicates with
extern "C"
{

__used void iosSDKinit(const char *pUnityObjName) {
    singleton = [[ProxyingVoxImplantDelegate alloc] init];
    singleton.sdk = [VoxImplant getInstance];
    [singleton.sdk setVoxDelegate:singleton];
    s_unityObjName = [[NSString alloc] initWithUTF8String:pUnityObjName];
    [singleton.sdk setResolution:352 andHeight:288];
}

__used void iosSDKconnect() {
    [singleton.sdk connect];
}

__used void iosSDKlogin(const char *pLogin, const char *pPass) {
    [singleton.sdk loginWithUsername:[NSString stringWithUTF8String:pLogin] andPassword:[NSString stringWithUTF8String:pPass]];
}

__used void iosSDKstartCall(const char *pId, bool pWithVideo, const char *pCustom, const char *pHeaders) {
    NSString *activeCallId = [singleton.sdk createCall:[NSString stringWithUTF8String:pId]
                                              withVideo:pWithVideo
                                          andCustomData:[[NSString alloc] initWithUTF8String:pCustom]];
    [singleton.sdk attachAudioTo:activeCallId];

    if (pHeaders == nil)
        [singleton.sdk startCall:activeCallId withHeaders:nil];
    else {
        JsonDic *headers = [[JsonDic alloc] initWithJSONString:[[NSString alloc] initWithUTF8String:pHeaders]];
        [singleton.sdk startCall:activeCallId withHeaders:headers.dic];
    }

    CallUnityMethodWithString(@"fiosonOnStartCall", activeCallId);
}

__used void iosSDKanswerCall(const char *pCallId, const char *pHeaders) {
    if (pCallId != Nil) {
        if (pHeaders == Nil)
            [singleton.sdk answerCall:[NSString stringWithUTF8String:pCallId] withHeaders:Nil];
        else {
            JsonDic *headers = [[JsonDic alloc] initWithJSONString:[[NSString alloc] initWithUTF8String:pHeaders]];
            [singleton.sdk answerCall:[NSString stringWithUTF8String:pCallId] withHeaders:headers.dic];
        }
    }
}

__used void iosSDKHungup(const char *pCallId, const char *pHeaders) {
    if (pHeaders == Nil)
        [singleton.sdk disconnectCall:[NSString stringWithUTF8String:pCallId] withHeaders:Nil];
    else {
        JsonDic *headers = [[JsonDic alloc] initWithJSONString:[[NSString alloc] initWithUTF8String:pHeaders]];
        [singleton.sdk disconnectCall:[NSString stringWithUTF8String:pCallId] withHeaders:headers.dic];
    }
}

__used void iosSDKDecline(const char *pCallId, const char *pHeaders) {
    if (pHeaders == Nil)
        [singleton.sdk declineCall:[NSString stringWithUTF8String:pCallId] withHeaders:Nil];
    else {
        JsonDic *headers = [[JsonDic alloc] initWithJSONString:[[NSString alloc] initWithUTF8String:pHeaders]];
        [singleton.sdk declineCall:[NSString stringWithUTF8String:pCallId] withHeaders:headers.dic];
    }
}

__used void iosSDKsetMute(bool pSetMute) {
    [singleton.sdk setMute:pSetMute];
}

__used void iosSDKsendVideo(bool pSendVideo) {
    [singleton.sdk sendVideo:pSendVideo];
}

__used void iosSDKsetCamera(bool pFrontCam) {
    if (pFrontCam)
        [singleton.sdk switchToCameraWithPosition:AVCaptureDevicePositionFront];
    else
        [singleton.sdk switchToCameraWithPosition:AVCaptureDevicePositionBack];
}

__used void iosSDKdisableTls() {
    [singleton.sdk disableTLS];
}

__used void iosSDKdisconnectCall(const char *pCall, const char *pHeaders) {
    if (pHeaders == Nil)
        [singleton.sdk disconnectCall:[NSString stringWithUTF8String:pCall] withHeaders:Nil];
    else {
        JsonDic *headers = [[JsonDic alloc] initWithJSONString:[[NSString alloc] initWithUTF8String:pHeaders]];
        [singleton.sdk disconnectCall:[NSString stringWithUTF8String:pCall] withHeaders:headers.dic];
    }
}

__used void iosSDKloginUsingOneTimeKey(const char *pUserName, const char *pOneTimeKey) {
    [singleton.sdk loginWithUsername:[NSString stringWithUTF8String:pUserName] andOneTimeKey:[NSString stringWithUTF8String:pOneTimeKey]];
}

__used void iosSDKrequestOneTimeKey(const char *pUserName) {
    [singleton.sdk requestOneTimeKeyWithUsername:[NSString stringWithUTF8String:pUserName]];
}

__used void iosSDKsendDTFM(const char *pCallId, int pDigit) {
    [singleton.sdk sendDTMF:[NSString stringWithUTF8String:pCallId] digit:pDigit];
}

__used void iosSDKsendInfo(const char *pCallId, const char *pMimeType, const char *pContent, const char *pAniHeaders) {
    if (pAniHeaders == Nil)
        [singleton.sdk sendInfo:[NSString stringWithUTF8String:pCallId] withType:[NSString stringWithUTF8String:pMimeType] content:[NSString stringWithUTF8String:pContent] andHeaders:Nil];
    else {
        JsonDic *headers = [[JsonDic alloc] initWithJSONString:[[NSString alloc] initWithUTF8String:pAniHeaders]];
        [singleton.sdk sendInfo:[NSString stringWithUTF8String:pCallId] withType:[NSString stringWithUTF8String:pMimeType] content:[NSString stringWithUTF8String:pContent] andHeaders:headers.dic];
    }
}

__used void iosSDKsendMessage(const char *pCallId, const char *pMsg, const char *pAniHeaders) {
    if (pAniHeaders == Nil)
        [singleton.sdk sendMessage:[NSString stringWithUTF8String:pCallId] withText:[NSString stringWithUTF8String:pMsg] andHeaders:Nil];
    else {
        JsonDic *headers = [[JsonDic alloc] initWithJSONString:[[NSString alloc] initWithUTF8String:pAniHeaders]];
        [singleton.sdk sendMessage:[NSString stringWithUTF8String:pCallId] withText:[NSString stringWithUTF8String:pMsg] andHeaders:headers.dic];
    }
}

__used void iosSDKsetCameraResolution(int pWidth, int pHeight) {
    [singleton.sdk setResolution:pWidth andHeight:pHeight];
}

__used void iosSDKsetUseLoudspeaker(bool pUseLoudspeaker) {
    [singleton.sdk setUseLoudspeaker:pUseLoudspeaker];
}

__used void iosSDKCloseConnection() {
    [singleton.sdk closeConnection];
}

__used void beginSendingVideoForStream(int stream) {
    id<VXRendererHolder> holder;
    if (s_unityGFXRenderer == kUnityGfxRendererMetal) {
        holder = [[MetalRendererHolder alloc] initWithStream:stream
                                         nativeTextureReport:^(id <MTLTexture> o, void *pVoid, int width, int height) {
                                             CallUnityMethod(@"fonNewNativeTexture", @[@((long long)o), @((long long)pVoid), @(width), @(height), @(stream)]);
                                         }];
    } else {
        holder = [[EAGLRendererHolder alloc] initWithStream:stream
                               nativeTextureReport:^(GLuint textureId, void *context, int width, int height) {
                                   CallUnityMethod(@"fonNewNativeTexture", @[@(textureId), @((long long)context), @(width), @(height), @(stream)]);
                               }];
    }

    if (stream == 0) {
        [singleton removeRemoteRendererHolder];
        [singleton.sdk addRemoteRendererHolder:holder];
    } else if (stream == 1) {
        [singleton removeLocalRendererHolder];
        [singleton.sdk addLocalRendererHolder:holder];
    }
}
}
