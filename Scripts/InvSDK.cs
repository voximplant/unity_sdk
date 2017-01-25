﻿using System;
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

        public delegate void deligateOnLoginSuccessful(string displayName);
        public delegate void deligateOnLoginFailed(LoginFailureReason reason);
        public delegate void deligateOnOneTimeKeyGenerated(string key);
        public delegate void deligateOnConnectionSuccessful();
        public delegate void deligateOnConnectionClosed();
        public delegate void deligateOnConnectionFailedWithError(string reason);
        public delegate void deligateOnCallConnected(string callId, Dictionary<string, string> headers);
        public delegate void deligateOnCallDisconnected(string callId, Dictionary<string, string> headers);
        public delegate void deligateOnCallRinging(string callId, Dictionary<string, string> headers);
        public delegate void deligateOnCallFailed(string callId, int code, string reason, Dictionary<string, string> headers);
        public delegate void deligateOnCallAudioStarted(string callId);
        public delegate void deligatonIncomingCall(String callId, String from, String displayName, Boolean videoCall, Dictionary<String, String> headers);
        public delegate void deligateOnSIPInfoReceivedInCall(string callId, string type, string content, Dictionary<string, string> headers);
        public delegate void deligateOnMessageReceivedInCall(string callId, string text);
        public delegate void deligateOnNetStatsReceived(string callId, int packetLoss);

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


        /**
        Close connection to the Voximplant cloud that was previously established via 'connect' call.
        @method closeConnection
        */
        public void closeConnection()
        {
            if (AndroidPlatform())
                jo.Call("closeConnection");
        }

        /**
        Initiate Voximplant cloud connection. After successful connection a "login" method should be called.
        @method connect
        */
        public void connect()
        {
            if (AndroidPlatform())
                jo.Call("connect");
        }
	
	/**
        Login into Voximplant cloud. Should be called after "connect" in the "OnConnectionSuccessful" handler.
        @method login
	@param {LoginClassParam} pLogin Username and password. Username is a fully-qualified string that includes Voximplant user, application and account names. The format is: "username@appname.accname.voximplant.com"
        */
        public void login(LoginClassParam pLogin)
        {
            if (AndroidPlatform())
                jo.Call("login", Invoice.JsonUtility.ToJson(pLogin));
        }
	
	/**
	Start a new outgoing call.
	@method call
	@param {CallClassParam} pCall Call options: number to call, video call flag and custom data to send alongside the call. For SIP compatibility reasons number should be a non-empty string even if the number itself is not used by a Voximplant cloud scenario. "OnCallConnected" will be called on call success, or "OnCallFailed" will be called if Voximplant cloud rejects a call or network error occur.  
	*/
        public string call(CallClassParam pCall) 
        {
            if (AndroidPlatform())
                return jo.Call<string>("call", Invoice.JsonUtility.ToJson(pCall));
			return "";
        }
	
	/**
	Answer incoming call. Should be called from the "OnIncomingCall" handler
	@method answer
	*/
        public void answer(string pCallId)
        {
            if (AndroidPlatform())
                jo.Call("answer", pCallId);
        }
	
	/**
	Decline an incoming call. Should be called from the "OnIncomingCall" handler
	@method declineCall
	*/
        public void declineCall(string pCallId)
        {
            if (AndroidPlatform())
                jo.Call("declineCall", pCallId);
        }
	
	/**
	Hang up the call in progress. Should be called from the "OnIncomingCall" handler
	@method hangup
	*/
        public void hangup(string pCallId)
        {
            if (AndroidPlatform())
				jo.Call("hangup", pCallId);
        }
	
	/**
	Mute or unmute microphone. This is reset after audio interruption
	@method setMute
	@param {Boolean} p 'true' to set mute status, 'false' to remove it
	*/
        public void setMute(Boolean p)
        {
            if (AndroidPlatform())
				jo.Call("setMute", Invoice.JsonUtility.ToJson(new BoolClassParam(p)));
        }
	
	/**
	Enable or disable video stream transfer during the call
	@method sendVideo
	@param {Boolean} p 'true' to enable video stream, 'false' to disable it
	*/
        public void sendVideo(Boolean p)
        {
            if (AndroidPlatform())
                jo.Call("sendVideo", Invoice.JsonUtility.ToJson(new BoolClassParam(p)));
        }
	
	/**
	Select a camera to use for the video call
	@method setCamera
	@param {CameraSet} p A camera to use: 'CameraSet.CAMERA_FACING_FRONT', 'CameraSet.CAMERA_FACING_BACK'
	*/
        public void setCamera(CameraSet p)
        {
            if (AndroidPlatform())
                jo.Call("setCamera", (p == CameraSet.CAMERA_FACING_FRONT)?"1":"0");
        }

	// Internal
        public void disableTls()
        {
            if (AndroidPlatform())
                jo.Call("disableTls");
        }
	
	/**
	Disconnect the specified call
	@method disconnectCall
	@param {string} p Call identifier returned by previous 'call()'.
	*/
        public void disconnectCall(string p)
        {
            if (AndroidPlatform())
                jo.Call("disconnectCall", Invoice.JsonUtility.ToJson(new StringClassParam(p)));
        }
	
	/**
	If called before any other SDK methods, enables debug logging into target platform default debug log facility
	@method enableDebugLogging
	*/
        public void enableDebugLogging()
        {
            if (AndroidPlatform())
                jo.Call("enableDebugLogging");
        }
	
	/**
	Login using a hash created from one-time key requested via 'requestOneTimeKey' and password. The has can be calculated on your backend so Voximplant password is never used in the application. Hash is calculated as 'MD5(one-time-key + "|" + MD5(vox-user + ":voximplant.com:" + vox-pass))'
	@method loginUsingOneTimeKey
	@param {LoginOneTimeKeyClassParam} pLogin Fully-qualified user name and hash to authenticate. Note that hash is created not from a fully-qualified user name, but from a bare user name, without the "@app.acc.voximplant.com" part
	*/
        public void loginUsingOneTimeKey(LoginOneTimeKeyClassParam pLogin)
        {
            if (AndroidPlatform())
                jo.Call("loginUsingOneTimeKey", Invoice.JsonUtility.ToJson(pLogin));
        }
	
	/**
	Request a one-time key that can be used on your backend to create a login hash for the 'loginUsingOneTimeKey'. Key is returned via 'onOneTimeKeyGenerated' event.
	@method requestOneTimeKey
	@param {string} pName Fully-qualified user name to get a one-time key for. Format is 'user@app.acc.voximplant.com'
	*/
        public void requestOneTimeKey(string pName)
        {
            if (AndroidPlatform())
                jo.Call("requestOneTimeKey", Invoice.JsonUtility.ToJson(new StringClassParam(pName)));
        }
	
	/**
	Send DTMF signal to the specified call
	@method sendDTMF
	@param {DTFMClassParam} pParam Call identifier returned by previous 'call()' and DTMF digit number (0-9, 10 for '*', 11 for '#')
	*/
        public void sendDTMF(DTFMClassParam pParam)
        {
            if (AndroidPlatform())
                jo.Call("sendDTMF", Invoice.JsonUtility.ToJson(pParam));
        }
	
	/**
	Send arbitrary data to Voximplant cloud. Data can be received in VoxEngine scenario by subscribing to the 'CallEvents.InfoReceived' event. Optional SIP headers can be automatically passed to second call via VoxEngine 'easyProcess()' method.
	@method sendInfo
	@param {InfoClassParam} pParam Call identifier returned by previous 'call()', data MIME type string, data string and optional SIP headers list as an array of 'PairKeyValue' objects with 'key' and 'value' properties.
	*/
        public void sendInfo(InfoClassParam pParam)
        {
            if (AndroidPlatform())
                jo.Call("sendInfo", Invoice.JsonUtility.ToJson(pParam));
        }
	
	/**
	Simplified version of 'sendInfo' that uses predefined MIME type to send a text message. Message can be received in VoxEngine scenario by subscribing to the 'CallEvents.MessageReceived' event. Optional SIP headers can be automatically passed to second call via VoxEngine 'easyProcess()' method.
	@method sendMessage
	@param {SendMessageClassParam} pParam Call identifier returned by previous 'call()', message string and optional SIP headers list as an array of 'PairKeyValue' objects with 'key' and 'value' properties.
	*/
        public void sendMessage(SendMessageClassParam pParam)
        {
            if (AndroidPlatform())
                jo.Call("sendMessage", Invoice.JsonUtility.ToJson(pParam));
        }
	
	/**
	Set local camera resolution. Increasing resolution increases video quality, but also uses more cpu and bandwidth.
	@method setCameraResolution
	@param {CameraResolutionClassParam} pParam Camera resolutino as width and height, in pixels
	*/
        public void setCameraResolution(CameraResolutionClassParam pParam)
        {
            if (AndroidPlatform())
                jo.Call("setCameraResolution", Invoice.JsonUtility.ToJson(pParam));
        }
	
	/**
	Enable or disable loud speaker, if available
	@method setUseLoudspeaker
	@param {bool} pUseLoudSpeaker 'true' to enable loud speaker, 'false' to disable it
	*/
        public void setUseLoudspeaker(bool pUseLoudSpeaker)
        {
            if (AndroidPlatform())
                jo.Call("setUseLoudspeaker", Invoice.JsonUtility.ToJson(new BoolClassParam(pUseLoudSpeaker)));
        }

	/**
	Called if login() call results in the successful login
	@event onLoginSuccessful
	@param {string} username Display name of logged in user
	*/
        public void faonLoginSuccessful(string p)
        {
            addLog("faonLoginSuccessful: " + p);
            if (onLoginSuccessful != null)
                onLoginSuccessful(p);
        }

	/**
	Called if login() call results in the failed login
	@event onLoginFailed
	@param {string} error Failure reason
	*/
        public void faonLoginFailed(string p)
        {
            addLog("faonLoginFailed: " + p);
            if (onLoginFailed != null)
            {
                switch (p)
                {
                    case "INVALID_PASSWORD":
                    {
                        onLoginFailed(LoginFailureReason.INVALID_PASSWORD);
                        break;
                    }
                    case "INVALID_USERNAME":
                    {
                        onLoginFailed(LoginFailureReason.INVALID_USERNAME);
                        break;
                    }
                    case "ACCOUNT_FROZEN":
                    {
                        onLoginFailed(LoginFailureReason.ACCOUNT_FROZEN);
                        break;
                    }
                    case "INTERNAL_ERROR":
                    {
                        onLoginFailed(LoginFailureReason.INTERNAL_ERROR);
                        break;
                    }
                }
            }
                
        }
	
	/**
	Called after key requested with 'requestOneTimeKey' is requested and returned from the Voximplant cloud
	@event onOneTimeKeyGenerated
	@param {string} key Key string that should be used in a hash for the 'loginUsingOneTimeKey'
	*/
        public void faonOneTimeKeyGenerated(string p)
        {
            addLog("faonOneTimeKeyGenerated: " + p);
            if (onOneTimeKeyGenerated != null)
                onOneTimeKeyGenerated(p);
        }
	
	/**
	Called after connect() successfully connects to Voximplant cloud
	@event onConnectionSuccessful
	*/
        public void faonConnectionSuccessful()
        {
            addLog("faonConnectionSuccessful");
            if (onConnectionSuccessful != null)
                onConnectionSuccessful();
        }
	
	/**
	Called after connection to Voximplant cloud is closed for any reason
	@event onConnectionClosed
	*/
        public void faonConnectionClosed()
        {
            addLog("faonConnectionClosed");
            if (onConnectionClosed != null)
                onConnectionClosed();
        }

	/**
	Called if connect() failed to establish a Voximplant cloud connection
	@event onConnectionFailedWithError
	@param {string} error Error message
	*/
        public void faonConnectionFailedWithError(string p)
        {
            addLog("faonConnectionFailedWithError" + p);
            if (onConnectionFailedWithError != null)
                onConnectionFailedWithError(p);
        }
	
	/**
	Called after call() method successfully established a call with the Voximplant cloud
	@event onCallConnected
	@param {string} callid Connected call identifier. It's same identifier returned by the call() function and it can be used in other function to specify one of multiple calls
	@param {string} headers Dictionary with optional SIP headers that was sent by Voximplant while accepting the call 
        */
	public void faonCallConnected(string p)
        {
            addLog("faonCallConnected: " + p);
            JSONNode node = GetParamList(p);
            if (onCallConnected != null)
	    	//! 1 arg iOS limit, unpack
                onCallConnected(node[0].Value, node[1].AsDictionary);
        }
	
	/**
	Called after call is gracefully disconnected from the Voximplant cloud
	@event onCallDisconnected
	@param {string} callid Call identifier, previously returned by the call() function
	@param {string} headers Dictionary with optional SIP headers that was sent by Voximplant while disconnecting the call
	*/
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
                onMessageReceivedInCall(node[0].Value, node[1].Value);
        }
        public void faonNetStatsReceived(string p)
        {
            addLog("faonNetStatsReceived");
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
