package com.voximplant.sdk;

import android.Manifest;
import android.app.DialogFragment;
import android.content.Context;
import android.content.pm.PackageManager;
import android.hardware.Camera;
import android.os.Build;
import android.os.Handler;
import android.os.Looper;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.PermissionChecker;
import android.util.Log;

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
import java.util.Map;

public class AVoImClient implements VoxImplantCallback {

    public VoxImplantClient client;
    private Context cntx;
    private UnityPlayer unityPlayer;
    private Map<String, Call> mCallsMap;
    private String sdkObjName = "SDK";
    private DialogFragment mVideoLocalDialog;
    private DialogFragment mVideoRemoteDialog;
    private ViewLayoutSize mSizeLocal = new ViewLayoutSize(-1,-1,-1,-1);
    private ViewLayoutSize mSizeRemote = new ViewLayoutSize(-1,-1,-1,-1);
    private String TAG = this.getClass().getSimpleName();

    public class ViewLayoutSize {
        public int x_pos;
        public int y_pos;
        public int width;
        public int height;

        public ViewLayoutSize(int pX, int pY, int pW, int pH){
            x_pos = pX;
            y_pos = pY;
            width = pW;
            height = pH;
        }

        public int[] getArray(){
            return new int[]{x_pos, y_pos, width, height};
        }
    }

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
        public PairKeyValue[] headers;

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
            String[] ListPermission = new String[]{   Manifest.permission.RECORD_AUDIO,
                    Manifest.permission.MODIFY_AUDIO_SETTINGS,
                    Manifest.permission.INTERNET,
                    Manifest.permission.CAMERA,
                    Manifest.permission.ACCESS_NETWORK_STATE};

            if (!isPermissionGranted(ListPermission)) {
                ActivityCompat.requestPermissions(UnityPlayer.currentActivity, ListPermission, 1);
            }
            else {
                Init();
            }
        }
        else {
            Init();
        }
    }

    private boolean isPermissionGranted(String[] pListPermissions){
        for (String permission: pListPermissions) {
            if (checkSelfPermission(cntx, permission) != PackageManager.PERMISSION_GRANTED) {
                return false;
            }
        }
        return true;
    }

    private static int checkSelfPermission(Context context, String permission) {
        if (permission == null) {
            throw new IllegalArgumentException("permission is null");
        }

        return context.checkPermission(permission, android.os.Process.myPid(), android.os.Process.myUid());
    }

    public void Init(){
        Log.d("VOXIMPLANT", "Start init");
        client = VoxImplantClient.instance();
        client.setAndroidContext(cntx);
        client.setCallback(this);
        client.setCamera(Camera.CameraInfo.CAMERA_FACING_FRONT);
        client.setCameraResolution(320, 320);
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
        client.startCall(callId, GetMapFromList(param.headers));
        mCallsMap.put(callId, new Call(callId, false, param.videoCall));
        onStartCall(callId);
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
    public void setLocalSize(String pSizejson){
        mSizeLocal = GetJsonObj(pSizejson, ViewLayoutSize.class);
    }
    public void setRemoteSize(String pSizejson){
        mSizeRemote = GetJsonObj(pSizejson, ViewLayoutSize.class);
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
        if (mCallsMap.get(s).video) {
            mVideoLocalDialog = CallDialogFragment.showDialog(UnityPlayer.currentActivity, mSizeLocal.getArray(), true);
            mVideoRemoteDialog = CallDialogFragment.showDialog(UnityPlayer.currentActivity, mSizeRemote.getArray(), false);
        }
    }

    public void onStartCall(String s) {
        unityPlayer.UnitySendMessage(sdkObjName,"faonOnStartCall", s);
        mCallsMap.put(s, new Call(s, false, true));
    }

    @Override
    public void onCallDisconnected(String s, Map<String, String> map) {
        unityPlayer.UnitySendMessage(sdkObjName,"faonCallDisconnected",GetParamListToString(new ArrayList<Object>(Arrays.asList(s,map))));
        mCallsMap.remove(s);
        mVideoLocalDialog.dismiss();
        mVideoRemoteDialog.dismiss();
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
