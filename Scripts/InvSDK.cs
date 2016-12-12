using System;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

namespace Invoice
{
    class InvSDK: MonoBehaviour, IUnitySDKCallbacks
    {
        private AndroidJavaClass jc = null;
        private AndroidJavaObject jo;
        public Action<String> LogMethod;

        public delegate void deligateOnLoginSuccessful(string p1);
        public delegate void deligateOnLoginFailed(string p1);
        public delegate void deligateOnOneTimeKeyGenerated(string p1);
        public delegate void deligateOnConnectionSuccessful();
        public delegate void deligateOnConnectionClosed();
        public delegate void deligateOnConnectionFailedWithError(string p1);
        public delegate void deligateOnCallConnected(string p1, Dictionary<string, string> p2);
        public delegate void deligateOnCallDisconnected(string p1, Dictionary<string, string> p2);
        public delegate void deligateOnCallRinging(string p1, Dictionary<string, string> p2);
        public delegate void deligateOnCallFailed(string p1, int p2, string p3, Dictionary<string, string> p4);
        public delegate void deligateOnCallAudioStarted(string p1);
        public delegate void deligatonIncomingCall(String p1, String p2, String p3, Boolean p4, Dictionary<String, String> p5);
        public delegate void deligateOnSIPInfoReceivedInCall(string p1, string p2, string p3, Dictionary<string, string> p4);
        public delegate void deligateOnMessageReceivedInCall(string p1, string p2, Dictionary<string, string> p3);
        public delegate void deligateOnNetStatsReceived(string p1, int p2);

        public event deligateOnNetStatsReceived onNetStatsReceived;
        public event deligateOnMessageReceivedInCall onMessageReceivedInCall;
        public event deligateOnSIPInfoReceivedInCall onSIPInfoReceivedInCall;
        public event deligatonIncomingCall onIncomingCall;
        public event deligateOnCallAudioStarted onCallAudioStarted;
        public event deligateOnCallFailed onCallFailed;
        public event deligateOnCallRinging onCallRinging;
        public event deligateOnCallDisconnected onCallDisconnected;
        public event deligateOnCallConnected onCallConnected;
        public event deligateOnConnectionFailedWithError onConnectionFailedWithError;
        public event deligateOnConnectionClosed onConnectionClosed;
        public event deligateOnConnectionSuccessful onConnectionSuccessful;
        public event deligateOnOneTimeKeyGenerated onOneTimeKeyGenerated;
        public event deligateOnLoginFailed onLoginFailed;
        public event deligateOnLoginSuccessful onLoginSuccessful;

        public void Start()
        {
            if (AndroidPlatform())
            {
                try
                {
                    jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                }
                catch (UnityException e)
                {
					Debug.logger.Log("JC Error: " + e.Message);
                }

                try
                {
                    jo = jc.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mVoxClient");
                }
                catch (AndroidJavaException e)
                {
                    Debug.logger.Log("JO Error: " + e.Message);
                }
            }

        }
        private bool AndroidPlatform()
        {
            return Application.platform == RuntimePlatform.Android;
        }
		private bool OSxPlatform()
		{
			return Application.platform == RuntimePlatform.OSXEditor;
		}

        public void addLog(String pMsg)
        {
            if (LogMethod != null)
                LogMethod(pMsg);
        }

        public void closeConnection()
        {
            if (AndroidPlatform())
                jo.Call("closeConnection");
        }
        public void connect()
        {
            if (AndroidPlatform())
                jo.Call("connect");
			else if (OSxPlatform())
			{
				
			}
        }
        public void login(LoginClassParam pLogin)
        {
            if (AndroidPlatform())
                jo.Call("login", Invoice.JsonUtility.ToJson(pLogin));
        }
        public void call(CallClassParam pCall) 
        {
            if (AndroidPlatform())
                jo.Call("call", Invoice.JsonUtility.ToJson(pCall));
        }
        public void answer()
        {
            if (AndroidPlatform())
                jo.Call("answer");
        }
        public void declineCall()
        {
            if (AndroidPlatform())
                jo.Call("declineCall");
        }
        public void hangup()
        {
            if (AndroidPlatform())
                jo.Call("hangup");
        }
        public void setMute(Boolean p)
        {
            if (AndroidPlatform())
				jo.Call("setMute", Invoice.JsonUtility.ToJson(new BoolClassParam(p)));
        }
        public void sendVideo(Boolean p)
        {
            if (AndroidPlatform())
                jo.Call("sendVideo", Invoice.JsonUtility.ToJson(new BoolClassParam(p)));
        }
        public void setCamera(CameraSet p)
        {
            if (AndroidPlatform())
                jo.Call("setCamera", (p == CameraSet.CAMERA_FACING_FRONT)?"1":"0");
        }
        public void disableTls()
        {
            if (AndroidPlatform())
                jo.Call("disableTls");
        }
        public void disconnectCall(string p)
        {
            if (AndroidPlatform())
                jo.Call("disconnectCall", Invoice.JsonUtility.ToJson(new StringClassParam(p)));
        }
        public void enableDebugLogging()
        {
            if (AndroidPlatform())
                jo.Call("enableDebugLogging");
        }
        public void loginUsingOneTimeKey(LoginOneTimeKeyClassParam pLogin)
        {
            if (AndroidPlatform())
                jo.Call("loginUsingOneTimeKey", Invoice.JsonUtility.ToJson(pLogin));
        }
        public void requestOneTimeKey(string pName)
        {
            if (AndroidPlatform())
                jo.Call("requestOneTimeKey", Invoice.JsonUtility.ToJson(new StringClassParam(pName)));
        }
        public void sendDTMF(DTFMClassParam pParam)
        {
            if (AndroidPlatform())
                jo.Call("sendDTMF", Invoice.JsonUtility.ToJson(pParam));
        }
        public void sendInfo(InfoClassParam pParam)
        {
            if (AndroidPlatform())
                jo.Call("sendInfo", Invoice.JsonUtility.ToJson(pParam));
        }
        public void sendMessage(SendMessageClassParam pParam)
        {
            if (AndroidPlatform())
                jo.Call("sendMessage", Invoice.JsonUtility.ToJson(pParam));
        }
        public void setCameraResolution(CameraResolutionClassParam pParam)
        {
            if (AndroidPlatform())
                jo.Call("setCameraResolution", Invoice.JsonUtility.ToJson(pParam));
        }
        public void setUseLoudspeaker(bool pUseLoudSpeaker)
        {
            if (AndroidPlatform())
                jo.Call("setUseLoudspeaker", Invoice.JsonUtility.ToJson(new BoolClassParam(pUseLoudSpeaker)));
        }

        public void faonLoginSuccessful(string p)
        {
            addLog("faonLoginSuccessful: " + p);
            if (onLoginSuccessful != null)
                onLoginSuccessful(p);
        }
        public void faonLoginFailed(string p)
        {
            addLog("faonLoginFailed: " + p);
            if (onLoginFailed != null)
                onLoginFailed(p);
        }
        public void faonOneTimeKeyGenerated(string p)
        {
            addLog("faonOneTimeKeyGenerated: " + p);
            if (onOneTimeKeyGenerated != null)
                onOneTimeKeyGenerated(p);
        }
        public void faonConnectionSuccessful()
        {
            addLog("faonConnectionSuccessful");
            if (onConnectionSuccessful != null)
                onConnectionSuccessful();
        }
        public void faonConnectionClosed()
        {
            addLog("faonConnectionClosed");
            if (onConnectionClosed != null)
                onConnectionClosed();
        }
        public void faonConnectionFailedWithError(string p)
        {
            addLog("faonConnectionFailedWithError" + p);
            if (onConnectionFailedWithError != null)
                onConnectionFailedWithError(p);
        }
        public void faonCallConnected(string p)
        {
            addLog("faonCallConnected: " + p);
            JSONNode node = GetParamList(p);
            if (onCallConnected != null)
                onCallConnected(node[0].Value, node[1].AsDictionary);
        }
        public void faonCallDisconnected(string p)
        {
            addLog("faonCallDisconnected: " + p);
            JSONNode node = GetParamList(p);
            if (onCallDisconnected != null)
                onCallDisconnected(node[0].Value, node[1].AsDictionary);
        }
        public void faonCallRinging(string p)
        {
            addLog("faonCallRinging: " + p);
            JSONNode node = GetParamList(p);
            if (onCallRinging != null)
                onCallRinging(node[0].Value, node[1].AsDictionary);
        }
        public void faonCallFailed(string p)
        {
            addLog("faonCallFailed: " + p);
            JSONNode node = GetParamList(p);
            if (onCallFailed != null)
                onCallFailed(node[0].Value, node[1].AsInt, node[2].Value, node[3].AsDictionary);
        }
        public void faonCallAudioStarted(string p)
        {
            addLog("faonCallAudioStarted" + p);
            if (onCallAudioStarted != null)
                onCallAudioStarted(p);
        }
        public void faonIncomingCall(string p)
        {
            addLog("faonIncomingCall: " + p);
            JSONNode node = GetParamList(p);
            if (onIncomingCall != null)
                onIncomingCall(node[0].Value, node[1].Value, node[2].Value, node[3].AsBool, node[4].AsDictionary);
        }
        public void faonSIPInfoReceivedInCall(string p)
        {
            addLog("faonSIPInfoReceivedInCall");
            JSONNode node = GetParamList(p);
            if (onSIPInfoReceivedInCall != null)
                onSIPInfoReceivedInCall(node[0].Value, node[1].Value, node[2].Value, node[3].AsDictionary);
        }
        public void faonMessageReceivedInCall(string p)
        {
            addLog("faonMessageReceivedInCall");
            JSONNode node = GetParamList(p);
            if (onMessageReceivedInCall != null)
                onMessageReceivedInCall(node[0].Value, node[1].Value, node[2].AsDictionary);
        }
        public void faonNetStatsReceived(string p)
        {
            addLog("faonMessageReceivedInCall");
            JSONNode node = GetParamList(p);
            if (onNetStatsReceived != null)
                onNetStatsReceived(node[0], node[1].AsInt);
        }

        public static JSONNode GetParamList(string p)
        {
            JSONNode rootNode = JSON.Parse(p);
            return rootNode;
        }
        public static PairKeyValue[] GetDictionaryToArray(Dictionary<string, string> pDic)
        {
            PairKeyValue[] list = new PairKeyValue[pDic.Count];
            int i = 0;
            foreach (KeyValuePair<string, string> pair in pDic)
            {
                list[i] = new PairKeyValue(pair.Key, pair.Value);
                i += 1;
            }
            return list;
        }
    }
}
