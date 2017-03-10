using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Assertions;

namespace Voximplant
{
    internal sealed class iOSSDK : VoximplantSDK
    {
        public override void init(Action<bool> initCallback)
        {
            iosSDKinit(gameObject.name);
            initCallback(true);
        }

        public override void closeConnection()
        {
            iosSDKCloseConnection();
        }

        public override void connect()
        {
            iosSDKconnect();
        }

        public override void login(LoginClassParam pLogin)
        {
            iosSDKlogin(pLogin.login, pLogin.pass);
        }

        public override void call(CallClassParam pCall)
        {
            iosSDKstartCall(pCall.userCall, pCall.videoCall, pCall.customData,
                JsonUtility.ToJson(new PairKeyValueArray(pCall.headers)));
        }

        public override void answer(string pCallId, Dictionary<string, string> pHeader = null)
        {
            iosSDKanswerCall(pCallId, JsonUtility.ToJson(new PairKeyValueArray(Utils.GetDictionaryToArray(pHeader))));
        }

        public override void declineCall(string pCallId, Dictionary<string, string> pHeader = null)
        {
            iosSDKDecline(pCallId, JsonUtility.ToJson(new PairKeyValueArray(Utils.GetDictionaryToArray(pHeader))));
        }

        public override void hangup(string pCallId, Dictionary<string, string> pHeader = null)
        {
            iosSDKHungup(pCallId, JsonUtility.ToJson(new PairKeyValueArray(Utils.GetDictionaryToArray(pHeader))));
        }

        public override void setMute(Boolean pState)
        {
            iosSDKsetMute(pState);
        }

        public override void sendVideo(Boolean pState)
        {
            iosSDKsendVideo(pState);
        }

        public override void setCamera(Camera cameraPosition)
        {
            iosSDKsetCamera(cameraPosition == Camera.CAMERA_FACING_FRONT);
        }

        public override void disableTls()
        {
            iosSDKdisableTls();
        }

        public override void disconnectCall(string p, Dictionary<string, string> pHeader = null)
        {
            iosSDKdisconnectCall(p, JsonUtility.ToJson(new PairKeyValueArray(Utils.GetDictionaryToArray(pHeader))));
        }

        public override void enableDebugLogging()
        {
            AddLog("Debug logging is unsupported on iOS");
        }

        public override void loginUsingOneTimeKey(LoginOneTimeKeyClassParam pLogin)
        {
            iosSDKloginUsingOneTimeKey(pLogin.name, pLogin.hash);
        }

        public override void requestOneTimeKey(string pName)
        {
            iosSDKrequestOneTimeKey(pName);
        }

        public override void sendDTMF(DTFMClassParam pParam)
        {
            iosSDKsendDTFM(pParam.callId, pParam.digit);
        }

        public override void sendInfo(InfoClassParam pParam)
        {
            iosSDKsendInfo(pParam.callId, pParam.mimeType, pParam.content,
                JsonUtility.ToJson(new PairKeyValueArray(pParam.headers)));
        }

        public override void sendMessage(SendMessageClassParam pParam)
        {
            iosSDKsendMessage(pParam.callId, pParam.text, JsonUtility.ToJson(new PairKeyValueArray(pParam.headers)));
        }

        public override void setCameraResolution(CameraResolutionClassParam pParam)
        {
            iosSDKsetCameraResolution(pParam.width, pParam.height);
        }

        public override void setUseLoudspeaker(bool pUseLoudSpeaker)
        {
            iosSDKsetUseLoudspeaker(pUseLoudSpeaker);
        }

        #region Texture Rendering

        protected override void startVideoStreamRendering(VideoStream stream)
        {
            Assert.IsTrue(GraphicsDeviceIsSupported());

            beginSendingVideoForStream((int) stream);
        }

        #endregion

        #region Native

        [DllImport("__Internal")]
        private static extern void iosSDKinit(string pUnityObj);

        [DllImport("__Internal")]
        private static extern void iosSDKconnect();

        [DllImport("__Internal")]
        private static extern void iosSDKlogin(string pLogin, string pPass);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
        private static extern void iosSDKstartCall(string pId, bool pWithVideo, string pCustomData, string pHeaderJson);

        [DllImport("__Internal")]
        private static extern void iosSDKanswerCall(string pCallId, string pHeaderJson);

        [DllImport("__Internal")]
        private static extern void iosSDKHungup(string pCallId, string pHeaderJson);

        [DllImport("__Internal")]
        private static extern void iosSDKDecline(string pCallId, string pHeaderJson);

        [DllImport("__Internal")]
        private static extern void iosSDKsetMute(bool psetMute);

        [DllImport("__Internal")]
        private static extern void iosSDKsendVideo(bool psendVideo);

        [DllImport("__Internal")]
        private static extern void iosSDKsetCamera(bool pSetFront);

        [DllImport("__Internal")]
        private static extern void iosSDKdisableTls();

        [DllImport("__Internal")]
        private static extern void iosSDKdisconnectCall(string pCallId, string pHeaderJson);

        [DllImport("__Internal")]
        private static extern void iosSDKloginUsingOneTimeKey(string pUserName, string pOneTimeKey);

        [DllImport("__Internal")]
        private static extern void iosSDKrequestOneTimeKey(string pUserName);

        [DllImport("__Internal")]
        private static extern void iosSDKsendDTFM(string pCallId, int pDigit);

        [DllImport("__Internal")]
        private static extern void iosSDKsendInfo(string pCallId, string pWithType, string pContent,
            string pHeaderJson);

        [DllImport("__Internal")]
        private static extern void iosSDKsendMessage(string pCallId, string pMsg, string pHeaderJson);

        [DllImport("__Internal")]
        private static extern void iosSDKsetCameraResolution(int pWidth, int pHeight);

        [DllImport("__Internal")]
        private static extern void iosSDKsetUseLoudspeaker(bool pUseLoudspeaker);

        [DllImport("__Internal")]
        private static extern void iosSDKCloseConnection();

        [DllImport("__Internal")]
        private static extern void beginSendingVideoForStream(int stream);

        #endregion

        #region Native callbacks

        protected void fiosonConnectionSuccessful()
        {
            AddLog("fiosonConnectionSuccessful");
            OnConnectionSuccessful();
        }

        protected void fiosonConnectionFailedWithError(string p)
        {
            AddLog("fiosonConnectionFailedWithError" + p);
            OnConnectionFailedWithError(p);
        }

        protected void fiosonConnectionClosed()
        {
            AddLog("fiosonConnectionClosed");
            CleanupAllVideoStreams();
            OnConnectionClosed();
        }

        protected void fiosonLoginSuccessful(string p)
        {
            AddLog("fiosonLoginSuccessful: " + p);
            JSONNode node = Utils.GetParamList(p);
            OnLoginSuccessful(node[0].Value);
        }

        protected void fiosonLoginFailed(string p)
        {
            AddLog("fiosonLoginFailed: " + p);
            JSONNode node = Utils.GetParamList(p);
            OnLoginFailed(LoginFailureReasonFromString(node[0].Value));
        }

        protected void fiosonOneTimeKeyGenerated(string p)
        {
            AddLog("fiosonOneTimeKeyGenerated: " + p);
            OnOneTimeKeyGenerated(p);
        }

        protected void fiosonCallConnected(string p)
        {
            AddLog("fiosonCallConnected: " + p);
            JSONNode node = Utils.GetParamList(p);
            OnCallConnected(node[0].Value, node[1].AsDictionary);
        }

        protected void fiosonCallDisconnected(string p)
        {
            AddLog("fiosonCallDisconnected: " + p);
            JSONNode node = Utils.GetParamList(p);
            CleanupAllVideoStreams();
            OnCallDisconnected(node[0].Value, node[1].AsDictionary);
        }

        protected void fiosonCallRinging(string p)
        {
            AddLog("fiosonCallRinging: " + p);
            JSONNode node = Utils.GetParamList(p);
            OnCallRinging(node[0].Value, node[1].AsDictionary);
        }

        protected void fiosonCallFailed(string p)
        {
            AddLog("fiosonCallFailed: " + p);
            JSONNode node = Utils.GetParamList(p);
            OnCallFailed(node[0].Value, node[1].AsInt, node[2].Value, node[3].AsDictionary);
        }

        protected void fiosonCallAudioStarted(string p)
        {
            AddLog("fiosonCallAudioStarted" + p);
            OnCallAudioStarted(p);
        }

        protected void fiosonIncomingCall(string p)
        {
            AddLog("fiosonIncomingCall: " + p);
            JSONNode node = Utils.GetParamList(p);
            OnIncomingCall(node[0].Value, node[1].Value, node[2].Value, node[3].AsBool, node[4].AsDictionary);
        }

        protected void fiosonSIPInfoReceivedInCall(string p)
        {
            AddLog("fiosonSIPInfoReceivedInCall: " + p);
            JSONNode node = Utils.GetParamList(p);
            OnSIPInfoReceivedInCall(node[0].Value, node[1].Value, node[2].Value, node[3].AsDictionary);
        }

        protected void fiosonMessageReceivedInCall(string p)
        {
            AddLog("fiosonMessageReceivedInCall: " + p);
            JSONNode node = Utils.GetParamList(p);
            OnMessageReceivedInCall(node[0].Value, node[1].Value, node[2].AsDictionary);
        }

        protected void fiosonNetStatsReceived(string p)
        {
            AddLog("fiosonNetStatsReceived: " + p);
            JSONNode node = Utils.GetParamList(p);
            OnNetStatsReceived(node[0].Value, node[1].AsInt);
        }

        protected void fiosonOnStartCall(string p)
        {
            AddLog("fiosonOnStartCall: " + p);
            OnStartCall(p);
        }

        #endregion
    }
}