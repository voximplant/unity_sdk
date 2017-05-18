/*
 * Copyright (c) 2017, Zingaya, Inc. All rights reserved.
 */

package com.zingaya.voximplant;

import android.content.Context;
import android.os.Handler;
import android.os.Looper;
import android.util.Log;

import com.voximplant.sdk.call.CallException;
import com.voximplant.sdk.call.CallStatistic;
import com.voximplant.sdk.call.ICallCompletionHandler;
import com.voximplant.sdk.call.ICustomVideoSource;
import com.voximplant.sdk.call.IEndpoint;
import com.voximplant.sdk.call.IEndpointListener;
import com.voximplant.sdk.call.RenderScaleType;
import com.voximplant.sdk.client.AuthParams;
import com.voximplant.sdk.client.ClientConfig;
import com.voximplant.sdk.client.LoginError;
import com.voximplant.sdk.hardware.IAudioDeviceManager;
import com.voximplant.sdk.call.ICall;
import com.voximplant.sdk.call.ICallListener;
import com.voximplant.sdk.hardware.ICameraManager;
import com.voximplant.sdk.client.IClient;
import com.voximplant.sdk.client.IClientIncomingCallListener;
import com.voximplant.sdk.client.IClientLoginListener;
import com.voximplant.sdk.client.IClientSessionListener;
import com.voximplant.sdk.call.IVideoStream;
import com.voximplant.sdk.Voximplant;

import org.webrtc.VideoRenderer;

import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.Timer;
import java.util.TimerTask;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.Executors;

import static com.voximplant.sdk.internal.constants.CallConstants.*;

class UnityVoxImplantClientImp implements IClientSessionListener,
        IClientLoginListener,
        IClientIncomingCallListener,
        ICallListener,
        IEndpointListener {

    private static String TAG = "VOXSDK";
    private Handler handler = new Handler(Looper.getMainLooper());
    private IClient voxClient = null;
    private IAudioDeviceManager audioDevice = null;
    private ICameraManager cameraManager = null;
    private Map<String, ICall> calls = new LinkedHashMap<>();
    private Map<String, IEndpoint> endpointMap = new ConcurrentHashMap<>();
    private int captureWidth = CAMERA_WIDTH_640;
    private int captureHeight = CAMERA_HEIGHT_480;
    private int cameraType = CAMERA_FRONT_FACING;
    private ConcurrentHashMap<String, Timer> callStatistics = new ConcurrentHashMap<>();

    //renderer
    private HashMap<String, IVideoStream> localVideoStreams = new HashMap<>();
    private HashMap<String, IVideoStream> remoteVideoStreams = new HashMap<>();
    private HashMap<String, VideoRenderer.Callbacks> localRenders = new HashMap<>();
    private HashMap<String, VideoRenderer.Callbacks> remoteRenders = new HashMap<>();
    private VideoRenderer.Callbacks localView;
    private VideoRenderer.Callbacks remoteView;

    UnityVoxImplantClientImp(Context context, UnityVoxImplantClient.VoxImplantClientConfig clientConfig) {
        ClientConfig config = new ClientConfig();
        config.enableVideo = clientConfig.enableVideo;
        config.enableHWAcceleration = clientConfig.enableHWAcceleration;
        config.provideLocalFramesInByteBuffers = clientConfig.provideLocalFramesInByteBuffers;
        config.enableDebugLogging = clientConfig.enableDebugLogging;

        voxClient = Voximplant.getClientInstance(Executors.newSingleThreadExecutor(), context, config);
        audioDevice = Voximplant.getAudioDeviceManager();
        cameraManager = Voximplant.getCameraManager(context);
    }

    private void runOnUIThread(Runnable runnable) {
        handler.post(runnable);
    }

    private VoxImplantCallback callback;

    void setCallback(VoxImplantCallback callback) {
        this.callback = callback;
        voxClient.setClientIncomingCallListener(this);
        voxClient.setClientLoginListener(this);
        voxClient.setClientSessionListener(this);
    }

    void connect(boolean connectivityCheck, List<String> serverNames) {
        voxClient.connect(connectivityCheck, serverNames);
    }

    void disconnect() {
        voxClient.disconnect();
    }

    void login(String user, String password) {
        voxClient.login(user, password);
    }

    void requestOneTimeKey(String user) {
        voxClient.requestOneTimeKey(user);
    }

    void loginUsingOneTimeKey(String user, String hash) {
        voxClient.loginWithOneTimeKey(user, hash);
    }

    void loginUsingAccessToken(String user, String accessToken) {
        voxClient.loginWithAccessToken(user, accessToken);
    }

    void refreshToken(String user, String refreshToken) {
        voxClient.refreshToken(user, refreshToken);
    }

    String createCall(String to, boolean video, String customData) {
        ICall call = voxClient.callTo(to, video, customData);
        String callId = null;
        if (call != null) {
            callId = call.getCallId();
            calls.put(callId, call);
            endpointMap.put(call.getEndpoints().get(0).getEndpointId(), call.getEndpoints().get(0));
        }
        return callId;
    }

    boolean startCall(String callId, Map<String, String> headers) {
        ICall call = calls.get(callId);
        if (call != null) {
            call.addCallListener(this);
            endpointMap.get(callId).setEndpointListener(this);
            call.start(headers);
            return true;
        }
        return false;
    }

    void answerCall(String callId, Map<String, String> headers) {
        ICall call = calls.get(callId);
        if (call != null) {
            try {
                call.answer(headers);
            } catch (CallException e) {
                e.printStackTrace();
            }
        }
    }

    void declineCall(String callId, Map<String, String> headers) {
        ICall call = calls.get(callId);
        if (call != null) {
            try {
                call.reject(headers);
                checkAndRemoveRenderers(call);
                calls.remove(callId);
            } catch (CallException e) {
                e.printStackTrace();
            }
        }
    }

    void hangupCall(String callId, Map<String, String> headers) {
        ICall call = calls.get(callId);
        if (call != null) {
            call.hangup(headers);
            checkAndRemoveRenderers(call);
            calls.remove(callId);
        }
    }

    void setMute(boolean doMute) {
        for (Map.Entry<String, ICall> call : calls.entrySet()) {
            call.getValue().sendAudio(!doMute);
        }
    }

    boolean setUseLoudspeaker(boolean useLoudspeaker) {
        return audioDevice.enableLoudspeaker(useLoudspeaker);
    }

    void setCameraResolution(int width, int height) {
        captureWidth = width;
        captureHeight = height;
        cameraManager.setCamera(cameraType, width, height);
    }

    void setCamera(int cam) {
        if (cam != cameraType) {
            cameraType = cam;
            cameraManager.setCamera(cam, captureWidth, captureHeight);
        }
    }

    void sendVideo(boolean doSendVideo) {
        for (Map.Entry<String, ICall> call : calls.entrySet()) {
            call.getValue().sendVideo(doSendVideo, null);
        }
    }

    void setLocalPreview(VideoRenderer.Callbacks videoView) {
        Log.i(TAG, "UnityVoxImplantClientImp: setLocalPreview: videoView" + videoView);
        for (Map.Entry<String, ICall> callEntry : calls.entrySet()) {
            ICall call = callEntry.getValue();
            if (call == null) {
                break;
            }
            IVideoStream videoStream = localVideoStreams.get(callEntry.getKey());
            if (videoStream != null) {
                if (videoView != null) {
                    Log.i(TAG,
                            "UnityVoxImplantClientImp: setLocalPreview adding video view to local renderers. localRenderers size = " + localRenders.size());
                    videoStream.addVideoRenderer(videoView, RenderScaleType.SCALE_FILL);
                    localRenders.put(call.getCallId(), videoView);
                    localView = null;
                } else {
                    videoStream.removeVideoRenderer(localRenders.get(call.getCallId()));
                    localRenders.clear();
                }
            } else {
                localView = videoView;
            }
        }
        if (calls.isEmpty()) {
            localView = videoView;
        }
        Log.v(TAG,
                "UnityVoxImplantClientImp: setLocalPreview: localView = " + localView + ", localRenderers = " + localRenders.toString());
    }

    void setRemoteView(VideoRenderer.Callbacks videoView) {
        Log.i(TAG, "UnityVoxImplantClientImp: setRemoteView: videoView" + videoView);
        if (calls.isEmpty()) {
            Log.e(TAG, "UnityVoxImplantClientImp: setRemoteView: there is no calls");
            remoteView = videoView;
            return;
        }
        if (calls.size() > 1) {
            Log.e(TAG,
                    "UnityVoxImplantClientImp: setRemoteView: there is more than 1 call, use setRemoteView method with call id");
            return;
        }

        Map.Entry<String, ICall> entry = calls.entrySet().iterator().next();
        ICall call = entry.getValue();
        setRemoteView(call.getCallId(), videoView);
        Log.v(TAG,
                "UnityVoxImplantClientImp: setRemoteView (empty): remoteView = " + remoteView + ", remoteRenders = " + remoteRenders.toString());
    }

    void setRemoteView(String callId, VideoRenderer.Callbacks videoView) {
        Log.i(TAG, "UnityVoxImplantClientImp: setRemoteView: callId: " + callId + "videoView: " + videoView);
        ICall call = calls.get(callId);
        if (call == null && !calls.isEmpty()) {
            call = calls.entrySet().iterator().next().getValue();
        }

        if (call != null) {
            IVideoStream videoStream = remoteVideoStreams.get(call.getCallId());
            if (videoStream != null) {
                if (videoView != null) {
                    videoStream.addVideoRenderer(videoView, RenderScaleType.SCALE_FILL);
                    remoteRenders.put(call.getCallId(), videoView);
                    remoteView = null;
                } else {
                    for (Map.Entry<String, VideoRenderer.Callbacks> entry : remoteRenders.entrySet()) {
                        if (entry.getKey().equals(call.getCallId())) {
                            videoStream.removeVideoRenderer(remoteRenders.get(call.getCallId()));
                        }
                    }
                    remoteRenders.remove(call.getCallId());
                }
            } else {
                Log.i(TAG, "UnityVoxImplantClientImp: setRemoteView: no video stream found for this call");
                remoteView = videoView;
            }
        } else {
            Log.i(TAG, "UnityVoxImplantClientImp: setRemoteView: no call found for this id");
            remoteView = videoView;
        }
        Log.v(TAG,
                "UnityVoxImplantClientImp: setRemoteView: remoteView = " + remoteView + ", remoteRenders = " + remoteRenders.toString());
    }

    private void checkAndRemoveRenderers(ICall call) {
        if (calls.containsKey(call.getCallId())) {
            if (localRenders.containsKey(call.getCallId())) {
                setLocalPreview(null);
            }
            if (remoteRenders.containsKey(call.getCallId())) {
                setRemoteView(call.getCallId(), null);
            }
        }
        localView = null;
        remoteView = null;
    }

    void sendDTMF(String callId, int digit) {
        ICall call = calls.get(callId);
        if (call != null) {
            if (digit == 10) {
                call.sendDTMF("*");
            } else if (digit == 11) {
                call.sendDTMF("#");
            } else {
                call.sendDTMF(Integer.toString(digit));
            }
        }
    }

    void sendMessage(String callId, String text) {
        ICall call = calls.get(callId);
        if (call != null) {
            call.sendMessage(text);
        }
    }

    void sendInfo(String callId, String mimeType, String content, Map<String, String> headers) {
        ICall call = calls.get(callId);
        if (call != null) {
            call.sendInfo(mimeType, content, headers);
        }
    }

    static List<String> getMissingPermissions(Context context, boolean videoSupportEnabled) {
        return Voximplant.getMissingPermissions(context, videoSupportEnabled);
    }

    long getCallDuration(String callId) {
        ICall call = calls.get(callId);
        if (call != null) {
            return call.getCallDuration();
        }
        return -1;
    }

    private Integer convertRefreshFailureReason(LoginError reason) {
        switch (reason) {
            case INVALID_PASSWORD:
                return 401;
            case ACCOUNT_FROZEN:
                return 403;
            case INVALID_USERNAME:
                return 404;
            case TOKEN_EXPIRED:
                return 701;
            case INTERNAL_ERROR:
            default:
                return 500;
        }
    }

    private VoxImplantClient.LoginFailureReason convertLoginFailureReason(LoginError reason) {
        switch (reason) {
            case INVALID_PASSWORD:
                return VoxImplantClient.LoginFailureReason.INVALID_PASSWORD;
            case ACCOUNT_FROZEN:
                return VoxImplantClient.LoginFailureReason.ACCOUNT_FROZEN;
            case INVALID_USERNAME:
                return VoxImplantClient.LoginFailureReason.INVALID_USERNAME;
            case TOKEN_EXPIRED:
                return VoxImplantClient.LoginFailureReason.TOKEN_EXPIRED;
            case INTERNAL_ERROR:
            default:
                return VoxImplantClient.LoginFailureReason.INTERNAL_ERROR;
        }
    }

    @Override
    public void onConnectionEstablished() {
        Log.d(TAG, "UnityVoxImplantClientImp: onConnected");
        runOnUIThread(new Runnable() {
            @Override
            public void run() {
                callback.onConnectionSuccessful();
            }
        });

    }

    @Override
    public void onConnectionFailed(final String error) {
        Log.d(TAG, "UnityVoxImplantClientImp: onConnectFailed(error = " + error + ")");
        runOnUIThread(new Runnable() {
            @Override
            public void run() {
                callback.onConnectionFailedWithError(error);
            }
        });
    }

    @Override
    public void onConnectionClosed() {
        Log.d(TAG, "UnityVoxImplantClientImp: onDisconnected");
        runOnUIThread(new Runnable() {
            @Override
            public void run() {
                callback.onConnectionClosed();
            }
        });
    }

    @Override
    public void onLoginSuccessful(final String displayName, final AuthParams authParams) {
        Log.d(TAG, "UnityVoxImplantClientImp: onLoggedIn(displayName = " + displayName + ")");
        runOnUIThread(new Runnable() {
            @Override
            public void run() {
                VoxImplantCallback.LoginTokens tokens = new VoxImplantCallback.LoginTokens(
                        authParams.getAccessTokenTimeExpired(),
                        authParams.getAccessToken(),
                        authParams.getRefreshTokenTimeExpired(),
                        authParams.getRefreshToken());
                callback.onLoginSuccessful(displayName, tokens);
            }
        });
    }

    @Override
    public void onLoginFailed(final LoginError reason) {
        runOnUIThread(new Runnable() {
            @Override
            public void run() {
                callback.onLoginFailed(convertLoginFailureReason(reason));
            }
        });
    }

    @Override
    public void onIncomingCall(final ICall call, final Map<String, String> headers) {
        Log.d(TAG, "UnityVoxImplantClientImp: onIncomingCall(call = " + call + ", headers = " + headers + ")");
        final UnityVoxImplantClientImp self = this;
        runOnUIThread(new Runnable() {
            @Override
            public void run() {
                String callId = call.getCallId();
                calls.put(callId, call);
                endpointMap.put(call.getEndpoints().get(0).getEndpointId(), call.getEndpoints().get(0));
                call.addCallListener(self);
                endpointMap.get(callId).setEndpointListener(self);
                callback.onIncomingCall(callId,
                        call.getEndpoints().get(0).getUserName(),
                        call.getEndpoints().get(0).getUserDisplayName(),
                        call.isVideoEnabled(),
                        headers);
            }
        });
    }

    @Override
    public void onOneTimeKeyGenerated(String key) {
        final String key_ = key;
        runOnUIThread(new Runnable() {
            @Override
            public void run() {
                callback.onOneTimeKeyGenerated(key_);
            }
        });
    }

    @Override
    public void onRefreshTokenFailed(final LoginError reason) {
        runOnUIThread(new Runnable() {
            @Override
            public void run() {
                callback.onRefreshTokenFailed(convertRefreshFailureReason(reason));
            }
        });
    }

    @Override
    public void onRefreshTokenSuccess(final AuthParams authParams) {
        final VoxImplantCallback.LoginTokens tokens = new VoxImplantCallback.LoginTokens(
                authParams.getAccessTokenTimeExpired(),
                authParams.getAccessToken(),
                authParams.getRefreshTokenTimeExpired(),
                authParams.getRefreshToken());
        runOnUIThread(new Runnable() {
            @Override
            public void run() {
                callback.onRefreshTokenSuccess(tokens);
            }
        });
    }

    @Override
    public void onCallConnected(ICall call, Map<String, String> headers) {
        Log.d(TAG,
                "UnityVoxImplantClientImp: onCallConnected(call = " + call.getCallId() + " headers = " + headers + ")");
        final ICall call_ = call;
        final Map<String, String> headers_ = headers;
        Timer timer = new Timer();
        callStatistics.put(call.getCallId(), timer);
        timer.schedule(new TimerTask() {
            @Override
            public void run() {
                CallStatistic stats = call_.getCallStatistic();
                if (stats != null && endpointMap.size() > 0) {
                    int maxPacketLossAudio = Math.max(stats.audioSendStatistics.packetLossCurrent,
                            stats.endpointStatistics.get(endpointMap.get(call_.getCallId())).audioReceiveStatistics.packetLossCurrent);
                    int maxPacketLossVideo = Math.max(stats.videoSendStatistics.packetLossCurrent,
                            stats.endpointStatistics.get(endpointMap.get(call_.getCallId())).videoReceiveStatistics.packetLossCurrent);
                    int maxPacketLoss = Math.max(maxPacketLossAudio, maxPacketLossVideo);
                    if (maxPacketLoss > 0) {
                        final VoxImplantCallback.NetworkInfo netInfo = new VoxImplantCallback.NetworkInfo();
                        netInfo.packetLoss = maxPacketLoss;
                        runOnUIThread(new Runnable() {
                            @Override
                            public void run() {
                                callback.onNetStatsReceived(call_.getCallId(), netInfo);
                            }
                        });
                    }
                }
            }
        }, 1000, 5000);
        runOnUIThread(new Runnable() {
            @Override
            public void run() {
                callback.onCallConnected(call_.getCallId(), headers_);
            }
        });
    }

    @Override
    public void onCallDisconnected(final ICall call, final Map<String, String> headers, final boolean answeredElsewhere) {
        Log.d(TAG,
                "UnityVoxImplantClientImp: onCallDisconnected(call = " + call.getCallId() + " headers = " + headers + " answeredElsewhere = " + answeredElsewhere + ")");
        call.removeCallListener(this);
        checkAndRemoveRenderers(call);
        calls.remove(call.getCallId());
        if (callStatistics.get(call.getCallId()) != null) {
            callStatistics.get(call.getCallId()).cancel();
            callStatistics.remove(call.getCallId());
        }
        //endpoint id = createCall id for 2 party calls.
        endpointMap.get(call.getCallId()).setEndpointListener(null);
        endpointMap.remove(call.getCallId());
        runOnUIThread(new Runnable() {
            @Override
            public void run() {
                callback.onCallDisconnected(call.getCallId(), headers, answeredElsewhere);
            }
        });
    }

    @Override
    public void onCallRinging(ICall call, final Map<String, String> headers) {
        Log.d(TAG, "UnityVoxImplantClientImp: onCallRinging(call = " + call + ")");
        final ICall call_ = call;
        runOnUIThread(new Runnable() {
            @Override
            public void run() {
                callback.onCallRinging(call_.getCallId(), headers);
            }
        });
    }

    @Override
    public void onCallFailed(ICall call, int code, String description, Map<String, String> headers) {
        Log.d(TAG, "UnityVoxImplantClientImp: onCallFailed(call = " + call.getCallId() +
                " code = " + code +
                " description = " + description +
                " headers = " + headers + ")");
        final ICall call_ = call;
        final int code_ = code;
        final String description_ = description;
        final Map<String, String> headers_ = headers;

        //clean up previously allocated resources
        checkAndRemoveRenderers(call_);
        calls.remove(call_.getCallId());
        //endpoint id = createCall id for 2 party calls.
        endpointMap.remove(call.getCallId());
        runOnUIThread(new Runnable() {
            @Override
            public void run() {
                callback.onCallFailed(call_.getCallId(), code_, description_, headers_);
            }
        });
    }

    @Override
    public void onCallAudioStarted(ICall call) {
        Log.d(TAG, "UnityVoxImplantClientImp: onCallAudioStarted(call = " + call.getCallId() + ")");
        final ICall call_ = call;
        runOnUIThread(new Runnable() {
            @Override
            public void run() {
                callback.onCallAudioStarted(call_.getCallId());
            }
        });
    }

    @Override
    public void onSIPInfoReceived(ICall call, String type, String content, Map<String, String> headers) {
        Log.d(TAG, "UnityVoxImplantClientImp: onSIPInfoReceived(call = " + call.getCallId() + ")");
        final ICall call_ = call;
        final String type_ = type;
        final String content_ = content;
        final Map<String, String> headers_ = headers;
        runOnUIThread(new Runnable() {
            @Override
            public void run() {
                callback.onSIPInfoReceivedInCall(call_.getCallId(), type_, content_, headers_);
            }
        });
    }

    @Override
    public void onMessageReceived(ICall call, String text) {
        Log.d(TAG, "UnityVoxImplantClientImp: onMessageReceived(call = " + call.getCallId() + ")");
        final ICall call_ = call;
        final String text_ = text;
        runOnUIThread(new Runnable() {
            @Override
            public void run() {
                callback.onMessageReceivedInCall(call_.getCallId(), text_);
            }
        });
    }

    @Override
    public void onLocalVideoStreamAdded(ICall call, IVideoStream videoStream) {
        Log.i(TAG, "UnityVoxImplantClientImp: onLocalVideoStreamAdded");
        localVideoStreams.put(call.getCallId(), videoStream);
        if (localView != null) {
            localRenders.put(call.getCallId(), localView);
            localView = null;
        }
        for (Map.Entry<String, VideoRenderer.Callbacks> entry : localRenders.entrySet()) {
            videoStream.addVideoRenderer(entry.getValue(), RenderScaleType.SCALE_FILL);
        }
        Log.v(TAG,
                "UnityVoxImplantClientImp: onLocalVideoStreamAdded: localView = " + localView + ", localRenderers = " + localRenders.toString());
    }

    @Override
    public void onLocalVideoStreamRemoved(ICall call, IVideoStream videoStream) {
        Log.i(TAG, "UnityVoxImplantClientImp: onLocalVideoStreamRemoved");
        localVideoStreams.remove(call.getCallId());
        for (Map.Entry<String, VideoRenderer.Callbacks> entry : localRenders.entrySet()) {
            videoStream.removeVideoRenderer(entry.getValue());
        }
        Log.v(TAG,
                "UnityVoxImplantClientImp: onLocalVideoStreamRemoved: localView = " + localView + ", localRenderers = " + localRenders.toString());
    }

    @Override
    public void onRemoteVideoStreamAdded(IEndpoint endpoint, IVideoStream videoStream) {
        Log.i(TAG, "UnityVoxImplantClientImp: onRemoteVideoStreamAdded");
        remoteVideoStreams.put(endpoint.getEndpointId(), videoStream);
        if (remoteView != null) {
            remoteRenders.put(endpoint.getEndpointId(), remoteView);
            remoteView = null;
        }
        for (Map.Entry<String, VideoRenderer.Callbacks> entry : remoteRenders.entrySet()) {
            videoStream.addVideoRenderer(entry.getValue(), RenderScaleType.SCALE_FILL);
        }
        Log.v(TAG,
                "UnityVoxImplantClientImp: onRemoteVideoStreamAdded: remoteView = " + remoteView + ", remoteRenders = " + remoteRenders.toString());
    }

    @Override
    public void onRemoteVideoStreamRemoved(IEndpoint endpoint, IVideoStream videoStream) {
        Log.i(TAG, "UnityVoxImplantClientImp: onRemoteVideoStreamRemoved");
        remoteVideoStreams.remove(endpoint.getEndpointId());
        for (Map.Entry<String, VideoRenderer.Callbacks> entry : remoteRenders.entrySet()) {
            videoStream.removeVideoRenderer(entry.getValue());
        }
        Log.v(TAG,
                "UnityVoxImplantClientImp: onRemoteVideoStreamRemoved: remoteView = " + remoteView + ", remoteRenders = " + remoteRenders.toString());
    }

    void handlePushNotification(Map<String, String> notification) {
        voxClient.handlePushNotification(notification);
    }

    void registerForPushNotifications(String pushRegistrationToken) {
        voxClient.registerForPushNotifications(pushRegistrationToken);
    }

    void unregisterFromPushNotifications(String pushRegistrationToken) {
        voxClient.unregisterFromPushNotifications(pushRegistrationToken);
    }

    ICustomVideoSource createCustomVideoSource(String callId) {
        ICall call = calls.get(callId);
        if (call == null && !calls.isEmpty()) {
            call = calls.entrySet().iterator().next().getValue();
        }

        if (call != null) {
            return call.createCustomVideoSource();
        }

        return null;
    }

    void useCustomVideoSource(String callId, final ICustomVideoSource customVideoSource) {
        ICall call = calls.get(callId);
        if (call == null && !calls.isEmpty()) {
            call = calls.entrySet().iterator().next().getValue();
        }

        if (call != null) {
            Log.v(TAG, "UnityVoxImplantClientImp: useCustomVideoSource");
            call.useCustomVideoSource(customVideoSource);
        }
    }
}
