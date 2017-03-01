using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

namespace Voximplant
{
    internal class AndroidSDK : VoximplantSDK
    {
        private AndroidJavaClass jc;
        private AndroidJavaObject jo;

        public override void init(Action<bool> initCallback, SizeView pLocalView,
            SizeView pRemoteView)
        {
            Action finishInit = () => {
                setLocalSizeView(pLocalView);
                setRemoteSizeView(pRemoteView);
                initCallback(true);
            };

#if UNITY_ANDROID || UNITY_EDITOR
            Action initJava = () => {
                try {
                    jc = new AndroidJavaClass("com.voximplant.sdk.AVoImClient");
                }
                catch (UnityException e) {
                    Debug.logger.Log("JC Error: " + e.Message);
                    initCallback(false);
                    return;
                }

                try {
                    jo = jc.CallStatic<AndroidJavaObject>("instance");
                }
                catch (AndroidJavaException e) {
                    Debug.logger.Log("JO Error: " + e.Message);
                    initCallback(false);
                    return;
                }
                jo.Call("setSDKObjectName", gameObject.name);

                finishInit();
            };


            if (!PermissionsRequester.RequirePermissionRequests()) {
                initJava(); // No permissions support at all
                return;
            }

            var requester = PermissionsRequester.Instance;

            string[] permissions = {
                "android.permission.RECORD_AUDIO",
                "android.permission.MODIFY_AUDIO_SETTINGS",
                "android.permission.INTERNET",
                "android.permission.CAMERA"
            };

            bool requestPermissions = false;
            foreach (var permission in permissions) {
                if (!requester.IsPermissionGranted(permission)) {
                    requestPermissions = true;
                    break;
                }
            }

            if (requestPermissions) {
                requester.RequestPermissions(permissions, statuses => {
                    foreach (var status in statuses) {
                        if (!status.Granted) {
                            initCallback(false);
                            return;
                        }
                    }

                    initJava();
                });
            } else {
                initJava();
            }
#endif
        }

        public override void setLocalSizeView(SizeView pSize)
        {
            jo.Call("setLocalSize", JsonUtility.ToJson(pSize));
        }

        public override void setRemoteSizeView(SizeView pSize)
        {
            jo.Call("setRemoteSize", JsonUtility.ToJson(pSize));
        }

        public override void closeConnection()
        {
            jo.Call("closeConnection");
        }

        public override void connect()
        {
            jo.Call("connect");
        }

        public override void login(LoginClassParam pLogin)
        {
            jo.Call("login", JsonUtility.ToJson(pLogin));
        }

        public override void call(CallClassParam pCall)
        {
            jo.Call<string>("call", JsonUtility.ToJson(pCall));
        }

        public override void answer(string pCallId, Dictionary<string, string> pHeader = null)
        {
            jo.Call("answer", pCallId);
        }

        public override void declineCall(string pCallId, Dictionary<string, string> pHeader = null)
        {
            jo.Call("declineCall", pCallId);
        }

        public override void hangup(string pCallId, Dictionary<string, string> pHeader = null)
        {
            jo.Call("hangup", pCallId);
        }

        public override void setMute(Boolean pState)
        {
            jo.Call("setMute", JsonUtility.ToJson(new BoolClassParam(pState)));
        }

        public override void sendVideo(Boolean pState)
        {
            jo.Call("sendVideo", JsonUtility.ToJson(new BoolClassParam(pState)));
        }

        public override void setLocalView(Boolean pState)
        {
            jo.Call("setLocalView", JsonUtility.ToJson(new BoolClassParam(pState)));
        }

        public override void setRemoteView(Boolean pState)
        {
            jo.Call("setRemoteView", JsonUtility.ToJson(new BoolClassParam(pState)));
        }

        public override void setCamera(CameraSet p)
        {
            jo.Call("setCamera", p == CameraSet.CAMERA_FACING_FRONT ? "1" : "0");
        }

        public override void disableTls()
        {
            jo.Call("disableTls");
        }

        public override void disconnectCall(string p, Dictionary<string, string> pHeader = null)
        {
            jo.Call("disconnectCall", JsonUtility.ToJson(new StringClassParam(p)));
        }

        public override void enableDebugLogging()
        {
            jo.Call("enableDebugLogging");
        }

        public override void loginUsingOneTimeKey(LoginOneTimeKeyClassParam pLogin)
        {
            jo.Call("loginUsingOneTimeKey", JsonUtility.ToJson(pLogin));
        }

        public override void requestOneTimeKey(string pName)
        {
            jo.Call("requestOneTimeKey", JsonUtility.ToJson(new StringClassParam(pName)));
        }

        public override void sendDTMF(DTFMClassParam pParam)
        {
            jo.Call("sendDTMF", JsonUtility.ToJson(pParam));
        }

        public override void sendInfo(InfoClassParam pParam)
        {
            jo.Call("sendInfo", JsonUtility.ToJson(pParam));
        }

        public override void sendMessage(SendMessageClassParam pParam)
        {
            jo.Call("sendMessage", JsonUtility.ToJson(pParam));
        }

        public override void setCameraResolution(CameraResolutionClassParam pParam)
        {
            jo.Call("setCameraResolution", JsonUtility.ToJson(pParam));
        }

        public override void setUseLoudspeaker(bool pUseLoudSpeaker)
        {
            jo.Call("setUseLoudspeaker", JsonUtility.ToJson(new BoolClassParam(pUseLoudSpeaker)));
        }

        // callbacks from Android sdk
        protected void faonLoginSuccessful(string p)
        {
            AddLog("faonLoginSuccessful: " + p);
            JSONNode node = Utils.GetParamList(p);
            OnLoginSuccessful(node[0].Value);
        }

        protected void faonLoginFailed(string p)
        {
            AddLog("faonLoginFailed: " + p);
            JSONNode node = Utils.GetParamList(p);

            OnLoginFailed(LoginFailureReasonFromString(node[0].Value));
        }

        protected void faonOneTimeKeyGenerated(string p)
        {
            AddLog("faonOneTimeKeyGenerated: " + p);
            OnOneTimeKeyGenerated(p);
        }

        protected void faonConnectionSuccessful()
        {
            AddLog("faonConnectionSuccessful");
            OnConnectionSuccessful();
        }

        protected void faonConnectionClosed()
        {
            AddLog("faonConnectionClosed");
            OnConnectionClosed();
        }

        protected void faonConnectionFailedWithError(string p)
        {
            AddLog("faonConnectionFailedWithError" + p);
            OnConnectionFailedWithError(p);
        }

        protected void faonCallConnected(string p)
        {
            AddLog("faonCallConnected: " + p);
            JSONNode node = Utils.GetParamList(p);
            OnCallConnected(node[0].Value, node[1].AsDictionary);
        }

        protected void faonCallDisconnected(string p)
        {
            AddLog("faonCallDisconnected: " + p);
            JSONNode node = Utils.GetParamList(p);
            OnCallDisconnected(node[0].Value, node[1].AsDictionary);
        }

        protected void faonCallRinging(string p)
        {
            AddLog("faonCallRinging: " + p);
            JSONNode node = Utils.GetParamList(p);
            OnCallRinging(node[0].Value, node[1].AsDictionary);
        }

        protected void faonCallFailed(string p)
        {
            AddLog("faonCallFailed: " + p);
            JSONNode node = Utils.GetParamList(p);
            OnCallFailed(node[0].Value, node[1].AsInt, node[2].Value, node[3].AsDictionary);
        }

        protected void faonCallAudioStarted(string p)
        {
            AddLog("faonCallAudioStarted" + p);
            OnCallAudioStarted(p);
        }

        protected void faonIncomingCall(string p)
        {
            AddLog("faonIncomingCall: " + p);
            JSONNode node = Utils.GetParamList(p);
            OnIncomingCall(node[0].Value, node[1].Value, node[2].Value, node[3].AsBool, node[4].AsDictionary);
        }

        protected void faonSIPInfoReceivedInCall(string p)
        {
            AddLog("faonSIPInfoReceivedInCall");
            JSONNode node = Utils.GetParamList(p);
            OnSIPInfoReceivedInCall(node[0].Value, node[1].Value, node[2].Value, node[3].AsDictionary);
        }

        protected void faonMessageReceivedInCall(string p)
        {
            AddLog("faonMessageReceivedInCall");
            JSONNode node = Utils.GetParamList(p);
            OnMessageReceivedInCall(node[0].Value, node[1].Value, null);
        }

        protected void faonNetStatsReceived(string p)
        {
            AddLog("faonNetStatsReceived");
            JSONNode node = Utils.GetParamList(p);
            OnNetStatsReceived(node[0], node[1].AsInt);
        }

        protected void faonOnStartCall(string p)
        {
            AddLog("faonOnStartCall: " + p);
            OnStartCall(p);
        }
    }
}