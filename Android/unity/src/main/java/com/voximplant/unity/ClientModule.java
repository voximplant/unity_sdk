/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
package com.voximplant.unity;

import android.content.Context;
import android.util.Log;

import com.unity3d.player.UnityPlayer;
import com.voximplant.sdk.Voximplant;
import com.voximplant.sdk.call.CallSettings;
import com.voximplant.sdk.call.ICall;
import com.voximplant.sdk.call.VideoCodec;
import com.voximplant.sdk.call.VideoFlags;
import com.voximplant.sdk.client.AuthParams;
import com.voximplant.sdk.client.ClientConfig;
import com.voximplant.sdk.client.ClientState;
import com.voximplant.sdk.client.IClient;
import com.voximplant.sdk.client.IClientIncomingCallListener;
import com.voximplant.sdk.client.IClientLoginListener;
import com.voximplant.sdk.client.IClientSessionListener;
import com.voximplant.sdk.client.LoginError;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.Executors;

public class ClientModule implements IClientSessionListener, IClientLoginListener, IClientIncomingCallListener {
    private final Context mContext;
    private IClient mClient;
    private Map<String, CallWrapper> mIncomingCalls;

    public ClientModule() {
        mContext = UnityPlayer.currentActivity.getApplicationContext();
    }

    @CalledByUnity
    public void init(String clientConfigJson) {
        Voximplant.subVersion = "unity-" + mContext.getString(R.string.voximplant_unity_sdk_version);
        Voximplant.getCameraManager(mContext);

        ClientConfig clientConfig = new ClientConfig();

        UnityConfig config = Emitter.getGson().fromJson(clientConfigJson, UnityConfig.class);
        clientConfig.enableDebugLogging = config.isEnableDebugLogging();
        clientConfig.enableLogcatLogging = config.isEnableLogcatLogging();
        clientConfig.requestAudioFocusMode = config.getAudioFocusMode();

        clientConfig.eglBase = GLContextHelper.getRootEglBase();

        Log.d("BRIDGE", "Setting GLES context: " + clientConfig.eglBase.getEglBaseContext().getNativeEglContext());

        mClient = Voximplant.getClientInstance(Executors.newSingleThreadExecutor(), mContext, clientConfig);
        mClient.setClientSessionListener(this);
        mClient.setClientLoginListener(this);
        mClient.setClientIncomingCallListener(this);

        mIncomingCalls = new HashMap<>();
    }

    // region IClient
    @CalledByUnity
    public void connect(boolean connectivityCheck, String[] servers) {
        if (mClient == null) return;

        List<String> gateways = servers != null ? Arrays.asList(servers) : new ArrayList<>();

        mClient.connect(connectivityCheck, gateways);
    }

    @CalledByUnity
    public void login(String user, String password) {
        if (mClient == null) return;

        mClient.login(user, password);
    }

    @CalledByUnity
    public void loginWithAccessToken(String user, String token) {
        if (mClient == null) return;

        mClient.loginWithAccessToken(user, token);
    }

    @CalledByUnity
    public void loginWithOneTimeKey(String user, String hash) {
        if (mClient == null) return;

        mClient.loginWithOneTimeKey(user, hash);
    }

    @CalledByUnity
    public void refreshToken(String user, String refreshToken) {
        if (mClient == null) return;

        mClient.refreshToken(user, refreshToken);
    }

    @CalledByUnity
    public void requestOneTimeKey(String user) {
        if (mClient == null) return;

        mClient.requestOneTimeKey(user);
    }

    @CalledByUnity
    public void disconnect() {
        if (mClient == null) return;

        mClient.disconnect();
    }

    @CalledByUnity
    public String getClientState() {
        if (mClient == null) return ClientState.DISCONNECTED.toString();

        return mClient.getClientState().toString();
    }

    @CalledByUnity
    public CallWrapper call(String user, boolean receiveVideo, boolean sendVideo, String videoCodec, String customData, String headers) {
        CallSettings callSettings = new CallSettings();
        callSettings.videoFlags = new VideoFlags(receiveVideo, sendVideo);
        try {
            callSettings.preferredVideoCodec = VideoCodec.valueOf(videoCodec.toUpperCase());
        } catch (IllegalArgumentException exc) {
            callSettings.preferredVideoCodec = VideoCodec.AUTO;
        }
        callSettings.customData = customData;
        @SuppressWarnings("unchecked") Map<String, String> hdrs = Emitter.getGson().fromJson(headers, Map.class);
        callSettings.extraHeaders = hdrs;

        ICall call = mClient.call(user, callSettings);
        return call != null ? new CallWrapper(call) : null;
    }

    @CalledByUnity
    public CallWrapper callConference(String conference, boolean receiveVideo, boolean sendVideo, String videoCodec, String customData, String headers) {
        CallSettings callSettings = new CallSettings();
        callSettings.videoFlags = new VideoFlags(receiveVideo, sendVideo);
        try {
            callSettings.preferredVideoCodec = VideoCodec.valueOf(videoCodec.toUpperCase());
        } catch (IllegalArgumentException exc) {
            callSettings.preferredVideoCodec = VideoCodec.AUTO;
        }
        callSettings.customData = customData;
        @SuppressWarnings("unchecked") Map<String, String> hdrs = Emitter.getGson().fromJson(headers, Map.class);
        callSettings.extraHeaders = hdrs;

        ICall call = mClient.callConference(conference, callSettings);
        return call != null ? new CallWrapper(call) : null;
    }

    // endregion

    // region IClientSessionListener
    @Override
    public void onConnectionEstablished() {
        Emitter.sendClientMessage("Connected");
    }

    @Override
    public void onConnectionFailed(String error) {
        Emitter.sendClientMessage("ConnectionFailed", ErrorHelper.payloadForError(error));
    }

    @Override
    public void onConnectionClosed() {
        Emitter.sendClientMessage("Disconnected");
    }
    // endregion

    // region IClientLoginListener
    @Override
    public void onLoginSuccessful(String displayName, AuthParams authParams) {
        Map<String, Object> loginParams = new HashMap<>();
        loginParams.put("displayName", displayName);
        loginParams.put("authParams", authParams);
        Emitter.sendClientMessage("LoginSuccess", loginParams);
    }

    @Override
    public void onLoginFailed(LoginError loginError) {
        Emitter.sendClientMessage("LoginFailed", ErrorHelper.payloadForError(loginError));
    }

    @Override
    public void onRefreshTokenFailed(LoginError loginError) {
        Emitter.sendClientMessage("RefreshTokenFailed", ErrorHelper.payloadForError(loginError));
    }

    @Override
    public void onRefreshTokenSuccess(AuthParams authParams) {
        Emitter.sendClientMessage("RefreshTokenSuccess", Collections.singletonMap("authParams", authParams));
    }

    @Override
    public void onOneTimeKeyGenerated(String token) {
        Emitter.sendClientMessage("OneTimeKeyGenerated", Collections.singletonMap("oneTimeKey", token));
    }
    // endregion

    // region IClientIncomingCallListener
    @Override
    public void onIncomingCall(ICall call, boolean hasIncomingVideo, Map<String, String> headers) {
        mIncomingCalls.put(call.getCallId(), new CallWrapper(call));

        Map<String, Object> payload = new HashMap<>();
        payload.put("callId", call.getCallId());
        payload.put("incomingVideo", hasIncomingVideo);
        if (headers != null) {
            payload.put("headers", headers);
        }
        Emitter.sendClientMessage("IncomingCall", payload);
    }
    // endregion

    public CallWrapper getIncomingCall(String callId) {
        return mIncomingCalls.remove(callId);
    }
}
