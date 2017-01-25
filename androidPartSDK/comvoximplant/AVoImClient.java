package com.voximplant.sdk;

import android.Manifest;
import android.app.Dialog;
import android.content.Context;
import android.content.DialogInterface;
import android.graphics.Color;
import android.graphics.drawable.ColorDrawable;
import android.hardware.Camera;
import android.opengl.GLSurfaceView;
import android.os.Build;
import android.os.Handler;
import android.os.Looper;
import android.support.v4.app.ActivityCompat;
import android.util.Log;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.widget.Button;
import android.widget.FrameLayout;
import android.widget.RelativeLayout;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.unity3d.player.UnityPlayer;
import com.zingaya.voximplant.VoxImplantCallback;
import com.zingaya.voximplant.VoxImplantClient;
import com.zingaya.voximplant.VoxImplantClient.LoginFailureReason;

import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;


public class AVoImClient implements VoxImplantCallback {

    public VoxImplantClient client;
    private Context cntx;
    private UnityPlayer unityPlayer;
    private Map<String, Call> mCallsMap;
    private String sdkObjName = "SDK";
    private Dialog mCurVideoDialog;
    private String TAG = this.getClass().getSimpleName();

    public class LoginClassParam    {
        public String login;
        public String pass;
        public LoginClassParam(String pName, String pPass)
        {
            login = pName;
            pass = pPass;
        }
    }
    public class LoginOneTimeKeyClassParam    {
        public String name;
        public String hash;
        public LoginOneTimeKeyClassParam(String pName, String pHash)
        {
            name = pName;
            hash = pHash;
        }
    }
    public class CallClassParam    {
        public String userCall;
        public Boolean videoCall;
        public String customData;

        public CallClassParam(String pCallUser, Boolean pVideoCall, String pCustomData)
        {
            userCall = pCallUser;
            videoCall = pVideoCall;
            customData = pCustomData;
        }
    }
    public class DTFMClassParam    {
        public String callId;
        public int digit;
        public DTFMClassParam(String pCallId, int pDigit)        {
            callId = pCallId;
            digit = pDigit;
        }
    }
    public class InfoClassParam    {
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
    public class SendMessageClassParam {
        public String callId;
        public String text;
        public PairKeyValue[] headers;
        public SendMessageClassParam(String pCallId, String pMsg) {
            callId = pCallId;
            text = pMsg;
        }
    }
    public class CameraResolutionClassParam {
        public int width;
        public int height;
        public CameraResolutionClassParam(int pWidth, int pHeight)
        {
            width = pWidth;
            height = pHeight;
        }
    }
    public class BoolClassParam {
        public Boolean value;
        public BoolClassParam(Boolean pValue)
        {
            value = pValue;
        }
    }
    public class StringClassParam {
        public String value;
        public StringClassParam(String pValue)
        {
            value = pValue;
        }
    }
    public class PairKeyValue    {
        public String key;
        public String value;
        public PairKeyValue(String pKey, String pValue)
        {
            key = pKey;
            value = pValue;
        }
    }

    public AVoImClient(Context pCntx) {
        cntx = pCntx;
        mCallsMap = new HashMap<String, Call>();
        Log.d("VOXIMPLANT", "Create instance");
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            Log.d("VOXIMPLANT", "Request permissions");
            ActivityCompat.requestPermissions(UnityPlayer.currentActivity,
                    new String[]{   Manifest.permission.RECORD_AUDIO,
                            Manifest.permission.MODIFY_AUDIO_SETTINGS,
                            Manifest.permission.INTERNET,
                            Manifest.permission.CAMERA,
                            Manifest.permission.ACCESS_NETWORK_STATE},1);
        }
        else {
            Log.d("VOXIMPLANT", "Init");
            Init();
        }
    }

    public void Init(){
        Log.d("VOXIMPLANT", "Start init");
        client = VoxImplantClient.instance();
        client.setAndroidContext(cntx);
        client.setCallback(this);
        client.setCamera(Camera.CameraInfo.CAMERA_FACING_FRONT);
        client.setCameraResolution(320, 240);
        Log.d("VOXIMPLANT", "End init");
    }

    public void closeConnection(){
        client.closeConnection();
    }
    public void connect(){
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
    public String call(String p){
        CallClassParam param = GetJsonObj(p, CallClassParam.class);
        String callId = client.createCall(param.userCall, param.videoCall, param.customData);
        Map<String, String> headers = new HashMap<String, String>();
        client.startCall(callId, headers);
        mCallsMap.put(callId, new Call(callId, false, param.videoCall));
        return callId;
    }
    public void answer(String pCallId){
        client.answerCall(pCallId);
    }
    public void declineCall(String pCallId){
        client.declineCall(pCallId);
    }
    public void hangup(String pCallId){
        client.disconnectCall(pCallId);
    }
    public void setMute(String p){
        client.setMute(((BoolClassParam)GetJsonObj(p,BoolClassParam.class)).value);
    }
    public void sendVideo(String p){
        client.sendVideo(((BoolClassParam)GetJsonObj(p,BoolClassParam.class)).value);
    }
    public void setCamera(String p){
        client.setCamera(Integer.parseInt(p));
    }
    public void disableTls()
    {
        client.disableTLS();
    }
    public void disconnectCall(String p) {
        StringClassParam param = GetJsonObj(p, StringClassParam.class);
        client.disconnectCall(param.value);
    }
    public void enableDebugLogging()
    {
        client.enableDebugLogging();
    }
    public void loginUsingOneTimeKey(String pLogin) {
        LoginOneTimeKeyClassParam param = GetJsonObj(pLogin, LoginOneTimeKeyClassParam.class);
        client.loginUsingOneTimeKey(param.name, param.hash);
    }
    public void requestOneTimeKey(String pName)
    {
        client.requestOneTimeKey(pName);
    }
    public void sendDTMF(String pParam) {
        DTFMClassParam param = GetJsonObj(pParam, DTFMClassParam.class);
        client.sendDTMF(param.callId, param.digit);
    }
    public void sendInfo(String pParam) {
        InfoClassParam param = GetJsonObj(pParam, InfoClassParam.class);
        if (param.headers.equals(null))
            client.sendInfo(param.callId, param.mimeType, param.content);
        else
            client.sendInfo(param.callId, param.mimeType, param.content, GetMapFromList(param.headers));
    }
    public void sendMessage(String pParam) {
        SendMessageClassParam param = GetJsonObj(pParam, SendMessageClassParam.class);
        if (param.headers.equals(null))
            client.sendMessage(param.callId, param.text);
        else
            client.sendMessage(param.callId, param.text, GetMapFromList(param.headers));
    }
    public void setCameraResolution(String pParam) {
        CameraResolutionClassParam param = GetJsonObj(pParam, CameraResolutionClassParam.class);
        client.setCameraResolution(param.width, param.height);
    }
    public void setUseLoudspeaker(String pUseLoudSpeaker) {
        BoolClassParam param = GetJsonObj(pUseLoudSpeaker, BoolClassParam.class);
        client.setUseLoudspeaker(param.value);
    }

    @Override
    public void onLoginSuccessful(String s) {
        unityPlayer.UnitySendMessage(sdkObjName,"faonLoginSuccessful", GetParamListToString(new ArrayList<Object>(Arrays.asList(s))));
    }
    @Override
    public void onLoginFailed(LoginFailureReason loginFailureReason) {
        unityPlayer.UnitySendMessage(sdkObjName,"faonLoginFailed", GetParamListToString(new ArrayList<Object>(Arrays.asList(loginFailureReason))));
    }
    @Override
    public void onOneTimeKeyGenerated(String s) {
        unityPlayer.UnitySendMessage(sdkObjName,"faonOneTimeKeyGenerated", GetParamListToString(new ArrayList<Object>(Arrays.asList(s))));
    }

    @Override
    public void onConnectionSuccessful() {
        unityPlayer.UnitySendMessage(sdkObjName,"faonConnectionSuccessful","");
    }

    @Override
    public void onConnectionClosed() {
        unityPlayer.UnitySendMessage(sdkObjName,"faonConnectionClosed","");
    }

    @Override
    public void onConnectionFailedWithError(String s) {
        unityPlayer.UnitySendMessage(sdkObjName,"faonConnectionFailedWithError",GetParamListToString(new ArrayList<Object>(Arrays.asList(s))));
    }

    @Override
    public void onCallConnected(final String s, Map<String, String> map) {
        unityPlayer.UnitySendMessage(sdkObjName,"faonCallConnected",GetParamListToString(new ArrayList<Object>(Arrays.asList(s,map))));
        Log.d(TAG, "before handler if");
        if (mCallsMap.get(s).video) {
            Log.d(TAG, "if (_activeCall.video) ");
            new Handler(Looper.getMainLooper()).post(new Runnable() {
                @Override
                public void run() {
                    Log.d(TAG, " public void run() ");
                    mCurVideoDialog = showDialog(s);
                }
            });
        }
        Log.d(TAG, "after handler if");
    }


    private GLSurfaceView mVideo;

    private Dialog showDialog(final String pCallId){
        FrameLayout frameLayout = new FrameLayout(cntx);
        frameLayout.setLayoutParams(new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT));
        frameLayout.setBackgroundColor(Color.WHITE);
        Log.d(TAG, "Create button");
        Button btnHungup = new Button(cntx);
        btnHungup.setText("Hungup");
        btnHungup.setGravity(Gravity.BOTTOM);
        btnHungup.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                hangup(pCallId);
            }
        });

        FrameLayout.LayoutParams params = new FrameLayout.LayoutParams(
                FrameLayout.LayoutParams.WRAP_CONTENT,
                FrameLayout.LayoutParams.WRAP_CONTENT);
        params.setMargins(10,10,10,10);
        params.gravity = Gravity.CENTER_HORIZONTAL;
        frameLayout.addView(btnHungup, params);

        Log.d(TAG, "Create GL surface");
        if(mVideo == null) {
            mVideo = new GLSurfaceView(cntx);
            Log.d(TAG, "Set remote view");
            VoxImplantClient.instance().setRemoteView(mVideo);
            VoxImplantClient.instance().sendVideo(true);
            mVideo.setLayoutParams(new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT));
            frameLayout.addView(mVideo);
        }
        mVideo.requestLayout();

        Log.d(TAG, "Create Dialog");
        Dialog dialog = new Dialog(this.cntx);
        dialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
        dialog.getWindow().setBackgroundDrawable(new ColorDrawable(android.graphics.Color.TRANSPARENT));
        dialog.setOnDismissListener(new DialogInterface.OnDismissListener() {
            @Override
            public void onDismiss(DialogInterface dialogInterface) {
                //nothing;
            }
        });
        dialog.setCanceledOnTouchOutside(false);
        dialog.addContentView(frameLayout, new RelativeLayout.LayoutParams(
                ViewGroup.LayoutParams.WRAP_CONTENT,
                ViewGroup.LayoutParams.WRAP_CONTENT));
        Log.d(TAG, "Show Dialog");
        dialog.show();
        Log.d(TAG, "return dialog");
        return dialog;
    }

    @Override
    public void onCallDisconnected(String s, Map<String, String> map) {
        unityPlayer.UnitySendMessage(sdkObjName,"faonCallDisconnected",GetParamListToString(new ArrayList<Object>(Arrays.asList(s,map))));

        // Close active video dialog
        if(mCurVideoDialog != null) {
            mCurVideoDialog.dismiss();
            mCurVideoDialog = null;
            VoxImplantClient.instance().setRemoteView(null);
            VoxImplantClient.instance().sendVideo(false);
        }

    }
    @Override
    public void onCallRinging(String s, Map<String, String> map) {
        unityPlayer.UnitySendMessage(sdkObjName,"faonCallRinging",GetParamListToString(new ArrayList<Object>(Arrays.asList(s,map))));
    }
    @Override
    public void onCallFailed(String s, int i, String s1, Map<String, String> map) {
        unityPlayer.UnitySendMessage(sdkObjName,"faonCallFailed",GetParamListToString(new ArrayList<Object>(Arrays.asList(s,i,s1,map))));
    }
    @Override
    public void onCallAudioStarted(String s) {
        unityPlayer.UnitySendMessage(sdkObjName,"faonCallAudioStarted",GetParamListToString(new ArrayList<Object>(Arrays.asList(s))));
    }
    @Override
    public void onIncomingCall(String s, String s1, String s2, boolean b, Map<String, String> map) {
        unityPlayer.UnitySendMessage(sdkObjName,"faonIncomingCall",GetParamListToString(new ArrayList<Object>(Arrays.asList(s,s1,s2,b,map))));
        mCallsMap.put(s, new Call(s, true, b));
    }
    @Override
    public void onSIPInfoReceivedInCall(String callId, String type, String content, Map<String, String> headers) {
        unityPlayer.UnitySendMessage(sdkObjName,"faonSIPInfoReceivedInCall",GetParamListToString(new ArrayList<Object>(Arrays.asList(callId,type,content,headers))));
    }
    @Override
    public void onMessageReceivedInCall(String s, String s1, Map<String, String> map) {
        unityPlayer.UnitySendMessage(sdkObjName,"faonMessageReceivedInCall",GetParamListToString(new ArrayList<Object>(Arrays.asList(s,s1,map))));
    }
    @Override
    public void onNetStatsReceived(String s, NetworkInfo networkInfo) {
        unityPlayer.UnitySendMessage(sdkObjName,"faonNetStatsReceived",GetParamListToString(new ArrayList<Object>(Arrays.asList(s, networkInfo))));
    }

    private String GetParamListToString(ArrayList<Object> pList){
        GsonBuilder builder = new GsonBuilder();
        Gson gson = builder.create();
        return gson.toJson(pList);
    }
    private <T> T GetJsonObj(String pParam, Type pType){
        Gson gson = new Gson();
        return gson.fromJson(pParam, pType);
    }
    private Map<String, String> GetMapFromList(PairKeyValue[] pList){
        Map<String, String> res = new HashMap<String, String>();
        for (PairKeyValue pair: pList) {
            res.put(pair.key, pair.value);
        }
        return res;
    }
}
