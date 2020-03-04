/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

#include <cstdint>
#include <cmath>
#include "IUnityRenderingExtensions.h"

#include "VINativeInterface.h"

#import "VIClientModule.h"
#import "VICallModule.h"
#import "VIVideoStreamModule.h"
#import "VIVideoStreamWrapper.h"
#import "VIEndpointModule.h"
#import "VIAudioManagerModule.h"
#import "VICameraManagerModule.h"

#pragma clang diagnostic push
#pragma ide diagnostic ignored "OCUnusedGlobalDeclarationInspection"

#define UNITY_LISTENER "Voximplant"
#define VOXIMPLANT_EXPORT extern "C"

extern "C" void UnitySendMessage(const char *class_name, const char *method_name, const char *param);

NSString *voximplant_cstring_to_nsstring(const char *cstring) {
    if (cstring) {
        return [NSString stringWithUTF8String:cstring];
    }
    return nil;
}

const char *voximplant_nsstring_to_cstring(NSString *nsstring) {
    const char *cstring = nsstring.UTF8String;
    if (cstring == NULL) {
        return NULL;
    }

    char *buffer = (char *) calloc(strlen(cstring) + 1, sizeof(char));
    strcpy(buffer, cstring);
    return buffer;
}

void voximplant_send_message(const char *node, NSString *event_payload) {
    UnitySendMessage(UNITY_LISTENER, node, voximplant_nsstring_to_cstring(event_payload));
}

static UnityGfxRenderer s_gfxRenderer;

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API
UnityPluginLoad(IUnityInterfaces* unityInterfaces)
{
    IUnityGraphics* graphics = unityInterfaces->Get<IUnityGraphics>();
    s_gfxRenderer = graphics->GetRenderer();
}

VOXIMPLANT_EXPORT void voximplant_init(const char *client_config) {
    [[VIClientModule sharedModule] initializeModule:voximplant_cstring_to_nsstring(client_config)];

    NSLog(@"Renderer: %d", s_gfxRenderer);
}

#pragma mark - VIClient

VOXIMPLANT_EXPORT void voximplant_client_connect(bool connectivity_check, const char *servers) {
    NSArray<NSString *> *clientServers = nil;
    if (strlen(servers) > 0) {
        clientServers = [voximplant_cstring_to_nsstring(servers) componentsSeparatedByString:@";"];
    }
    [[VIClientModule sharedModule] connect:connectivity_check gateways:clientServers];
}

VOXIMPLANT_EXPORT NSUInteger voximplant_client_state() {
    return [VIClientModule sharedModule].state;
}

VOXIMPLANT_EXPORT void voximplant_client_disconnect() {
    [[VIClientModule sharedModule] disconnect];
}

VOXIMPLANT_EXPORT void voximplant_client_login(const char *username, const char *password) {
    [[VIClientModule sharedModule] login:voximplant_cstring_to_nsstring(username) password:voximplant_cstring_to_nsstring(password)];
}

VOXIMPLANT_EXPORT void voximplant_client_login_with_token(const char *username, const char *token) {
    [[VIClientModule sharedModule] login:voximplant_cstring_to_nsstring(username) token:voximplant_cstring_to_nsstring(token)];
}

VOXIMPLANT_EXPORT void voximplant_client_login_with_one_time_key(const char *username, const char *hash) {
    [[VIClientModule sharedModule] login:voximplant_cstring_to_nsstring(username) hash:voximplant_cstring_to_nsstring(hash)];
}

VOXIMPLANT_EXPORT void voximplant_client_request_one_time_key(const char *username) {
    [[VIClientModule sharedModule] requestOneTimeKey:voximplant_cstring_to_nsstring(username)];
}

VOXIMPLANT_EXPORT void voximplant_client_refresh_token(const char *username, const char *refresh_token) {
    [[VIClientModule sharedModule] refreshTokenWithUser:voximplant_cstring_to_nsstring(username) token:voximplant_cstring_to_nsstring(refresh_token)];
}

VOXIMPLANT_EXPORT const char *voximplant_client_call(const char *username, bool receive_video, bool send_video, NSInteger video_codec, const char *custom_data, const char *headers) {
    return voximplant_nsstring_to_cstring([[VIClientModule sharedModule] call:voximplant_cstring_to_nsstring(username)
                                                                 receiveVideo:receive_video
                                                                    sendVideo:send_video
                                                                   videoCodec:video_codec
                                                                   customData:voximplant_cstring_to_nsstring(custom_data)
                                                                      headers:voximplant_cstring_to_nsstring(headers)]);
}

VOXIMPLANT_EXPORT const char *voximplant_client_call_conference(const char *username, bool receive_video, bool send_video, NSInteger video_codec, const char *custom_data, const char *headers) {
    return voximplant_nsstring_to_cstring([[VIClientModule sharedModule] callConference:voximplant_cstring_to_nsstring(username)
                                                                           receiveVideo:receive_video
                                                                              sendVideo:send_video
                                                                             videoCodec:video_codec
                                                                             customData:voximplant_cstring_to_nsstring(custom_data)
                                                                                headers:voximplant_cstring_to_nsstring(headers)]);
}

#pragma mark - VICall

VOXIMPLANT_EXPORT const char *voximplant_call_endpoints(const char *call_id) {
    return voximplant_nsstring_to_cstring([[VICallModule sharedModule] endpointsForCall:voximplant_cstring_to_nsstring(call_id)]);
}

VOXIMPLANT_EXPORT const char *voximplant_call_video_streams(const char *call_id) {
    return voximplant_nsstring_to_cstring([[VICallModule sharedModule] videoStreamsForCall:voximplant_cstring_to_nsstring(call_id)]);
}

VOXIMPLANT_EXPORT double voximplant_call_duration(const char *call_id) {
    return [[VICallModule sharedModule] durationForCall:voximplant_cstring_to_nsstring(call_id)];
}

VOXIMPLANT_EXPORT void voximplant_call_set_send_audio(const char *call_id, bool send_audio) {
    [[VICallModule sharedModule] setSendAudio:send_audio forCall:voximplant_cstring_to_nsstring(call_id)];
}

VOXIMPLANT_EXPORT void voximplant_call_start_receive_video(const char *call_id, const char *request_guid) {
    [[VICallModule sharedModule] startReceiveVideoForCall:voximplant_cstring_to_nsstring(call_id) requestGuid:voximplant_cstring_to_nsstring(request_guid)];
}

VOXIMPLANT_EXPORT void voximplant_call_set_send_video(const char *call_id, bool send_video, const char *request_guid) {
    [[VICallModule sharedModule] setSendVideo:send_video forCall:voximplant_cstring_to_nsstring(call_id) requestGuid:voximplant_cstring_to_nsstring(request_guid)];
}

VOXIMPLANT_EXPORT void voximplant_call_set_hold(const char *call_id, bool hold, const char *request_guid) {
    [[VICallModule sharedModule] setHold:hold forCall:voximplant_cstring_to_nsstring(call_id) requestGuid:voximplant_cstring_to_nsstring(request_guid)];
}

VOXIMPLANT_EXPORT void voximplant_call_send_message(const char *call_id, const char *message) {
    [[VICallModule sharedModule] sendMessage:voximplant_cstring_to_nsstring(message) toCall:voximplant_cstring_to_nsstring(call_id)];
}

VOXIMPLANT_EXPORT void voximplant_call_send_info(const char *call_id, const char *content, const char *mime_type, const char *headers) {
    [[VICallModule sharedModule] sendInfo:voximplant_cstring_to_nsstring(content)
                                 mimeType:voximplant_cstring_to_nsstring(mime_type)
                                  headers:voximplant_cstring_to_nsstring(headers)
                                   toCall:voximplant_cstring_to_nsstring(call_id)];
}

VOXIMPLANT_EXPORT void voximplant_call_send_dtmf(const char *call_id, const char *dtmf) {
    [[VICallModule sharedModule] sendDTMF:voximplant_cstring_to_nsstring(dtmf) toCall:voximplant_cstring_to_nsstring(call_id)];
}

VOXIMPLANT_EXPORT void voximplant_call_start(const char *call_id) {
    [[VICallModule sharedModule] startCall:voximplant_cstring_to_nsstring(call_id)];
}

VOXIMPLANT_EXPORT void voximplant_call_answer(const char *call_id, bool receive_video, bool send_video, NSInteger video_codec, const char *custom_data, const char *headers) {
    [[VICallModule sharedModule] answerCall:voximplant_cstring_to_nsstring(call_id)
                           withReceiveVideo:receive_video
                                  sendVideo:send_video
                                 videoCodec:video_codec
                                 customData:voximplant_cstring_to_nsstring(custom_data)
                                    headers:voximplant_cstring_to_nsstring(headers)];
}

VOXIMPLANT_EXPORT void voximplant_call_reject(const char *call_id, NSInteger reject_mode, const char *headers) {
    [[VICallModule sharedModule] rejectCall:voximplant_cstring_to_nsstring(call_id) mode:reject_mode headers:voximplant_cstring_to_nsstring(headers)];
}

VOXIMPLANT_EXPORT void voximplant_call_hangup(const char *call_id, const char *headers) {
    [[VICallModule sharedModule] hangupCall:voximplant_cstring_to_nsstring(call_id) headers:voximplant_cstring_to_nsstring(headers)];
}

#pragma mark - VICustomVideoSource

VOXIMPLANT_EXPORT const char * voximplant_call_custom_video_source_create(const char *call_id, int width, int height, NSUInteger fps) {
    return voximplant_nsstring_to_cstring([[VICallModule sharedModule]
            createCustomVideoSource:voximplant_cstring_to_nsstring(call_id)
                          frameSize:CGSizeMake(width, height)
                                fps:fps]);
}

VOXIMPLANT_EXPORT void voximplant_call_custom_video_source_send_frame(void *data, const char *source_id) {
    [[VICallModule sharedModule] sendVideoFrame:data forSource:voximplant_cstring_to_nsstring(source_id)];
}

VOXIMPLANT_EXPORT void voximplant_call_custom_video_source_release(const char *source_id) {
    [[VICallModule sharedModule] releaseCustomVideoSource:voximplant_cstring_to_nsstring(source_id)];
}

#pragma mark - VIEndpoint

VOXIMPLANT_EXPORT const char *voximplant_endpoint_username(const char *endpoint_id) {
    return voximplant_nsstring_to_cstring(voximplant_cstring_to_nsstring(endpoint_id));
}

VOXIMPLANT_EXPORT const char *voximplant_endpoint_user_display_name(const char *endpoint_id) {
    return voximplant_nsstring_to_cstring(voximplant_cstring_to_nsstring(endpoint_id));
}

VOXIMPLANT_EXPORT const char *voximplant_endpoint_sip_uri(const char *endpoint_id) {
    return voximplant_nsstring_to_cstring(voximplant_cstring_to_nsstring(endpoint_id));
}

#pragma mark - VIVideoStream

VOXIMPLANT_EXPORT NSInteger voximplant_video_stream_width(const char *stream_id) { // @todo: nullchecking
    return [[VIVideoStreamModule sharedModule] videoStreamForId:voximplant_cstring_to_nsstring(stream_id)].width;
}

VOXIMPLANT_EXPORT NSInteger voximplant_video_stream_height(const char *stream_id) {
    return [[VIVideoStreamModule sharedModule] videoStreamForId:voximplant_cstring_to_nsstring(stream_id)].height;
}

VOXIMPLANT_EXPORT NSInteger voximplant_video_stream_rotation(const char *stream_id) {
    return [[VIVideoStreamModule sharedModule] videoStreamForId:voximplant_cstring_to_nsstring(stream_id)].rotation;
}

VOXIMPLANT_EXPORT long long voximplant_video_stream_texture_ptr(const char *stream_id) {
    return [[VIVideoStreamModule sharedModule] videoStreamForId:voximplant_cstring_to_nsstring(stream_id)].texture;
}

VOXIMPLANT_EXPORT void voximplant_video_stream_update_texture(const char *stream_id) {
    [[[VIVideoStreamModule sharedModule] videoStreamForId:voximplant_cstring_to_nsstring(stream_id)] updateTexture];
}

VOXIMPLANT_EXPORT void voximplant_video_stream_dispose_texture(const char *stream_id) {

}

// @fixme: keep?
VOXIMPLANT_EXPORT void voximplant_dump() {
}

VOXIMPLANT_EXPORT void voximplant_video_stream_texture_update_callback(int event_id, void *data) {
    auto event = static_cast<UnityRenderingExtEventType>(event_id);
    auto params = reinterpret_cast<UnityRenderingExtTextureUpdateParamsV2 *>(data);

    if (event == kUnityRenderingExtEventUpdateTextureBeginV2) {
        auto video_stream = params->userData;
        params->textureID = [[VIVideoStreamModule sharedModule] textureForStream:video_stream];
    } else if (event == kUnityRenderingExtEventUpdateTextureEndV2) {
//        delete[] reinterpret_cast<uint8_t *>(params->texData);
    }
}

VOXIMPLANT_EXPORT uint32_t voximplant_video_stream_get_id(const char *stream_id) {
    return [[VIVideoStreamModule sharedModule] idForVideoStream:voximplant_cstring_to_nsstring(stream_id)];
}

VOXIMPLANT_EXPORT UnityRenderingEventAndData UNITY_INTERFACE_EXPORT voximplant_video_stream_get_texture_update_callback() {
    return voximplant_video_stream_texture_update_callback;
}

#pragma mark - VIAudioManager

VOXIMPLANT_EXPORT void voximplant_audio_manager_select_audio_device(int audioDevice) {
    [[VIAudioManagerModule sharedInstance] selectAudioDevice:static_cast<VIAudioDeviceType>(audioDevice)];
}

VOXIMPLANT_EXPORT int voximplant_audio_manager_current_audio_device() {
    return [[VIAudioManagerModule sharedInstance] currentAudioDevice];
}

VOXIMPLANT_EXPORT const char *voximplant_audio_manager_available_audio_devices() {
    return voximplant_nsstring_to_cstring([[VIAudioManagerModule sharedInstance] availableAudioDevices]);
}

#pragma mark - VICameraManager

VOXIMPLANT_EXPORT void voximplant_camera_manager_switch_camera(int cameraType) {
    [[VICameraManagerModule sharedInstance] switchCamera:static_cast<VICameraType>(cameraType)];
}

VOXIMPLANT_EXPORT void voximplant_camera_manager_set_camera_resolution(int width, int height) {
    [[VICameraManagerModule sharedInstance] setCameraResolution:CGSizeMake(width, height)];
}

#pragma clang diagnostic pop
