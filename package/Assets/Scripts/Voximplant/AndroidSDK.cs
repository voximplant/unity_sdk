using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using Voximplant.Threading;

namespace Voximplant
{
    sealed internal class AndroidSDK : VoximplantSDK
    {
#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
        [DllImport ("__Internal")]
#else
        [DllImport("VoximplantAndroidRendererPlugin")]
#endif
        private static extern void InitializeVoximplant();

        private GameObject invokePumpHolder;
        private InvokePump pump;

        void Awake()
        {
            InitializeVoximplant();

            // Ensure invoke pump
            invokePumpHolder = new GameObject("[PermissionsRequesterHelper]"){
                hideFlags = HideFlags.NotEditable | HideFlags.HideInHierarchy | HideFlags.HideInInspector
            };
            pump = invokePumpHolder.AddComponent<InvokePumpBehavior>().invokePump;
        }

        private AndroidJavaClass jc;
        private AndroidJavaObject jo;

        public override void init(Action<bool> initCallback)
        {
            Action finishInit = () => {
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

        public override void setCamera(Camera cameraPosition)
        {
            jo.Call("setCamera", cameraPosition == Camera.CAMERA_FACING_FRONT ? "1" : "0");
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

        #region Texture Rendering

        private static bool IsRunningOnOpenGL()
        {
            switch (SystemInfo.graphicsDeviceType) {
                case GraphicsDeviceType.OpenGLES2:
                case GraphicsDeviceType.OpenGLES3:
                    return true;
                default:
                    return false;
            }
        }

#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
        [DllImport ("__Internal")]
#else
        [DllImport("VoximplantAndroidRendererPlugin")]
#endif
        private static extern void DestroyRenderer(long textureId, long oglContext);

        protected override void startVideoStreamRendering(VideoStream stream)
        {
            Assert.IsTrue(IsRunningOnOpenGL());

            jo.Call("beginSendingVideoForStream", (int)stream);
        }

        #endregion

        #region Native Callbacks

        private struct NativeTextureDescriptor
        {
            public long textureId;
            public long oglContext;
            public Texture2D texture;
        }

        private Dictionary<VideoStream, NativeTextureDescriptor> nativeTextures =
            new Dictionary<VideoStream, NativeTextureDescriptor>();

        protected void faonNewNativeTexture(String p)
        {
            Debug.Log(string.Format("new native texture {0}", p));
            
            var paramList = Utils.GetParamList(p);
            var textureId = paramList[0].AsLong;
            var oglContext = paramList[1].AsLong;
            var width = paramList[2].AsInt;
            int height = paramList[3].AsInt;
            int stream = paramList[4].AsInt;
            var videoStream = (VideoStream) stream;

            if (!videoStreamCallbacks.ContainsKey(videoStream)) {
                return;
            }

            pump.BeginInvoke(() => {
                var texture = Texture2D.CreateExternalTexture(width, height, TextureFormat.RGBA32, false, false, new IntPtr(textureId));

                var newNativeTexture = new NativeTextureDescriptor{
                    oglContext = oglContext,
                    textureId = textureId,
                    texture = texture
                };
                videoStreamCallbacks[videoStream](texture);
                if (nativeTextures.ContainsKey(videoStream)) {
                    var nativeTexture = nativeTextures[videoStream];
                    DestroyRenderer(nativeTexture.textureId, nativeTexture.oglContext);
                }
                nativeTextures[videoStream] = newNativeTexture;
            });
        }

        private void CleanupAllVideoStreams()
        {
            foreach (var texture in nativeTextures.Values) {
                DestroyRenderer(texture.textureId, texture.oglContext);
            }

            nativeTextures.Clear();
            foreach (var pair in videoStreamCallbacks) {
                pair.Value(null);
            }
            videoStreamCallbacks.Clear();
        }

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
            CleanupAllVideoStreams();
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
            CleanupAllVideoStreams();
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
            CleanupAllVideoStreams();
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

        protected void faonOnStartCall(string callId)
        {
            AddLog("faonOnStartCall: " + callId);
            OnStartCall(callId);
        }

        #endregion
    }
}