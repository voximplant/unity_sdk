
//Objective-C Code
#import <Foundation/Foundation.h>

#import <VoxImplantSDK/VoxImplant.h>

extern "C" void UnitySendMessage(const char *obj, const char *method, const char *msg);

@interface DelegateSDK : NSObject <VoxImplantDelegate>
@end

@interface ViewLayoutSize : NSObject
@property int x_pos;
@property int y_pos;
@property int width;
@property int heigth;

- (instancetype)initWithParam:(int)xPos withyPos:(int)yPos withWidth:(int)pWidth withHeight:(int)pHeight;
@end

@implementation ViewLayoutSize

- (instancetype)init {
    self = [super init];
    if (self) {

    }
    return self;
}

- (instancetype)initWithParam:(int)xPos withyPos:(int)yPos withWidth:(int)pWidth withHeight:(int)pHeight {
    self = [super init];
    if (self) {
        [self setX_pos:xPos];
        [self setY_pos:yPos];
        [self setWidth:pWidth];
        [self setHeigth:pHeight];
    }
    return self;
}

@end

@interface JsonDic : NSObject

@property NSArray *list;
@property NSMutableDictionary *dic;

- (instancetype)initWithJSONString:(NSString *)JSONString;

@end

@implementation JsonDic

- (instancetype)init {
    self = [super init];
    if (self) {

    }
    return self;
}

- (instancetype)initWithJSONString:(NSString *)JSONString {
    self = [super init];
    if (self) {

        NSError *error = nil;
        NSData *JSONData = [JSONString dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *JSONDictionary = [NSJSONSerialization JSONObjectWithData:JSONData options:0 error:&error];

        if (!error && JSONDictionary) {

            //Loop method
            for (NSString *key in JSONDictionary) {
                [self setValue:[JSONDictionary valueForKey:key] forKey:key];
            }

            if (_list != Nil) {
                _dic = [[NSMutableDictionary alloc] init];
                for (NSDictionary *item in _list) {
                    [_dic setValue:item[@"value"] forKey:item[@"key"]];
                }
            }
        }
    }
    return self;
}

@end

@implementation DelegateSDK {
@public

    VoxImplant *sdk;
    NSString *unityObjName;
    UIAlertView *currentAlertView;
    UIView *remoteView;
    UIView *localView;

    ViewLayoutSize *sizeLocal;
    ViewLayoutSize *sizeRemote;
}


- (void)setLocalSizeViews:(ViewLayoutSize *)pLocalSize {
    sizeLocal = pLocalSize;
}

- (void)setRemSizeViews:(ViewLayoutSize *)pRemSize {
    sizeRemote = pRemSize;
}

- (void)setObjName:(const char *)objName {
    unityObjName = [[NSString alloc] initWithUTF8String:objName];
}

- (void)callUnityObject:(const char *)method Parameter:(const char *)parameter {
    if (parameter == nil)
        UnitySendMessage([self getCharFromNs:unityObjName], method, "");
    else
        UnitySendMessage([self getCharFromNs:unityObjName], method, parameter);
}

- (void)onConnectionSuccessful {
    [self callUnityObject:"fiosonConnectionSuccessful" Parameter:Nil];
}

- (void)onConnectionFailedWithError:(NSString *)reason {
    NSError *writeError = [NSError alloc];
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:@[reason] options:NSJSONWritingPrettyPrinted error:&writeError];
    NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];

    [self callUnityObject:"fiosonConnectionFailedWithError" Parameter:[self getCharFromNs:jsonString]];
}

- (void)onConnectionClosed {
    [self callUnityObject:"fiosonConnectionClosed" Parameter:Nil];
}

- (void)onLoginFailedWithErrorCode:(NSNumber *)errorCode {
    NSError *writeError = [NSError alloc];
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:@[errorCode] options:NSJSONWritingPrettyPrinted error:&writeError];
    NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    [self callUnityObject:"fiosonLoginFailed" Parameter:[self getCharFromNs:jsonString]];
}

- (void)onLoginSuccessfulWithDisplayName:(NSString *)displayName {
    NSError *writeError = [NSError alloc];
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:@[displayName] options:NSJSONWritingPrettyPrinted error:&writeError];
    NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    [self callUnityObject:"fiosonLoginSuccessful" Parameter:[self getCharFromNs:jsonString]];
}

- (void)onCallRinging:(NSString *)callId withHeaders:(NSDictionary *)headers {
    NSError *writeError = [NSError alloc];
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:@[callId, headers] options:NSJSONWritingPrettyPrinted error:&writeError];
    NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    [self callUnityObject:"fiosonCallRinging" Parameter:[self getCharFromNs:jsonString]];
}

- (void)onCallConnected:(NSString *)callId withHeaders:(NSDictionary *)headers {
    [sdk attachAudioTo:callId];
    NSError *writeError = [NSError alloc];
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:@[callId, headers] options:NSJSONWritingPrettyPrinted error:&writeError];
    NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];

    [self callUnityObject:"fiosonCallConnected" Parameter:[self getCharFromNs:jsonString]];
}

- (void)setLocaView:(bool)pState {
    if (pState) {
        if (sizeLocal != nil) {
            UIView *rootView = [UIApplication sharedApplication].keyWindow.rootViewController.view;
            if (localView != nil) {
                [sdk setLocalPreview:Nil];
                [self removeView:localView];
                localView = nil;
            }
            localView = [[UIView alloc] initWithFrame:CGRectMake(sizeLocal.x_pos, sizeLocal.y_pos, sizeLocal.width, sizeLocal.heigth)];
            [rootView addSubview:localView];
            [sdk setLocalPreview:localView];
        }
    } else {
        [sdk setLocalPreview:Nil];
        [self removeView:localView];
        localView = nil;
    }
}

- (void)setRemoteView:(bool)pState {
    if (pState) {
        if (sizeRemote != nil) {
            UIView *rootView = [UIApplication sharedApplication].keyWindow.rootViewController.view;
            if (remoteView != nil) {
                [sdk setRemoteView:Nil];
                [self removeView:remoteView];
                remoteView = nil;
            }
            remoteView = [[UIView alloc] initWithFrame:CGRectMake(sizeRemote.x_pos, sizeRemote.y_pos, sizeRemote.width, sizeRemote.heigth)];
            [rootView addSubview:remoteView];
            [sdk setRemoteView:remoteView];
        }
    } else {
        [sdk setRemoteView:Nil];
        [self removeView:remoteView];
        remoteView = nil;
    }
}


- (void)onCallDisconnected:(NSString *)callId withHeaders:(NSDictionary *)headers {
    if (currentAlertView != Nil) {
        UIAlertView *v = currentAlertView;
        currentAlertView = Nil;
        [v dismissWithClickedButtonIndex:0 animated:false];
    }

    NSError *writeError = [NSError alloc];
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:@[callId, headers] options:NSJSONWritingPrettyPrinted error:&writeError];
    NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];

    [self callUnityObject:"fiosonCallDisconnected" Parameter:[self getCharFromNs:jsonString]];
}

- (void)onCallFailed:(NSString *)callId withCode:(int)code andReason:(NSString *)reason withHeaders:(NSDictionary *)headers {
    NSError *writeError = [NSError alloc];
    NSArray *array = @[callId, [NSString stringWithFormat:@"%d", code], reason, headers];
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:array options:NSJSONWritingPrettyPrinted error:&writeError];
    NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];

    [self callUnityObject:"fiosonCallFailed" Parameter:[self getCharFromNs:jsonString]];
}


- (void)onIncomingCall:(NSString *)callId caller:(NSString *)from named:(NSString *)displayName withVideo:(bool)videoCall withHeaders:(NSDictionary *)headers {
    NSError *writeError = [NSError alloc];
    NSArray *array = @[callId, from, displayName, videoCall ? @"true" : @"false", headers];
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:array options:NSJSONWritingPrettyPrinted error:&writeError];
    NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];

    [self callUnityObject:"fiosonIncomingCall" Parameter:[self getCharFromNs:jsonString]];
}

- (void)onCallAudioStarted:(NSString *)callId {
    [sdk attachAudioTo:callId];
    NSError *writeError = [NSError alloc];
    NSArray *array = @[callId];
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:@[array] options:NSJSONWritingPrettyPrinted error:&writeError];
    NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    [self callUnityObject:"fiosonCallAudioStarted" Parameter:[self getCharFromNs:jsonString]];
}

- (void)onNetStatsReceivedInCall:(NSString *)callId withStats:(const struct VoxImplantNetworkInfo *)stats {
    NSError *writeError = [NSError alloc];
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:@[callId, [NSString stringWithFormat:@"%ld", stats->packetLoss]] options:NSJSONWritingPrettyPrinted error:&writeError];
    NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];

    [self callUnityObject:"fiosonNetStatsReceived" Parameter:[self getCharFromNs:jsonString]];

}

- (void)onSIPInfoReceivedInCall:(NSString *)callId withType:(NSString *)type andContent:(NSString *)content withHeaders:(NSDictionary *)headers {
    NSError *writeError = [NSError alloc];
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:@[callId, type, content, headers] options:NSJSONWritingPrettyPrinted error:&writeError];
    NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];

    [self callUnityObject:"fiosonSIPInfoReceivedInCall" Parameter:[self getCharFromNs:jsonString]];
}

- (void)onMessageReceivedInCall:(NSString *)callId withText:(NSString *)text withHeaders:(NSDictionary *)headers {
    NSError *writeError = [NSError alloc];
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:@[callId, text, headers] options:NSJSONWritingPrettyPrinted error:&writeError];
    NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];

    [self callUnityObject:"fiosonMessageReceivedInCall" Parameter:[self getCharFromNs:jsonString]];
}

- (void)removeView:(UIView *)pView {
    if (pView != nil) {
        [pView setHidden:true];
        [pView removeFromSuperview];
    }
}

- (const char *)getCharFromNs:(NSString *)str {
    NSMutableString *param = [[NSMutableString alloc] initWithFormat:@"%@", str];
    return [param UTF8String];

}

@end

//C-wrapper that Unity communicates with
extern "C"
{
DelegateSDK *singleton;

void iosSDKinit(const char *pUnityObjName) {
    singleton = [[DelegateSDK alloc] init];
    singleton->sdk = [VoxImplant getInstance];
    [singleton->sdk setVoxDelegate:singleton];
    [singleton setObjName:pUnityObjName];
    [singleton->sdk setResolution:352 andHeight:288];
}

void iosSDKconnect() {
    [singleton->sdk connect];
}

void iosSDKlogin(const char *pLogin, const char *pPass) {
    [singleton->sdk loginWithUsername:[NSString stringWithUTF8String:pLogin] andPassword:[NSString stringWithUTF8String:pPass]];
}

void iosSDKsetLocalSize(int xPos, int yPos, int pWidth, int pHeight) {
    [singleton setLocalSizeViews:[[ViewLayoutSize alloc] initWithParam:xPos withyPos:yPos withWidth:pWidth withHeight:pHeight]];
}

void iosSDKsetRemoteSize(int xPos, int yPos, int pWidth, int pHeight) {
    [singleton setRemSizeViews:[[ViewLayoutSize alloc] initWithParam:xPos withyPos:yPos withWidth:pWidth withHeight:pHeight]];
}

void iosSDKsetLocalView(bool pState) {
    [singleton setLocaView:pState];
}

void iosSDKsetRemoteView(bool pState) {
    [singleton setRemoteView:pState];
}

void iosSDKstartCall(const char *pId, bool pWithVideo, const char *pCustom, const char *pHeaders) {
    NSString *activeCallId = [singleton->sdk createCall:[NSString stringWithUTF8String:pId]
                                              withVideo:pWithVideo
                                          andCustomData:[[NSString alloc] initWithUTF8String:pCustom]];
    [singleton->sdk attachAudioTo:activeCallId];

    if (pHeaders == nil)
        [singleton->sdk startCall:activeCallId withHeaders:Nil];
    else {
        JsonDic *headers = [[JsonDic alloc] initWithJSONString:[[NSString alloc] initWithUTF8String:pHeaders]];
        [singleton->sdk startCall:activeCallId withHeaders:headers.dic];
    }

    UnitySendMessage([singleton->unityObjName UTF8String], "fiosonOnStartCall", [activeCallId UTF8String]);
}

void iosSDKanswerCall(const char *pCallId, const char *pHeaders) {
    if (pCallId != Nil) {
        if (pHeaders == Nil)
            [singleton->sdk answerCall:[NSString stringWithUTF8String:pCallId] withHeaders:Nil];
        else {
            JsonDic *headers = [[JsonDic alloc] initWithJSONString:[[NSString alloc] initWithUTF8String:pHeaders]];
            [singleton->sdk answerCall:[NSString stringWithUTF8String:pCallId] withHeaders:headers.dic];
        }
    }
}

void iosSDKHungup(const char *pCallId, const char *pHeaders) {
    if (pHeaders == Nil)
        [singleton->sdk disconnectCall:[NSString stringWithUTF8String:pCallId] withHeaders:Nil];
    else {
        JsonDic *headers = [[JsonDic alloc] initWithJSONString:[[NSString alloc] initWithUTF8String:pHeaders]];
        [singleton->sdk disconnectCall:[NSString stringWithUTF8String:pCallId] withHeaders:headers.dic];
    }
}

void iosSDKDecline(const char *pCallId, const char *pHeaders) {
    if (pHeaders == Nil)
        [singleton->sdk declineCall:[NSString stringWithUTF8String:pCallId] withHeaders:Nil];
    else {
        JsonDic *headers = [[JsonDic alloc] initWithJSONString:[[NSString alloc] initWithUTF8String:pHeaders]];
        [singleton->sdk declineCall:[NSString stringWithUTF8String:pCallId] withHeaders:headers.dic];
    }
}

void iosSDKsetMute(bool pSetMute) {
    [singleton->sdk setMute:pSetMute];
}

void iosSDKsendVideo(bool pSendVideo) {
    [singleton->sdk sendVideo:pSendVideo];
}

void iosSDKsetCamera(bool pFrontCam) {
    if (pFrontCam)
        [singleton->sdk switchToCameraWithPosition:AVCaptureDevicePositionFront];
    else
        [singleton->sdk switchToCameraWithPosition:AVCaptureDevicePositionBack];
}

void iosSDKdisableTls() {
    [singleton->sdk disableTLS];
}

void iosSDKdisconnectCall(const char *pCall, const char *pHeaders) {
    if (pHeaders == Nil)
        [singleton->sdk disconnectCall:[NSString stringWithUTF8String:pCall] withHeaders:Nil];
    else {
        JsonDic *headers = [[JsonDic alloc] initWithJSONString:[[NSString alloc] initWithUTF8String:pHeaders]];
        [singleton->sdk disconnectCall:[NSString stringWithUTF8String:pCall] withHeaders:headers.dic];
    }
}

void iosSDKloginUsingOneTimeKey(const char *pUserName, const char *pOneTimeKey) {
    [singleton->sdk loginWithUsername:[NSString stringWithUTF8String:pUserName] andOneTimeKey:[NSString stringWithUTF8String:pOneTimeKey]];
}

void iosSDKrequestOneTimeKey(const char *pUserName) {
    [singleton->sdk requestOneTimeKeyWithUsername:[NSString stringWithUTF8String:pUserName]];
}

void iosSDKsendDTFM(const char *pCallId, int pDigit) {
    [singleton->sdk sendDTMF:[NSString stringWithUTF8String:pCallId] digit:pDigit];
}

void iosSDKsendInfo(const char *pCallId, const char *pMimeType, const char *pContent, const char *pAniHeaders) {
    if (pAniHeaders == Nil)
        [singleton->sdk sendInfo:[NSString stringWithUTF8String:pCallId] withType:[NSString stringWithUTF8String:pMimeType] content:[NSString stringWithUTF8String:pContent] andHeaders:Nil];
    else {
        JsonDic *headers = [[JsonDic alloc] initWithJSONString:[[NSString alloc] initWithUTF8String:pAniHeaders]];
        [singleton->sdk sendInfo:[NSString stringWithUTF8String:pCallId] withType:[NSString stringWithUTF8String:pMimeType] content:[NSString stringWithUTF8String:pContent] andHeaders:headers.dic];
    }
}

void iosSDKsendMessage(const char *pCallId, const char *pMsg, const char *pAniHeaders) {
    if (pAniHeaders == Nil)
        [singleton->sdk sendMessage:[NSString stringWithUTF8String:pCallId] withText:[NSString stringWithUTF8String:pMsg] andHeaders:Nil];
    else {
        JsonDic *headers = [[JsonDic alloc] initWithJSONString:[[NSString alloc] initWithUTF8String:pAniHeaders]];
        [singleton->sdk sendMessage:[NSString stringWithUTF8String:pCallId] withText:[NSString stringWithUTF8String:pMsg] andHeaders:headers.dic];
    }
}

void iosSDKsetCameraResolution(int pWidth, int pHeight) {
    [singleton->sdk setResolution:pWidth andHeight:pHeight];
}

void iosSDKsetUseLoudspeaker(bool pUseLoudspeaker) {
    [singleton->sdk setUseLoudspeaker:pUseLoudspeaker];
}

void iosSDKCloseConnection() {
    [singleton->sdk closeConnection];
}
}
