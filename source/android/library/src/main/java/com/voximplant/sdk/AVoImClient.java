package com.voximplant.sdk;

import android.app.DialogFragment;
import android.app.Fragment;
import android.hardware.Camera;
import android.os.Handler;
import android.os.Looper;
import android.util.Log;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.unity3d.player.UnityPlayer;
import com.voximplant.sdk.hardware.ICustomVideoSource;
import com.zingaya.voximplant.UnityVoxImplantClient;
import com.zingaya.voximplant.VoxImplantCallback;
import com.zingaya.voximplant.VoxImplantClient;

import java.lang.reflect.Type;
import java.nio.ByteBuffer;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.Dictionary;
import java.util.HashMap;
import java.util.Hashtable;
import java.util.Map;

public class AVoImClient implements VoxImplantCallback {
    private static AVoImClient inst = null;

    public static AVoImClient instance() {
        if (inst == null) {
            inst = new AVoImClient();
        }

        return inst;
    }

    static {
        System.loadLibrary("c++_shared");
    }

    private UnityVoxImplantClient client;
    private String sdkObjName = "SDK";
    private String TAG = this.getClass().getSimpleName();

    public class LoginClassParam {
        public String login;
        public String pass;

        public LoginClassParam(String pName, String pPass) {
            login = pName;
            pass = pPass;
        }
    }

    public class LoginOneTimeKeyClassParam {
        public String name;
        public String hash;

        public LoginOneTimeKeyClassParam(String pName, String pHash) {
            name = pName;
            hash = pHash;
        }
    }

    public class CallClassParam {
        public String userCall;
        public Boolean videoCall;
        public String customData;

        public CallClassParam(String pCallUser, Boolean pVideoCall, String pCustomData) {
            userCall = pCallUser;
            videoCall = pVideoCall;
            customData = pCustomData;
        }
    }

    public class DTFMClassParam {
        public String callId;
        public int digit;

        public DTFMClassParam(String pCallId, int pDigit) {
            callId = pCallId;
            digit = pDigit;
        }
    }

    public class InfoClassParam {
        public String callId;
        public String mimeType;
        public String content;
        public PairKeyValue[] headers;

        public InfoClassParam(String pCallId, String pMimeType, String pContent) {
            callId = pCallId;
            mimeType = pMimeType;
            content = pContent;
        }
    }

    public class AnswerClassParam {
        public String callId;
        public PairKeyValue[] headers;

        public AnswerClassParam(String pCallId) {
            callId = pCallId;
        }
    }

    public class DeclineCallClassParam {
        public String callId;
        public PairKeyValue[] headers;

        public DeclineCallClassParam(String pCallId) {
            callId = pCallId;
        }
    }

    public class HangupClassParam {
        public String callId;
        public PairKeyValue[] headers;

        public HangupClassParam(String pCallId) {
            callId = pCallId;
        }
    }

    public class DisconnectCallClassParam {
        public String callId;
        public PairKeyValue[] headers;

        public DisconnectCallClassParam(String pCallId) {
            callId = pCallId;
        }
    }

    public class SendMessageClassParam {
        public String callId;
        public String text;

        public SendMessageClassParam(String pCallId, String pMsg) {
            callId = pCallId;
            text = pMsg;
        }
    }

    public class CameraResolutionClassParam {
        public int width;
        public int height;

        public CameraResolutionClassParam(int pWidth, int pHeight) {
            width = pWidth;
            height = pHeight;
        }
    }

    public class BoolClassParam {
        public Boolean value;

        public BoolClassParam(Boolean pValue) {
            value = pValue;
        }
    }

    public class StringClassParam {
        public String value;

        public StringClassParam(String pValue) {
            value = pValue;
        }
    }

    public class PairKeyValue {
        public String key;
        public String value;

        public PairKeyValue(String pKey, String pValue) {
            key = pKey;
            value = pValue;
        }
    }

    public AVoImClient() {
        Log.d(TAG, "Start init");

        client = UnityVoxImplantClient.instance();

        UnityVoxImplantClient.VoxImplantClientConfig config = new UnityVoxImplantClient.VoxImplantClientConfig();
        config.enableHWAcceleration = false;
        config.provideLocalFramesInByteBuffers = true;

        client.setAndroidContext(UnityPlayer.currentActivity.getApplicationContext(), config);
        client.setCallback(this);
        client.setCamera(Camera.CameraInfo.CAMERA_FACING_FRONT);
        client.setCameraResolution(320, 320);
        Log.d(TAG, "End init");
    }

    public void setSDKObjectName(String name) {
        this.sdkObjName = name;
    }

    public void closeConnection() {
        client.closeConnection();
    }

    public void connect() {
        client.connect();
    }

    public void login(final String p) {
        new Handler(Looper.getMainLooper()).post(new Runnable() {
            @Override
            public void run() {
                LoginClassParam param = GetJsonObj(p, LoginClassParam.class);
                client.login(param.login, param.pass);
            }
        });
    }

    public String createCall(String p) {
        CallClassParam param = GetJsonObj(p, CallClassParam.class);
        return client.createCall(param.userCall, param.videoCall, param.customData);
    }

    public void startCall(String callId, Map<String, String> headers) {
        client.startCall(callId, headers);
        onStartCall(callId);
    }

    public void answer(String pParam) {
        AnswerClassParam param = GetJsonObj(pParam, AnswerClassParam.class);
        if (param.headers == null)
            client.answerCall(param.callId);
        else
            client.answerCall(param.callId, GetMapFromList(param.headers));
    }

    public void declineCall(String pParam) {
        DeclineCallClassParam param = GetJsonObj(pParam, DeclineCallClassParam.class);
        if (param.headers == null)
            client.declineCall(param.callId);
        else
            client.declineCall(param.callId, GetMapFromList(param.headers));
    }

    public void hangup(String pParam) {
        HangupClassParam param = GetJsonObj(pParam, HangupClassParam.class);
        if (param.headers == null)
            client.disconnectCall(param.callId);
        else
            client.disconnectCall(param.callId, GetMapFromList(param.headers));
    }

    public void setMute(String pState) {
        client.setMute(((BoolClassParam) GetJsonObj(pState, BoolClassParam.class)).value);
    }

    public void sendVideo(String pState) {
        client.sendVideo(((BoolClassParam) GetJsonObj(pState, BoolClassParam.class)).value);
    }

    public void dismissByTag(String pTag) {
        Fragment prev = UnityPlayer.currentActivity.getFragmentManager().findFragmentByTag(pTag);
        if (prev != null) {
            ((DialogFragment) prev).dismiss();
        }
    }

    public void setCamera(String p) {
        client.setCamera(Integer.parseInt(p));
    }

    public void disableTls() {
        throw new RuntimeException("Not implemented");
    }

    public void disconnectCall(String pParam) {
        DisconnectCallClassParam param = GetJsonObj(pParam, DisconnectCallClassParam.class);
        if (param.headers == null)
            client.disconnectCall(param.callId);
        else
            client.disconnectCall(param.callId, GetMapFromList(param.headers));
    }

    public void enableDebugLogging() {

//        UnityVoxImplantClient.enableDebugLogging();

    }

    public void loginUsingOneTimeKey(String pLogin) {
        LoginOneTimeKeyClassParam param = GetJsonObj(pLogin, LoginOneTimeKeyClassParam.class);
        client.loginUsingOneTimeKey(param.name, param.hash);
    }

    public void requestOneTimeKey(String pName) {
        client.requestOneTimeKey(pName);
    }

    public void sendDTMF(String pParam) {
        DTFMClassParam param = GetJsonObj(pParam, DTFMClassParam.class);
        client.sendDTMF(param.callId, param.digit);
    }

    public void sendInfo(String pParam) {
        InfoClassParam param = GetJsonObj(pParam, InfoClassParam.class);
        if (param.headers == null)
            client.sendInfo(param.callId, param.mimeType, param.content);
        else
            client.sendInfo(param.callId, param.mimeType, param.content, GetMapFromList(param.headers));
    }

    public void sendMessage(String pParam) {
        SendMessageClassParam param = GetJsonObj(pParam, SendMessageClassParam.class);
        client.sendMessage(param.callId, param.text);
    }

    public void setCameraResolution(String pParam) {
        CameraResolutionClassParam param = GetJsonObj(pParam, CameraResolutionClassParam.class);
        client.setCameraResolution(param.width, param.height);
    }

    public void setUseLoudspeaker(String pUseLoudSpeaker) {
        BoolClassParam param = GetJsonObj(pUseLoudSpeaker, BoolClassParam.class);
        client.setUseLoudspeaker(param.value);
    }

    public enum VideoStream {
        VideoStreamIncoming,
        VideoStreamOutgoing;

        public static VideoStream fromInteger(int value) {
            value = Math.min(Math.max(0, value), VideoStream.values().length - 1);
            return VideoStream.values()[value];
        }
    }

    private native void renderBufferFrame(String callId,
                                          ByteBuffer yPlane, int yStride,
                                          ByteBuffer uPlane, int uStride,
                                          ByteBuffer vPlane, int vStride,
                                          int width, int height,
                                          int stream,
                                          int degrees);

    public void beginSendingVideoForStream(final String callId, final int stream) {
        VideoStream videoStream = VideoStream.fromInteger(stream);

        NativeVideoRenderer videoRenderer = new NativeVideoRenderer(new NativeVideoRenderer.NativeVideoRendererCallbacks() {
            @Override
            public void onBufferFrameRender(ByteBuffer[] planes, int[] strides, int width, int height, int degrees) {
                renderBufferFrame(
                        callId,
                        planes[0], strides[0],
                        planes[1], strides[1],
                        planes[2], strides[2],
                        width, height,
                        stream,
                        degrees);
            }
        });

        switch (videoStream) {
            case VideoStreamIncoming:
                client.setRemoteView(videoRenderer);
                break;
            case VideoStreamOutgoing:
                client.setLocalPreview(videoRenderer);
                break;
        }
    }

    public void reportNewNativeTexture(String callId, long textureId, long oglContext, int width, int height, int stream) {
        UnityPlayer.UnitySendMessage(sdkObjName,
                "fonNewNativeTexture",
                GetParamListToString(new ArrayList<Object>(Arrays.asList(callId, textureId, oglContext, width, height, stream))));
    }

    private Dictionary<String, RGBATextureVideoSource> videoSources = new Hashtable<>();
    private Dictionary<String, ICustomVideoSource> customVideoSources = new Hashtable<>();

    public void registerCallVideoStream(String callId, int width, int height) {
        Log.v(TAG, "registerCallVideoStream");
        ICustomVideoSource customVideoSource = customVideoSources.get(callId);
        if (customVideoSource == null) {
            customVideoSource = Voximplant.getCustomVideoSource();
            customVideoSources.put(callId, customVideoSource);
        }

        videoSources.put(callId, new RGBATextureVideoSource(width, height, customVideoSource));
        client.useCustomVideoSource(callId, customVideoSource);
    }

    public void callVideoStreamTextureUpdated(String callId, int textureId) {
        RGBATextureVideoSource videoSource = videoSources.get(callId);
        if (videoSource != null) {
            videoSource.SendFrame(textureId);
        }
    }

    public void unregisterCallVideoStream(String callId) {
        Log.v(TAG, "unregisterCallVideoStream " + callId);
        videoSources.remove(callId);
    }

    @Override
    public void onLoginSuccessful(String s, LoginTokens loginTokens) {
        UnityPlayer.UnitySendMessage(sdkObjName,
                "faonLoginSuccessful",
                GetParamListToString(new ArrayList<Object>(Collections.singletonList(s))));
    }

    @Override
    public void onRefreshTokenFailed(Integer integer) {

    }

    @Override
    public void onRefreshTokenSuccess(LoginTokens loginTokens) {

    }

    @Override
    public void onCallDisconnected(String callId, Map<String, String> map, boolean b) {
        customVideoSources.remove(callId);
        UnityPlayer.UnitySendMessage(sdkObjName,
                "faonCallDisconnected",
                GetParamListToString(new ArrayList<>(Arrays.asList(callId, map))));
    }

    @Override
    public void onMessageReceivedInCall(String s, String s1) {
        UnityPlayer.UnitySendMessage(sdkObjName,
                "faonMessageReceivedInCall",
                GetParamListToString(new ArrayList<Object>(Arrays.asList(s, s1))));
    }

    @Override
    public void onLoginFailed(VoxImplantClient.LoginFailureReason loginFailureReason) {
        UnityPlayer.UnitySendMessage(sdkObjName,
                "faonLoginFailed",
                GetParamListToString(new ArrayList<Object>(Collections.singletonList(loginFailureReason))));
    }

    @Override
    public void onOneTimeKeyGenerated(String s) {
        UnityPlayer.UnitySendMessage(sdkObjName,
                "faonOneTimeKeyGenerated",
                GetParamListToString(new ArrayList<Object>(Collections.singletonList(s))));
    }

    @Override
    public void onConnectionSuccessful() {
        UnityPlayer.UnitySendMessage(sdkObjName, "faonConnectionSuccessful", "");
    }

    @Override
    public void onConnectionClosed() {
        UnityPlayer.UnitySendMessage(sdkObjName, "faonConnectionClosed", "");
    }

    @Override
    public void onConnectionFailedWithError(String s) {
        UnityPlayer.UnitySendMessage(sdkObjName,
                "faonConnectionFailedWithError",
                GetParamListToString(new ArrayList<Object>(Collections.singletonList(s))));
    }

    @Override
    public void onCallConnected(final String s, Map<String, String> map) {
        UnityPlayer.UnitySendMessage(sdkObjName,
                "faonCallConnected",
                GetParamListToString(new ArrayList<>(Arrays.asList(s, map))));
    }

    public void onStartCall(String s) {
        if (s == null) {
            Log.e("VOXIMPLANT", "Null call Id in onStartCall");
            return;
        }
        UnityPlayer.UnitySendMessage(sdkObjName, "faonOnStartCall", s);
    }

    @Override
    public void onCallRinging(String s, Map<String, String> map) {
        UnityPlayer.UnitySendMessage(sdkObjName,
                "faonCallRinging",
                GetParamListToString(new ArrayList<>(Arrays.asList(s, map))));
    }

    @Override
    public void onCallFailed(String s, int i, String s1, Map<String, String> map) {
        UnityPlayer.UnitySendMessage(sdkObjName,
                "faonCallFailed",
                GetParamListToString(new ArrayList<>(Arrays.asList(s, i, s1, map))));
    }

    @Override
    public void onCallAudioStarted(String s) {
        UnityPlayer.UnitySendMessage(sdkObjName,
                "faonCallAudioStarted",
                GetParamListToString(new ArrayList<Object>(Collections.singletonList(s))));
    }

    @Override
    public void onIncomingCall(String s, String s1, String s2, boolean b, Map<String, String> map) {
        UnityPlayer.UnitySendMessage(sdkObjName,
                "faonIncomingCall",
                GetParamListToString(new ArrayList<>(Arrays.asList(s, s1, s2, b, map))));
    }

    @Override
    public void onSIPInfoReceivedInCall(String callId, String type, String content, Map<String, String> headers) {
        UnityPlayer.UnitySendMessage(sdkObjName,
                "faonSIPInfoReceivedInCall",
                GetParamListToString(new ArrayList<>(Arrays.asList(callId, type, content, headers))));
    }

    @Override
    public void onNetStatsReceived(String s, NetworkInfo networkInfo) {
        UnityPlayer.UnitySendMessage(sdkObjName,
                "faonNetStatsReceived",
                GetParamListToString(new ArrayList<>(Arrays.asList(s, networkInfo))));
    }

    private String GetParamListToString(ArrayList<Object> pList) {
        GsonBuilder builder = new GsonBuilder();
        Gson gson = builder.create();
        return gson.toJson(pList);
    }

    private <T> T GetJsonObj(String pParam, Type pType) {
        Gson gson = new Gson();
        return gson.fromJson(pParam, pType);
    }

    private Map<String, String> GetMapFromList(PairKeyValue[] pList) {
        Map<String, String> res = new HashMap<>();
        for (PairKeyValue pair : pList) {
            res.put(pair.key, pair.value);
        }
        return res;
    }
}
