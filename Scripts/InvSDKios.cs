using System;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;
using System.Runtime.InteropServices;

namespace Invoice
{
	public class InvSDKios: MonoBehaviour
	{

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

		[DllImport ("__Internal")]
		private static extern void iosSDKinit(string pUnityObj);

		[DllImport ("__Internal")]
		private static extern void iosSDKconnect();

		[DllImport ("__Internal")]
		private static extern void iosSDKlogin(string pLogin, string pPass);

		[DllImport ("__Internal")]
		private static extern void iosSDKstartCall(string pId, bool pWithVideo, string pCustomData, string pHeaderJson);

		[DllImport ("__Internal")]
		private static extern void iosSDKanswerCall();

		[DllImport ("__Internal")]
		private static extern void iosSDKHungup();

		[DllImport ("__Internal")]
		private static extern void iosSDKDecline();

		[DllImport ("__Internal")]
		private static extern void iosSDKsetMute(bool psetMute);

		[DllImport ("__Internal")]
		private static extern void iosSDKsendVideo(bool psendVideo);

		[DllImport ("__Internal")]
		private static extern void iosSDKsetCamera(bool pSetFront);

		[DllImport ("__Internal")]
		private static extern void iosSDKdisableTls();

		[DllImport ("__Internal")]
		private static extern void iosSDKdisconnectCall(string pCallId);

		[DllImport ("__Internal")]
		private static extern void iosSDKloginUsingOneTimeKey(string pUserName, string pOneTimeKey);

		[DllImport ("__Internal")]
		private static extern void iosSDKrequestOneTimeKey(string pUserName);

		[DllImport ("__Internal")]
		private static extern void iosSDKsendDTFM(string pCallId, int pDigit);
	
		[DllImport ("__Internal")]
		private static extern void iosSDKsendInfo(string pCallId, string pWithType, string pContent, string pAniHeaders);

		[DllImport ("__Internal")]
		private static extern void iosSDKsendMessage(string pCallId, string pMsg, string pAniHaders);

		[DllImport ("__Internal")]
		private static extern void iosSDKsetCameraResolution(int pWidth, int pHeight);

		[DllImport ("__Internal")]
		private static extern void iosSDKsetUseLoudspeaker(bool pUseLoudspeaker);

		[DllImport ("__Internal")]
		private static extern void iosSDKsetLocalSize(int xPos, int yPos, int pWidth, int pHeight);

		[DllImport ("__Internal")]
		private static extern void iosSDKsetRemoteSize(int xPos, int yPos, int pWidth, int pHeight);

		public InvSDKios ()
		{
		}

		private bool IPhonePlatform()
		{
			return Application.platform == RuntimePlatform.IPhonePlayer;
		}

		public void addLog(String pMsg)
		{
			if (LogMethod != null)
				LogMethod(pMsg);
		}

		public void setLocalSizeView(SizeView pSize)
		{
			if (IPhonePlatform())
				iosSDKsetLocalSize(pSize.x_pos, pSize.y_pos, pSize.width, pSize.height);
		}

		public void setRemoteSizeView(SizeView pSize)
		{
			if (IPhonePlatform())
				iosSDKsetRemoteSize(pSize.x_pos, pSize.y_pos, pSize.width, pSize.height);
		}

		public void init(String pObjectNameSDK)
		{
			if (IPhonePlatform())
				iosSDKinit(pObjectNameSDK);
		}
		public void init(String pObjectNameSDK, SizeView pLocalView, SizeView pRemoteView)
		{
			if (IPhonePlatform())
			{
				iosSDKinit(pObjectNameSDK);
				setLocalSizeView(pLocalView);
				setRemoteSizeView(pRemoteView);
			}
		}
		public void connect()
		{
			if (IPhonePlatform())
				iosSDKconnect();
		}
//		public void closeConnection()
//		{
//			if (IPhonePlatform())
//				jo.Call("closeConnection");
//		}
		public void login(LoginClassParam pLogin)
		{
			if (IPhonePlatform())
				iosSDKlogin(pLogin.login, pLogin.pass);
		}
		public void call(CallClassParamios pCall) 
		{
			if (IPhonePlatform()){
				iosSDKstartCall(pCall.userCall, pCall.videoCall, pCall.customData, Invoice.JsonUtility.ToJson(new PairKeyValueArray(pCall.headers)));
			}
		}
		public void answer()
		{
			if (IPhonePlatform())
				iosSDKanswerCall();
		}
		public void declineCall()
		{
			if (IPhonePlatform())
				iosSDKDecline();
		}
		public void hangup()
		{
			if (IPhonePlatform())
				iosSDKHungup();
		}
		public void setMute(Boolean p)
		{
			if (IPhonePlatform())
				iosSDKsetMute(p);
		}
		public void sendVideo(Boolean p)
		{
			if (IPhonePlatform())
				iosSDKsendVideo(p);
		}
		public void setCamera(CameraSet p)
		{
			if (IPhonePlatform())
				iosSDKsetCamera((p == CameraSet.CAMERA_FACING_FRONT)?true:false);
		}
		public void disableTls()
		{
			if (IPhonePlatform())
				iosSDKdisableTls();
		}
		public void disconnectCall(string p)
		{
			if (IPhonePlatform())
				iosSDKdisconnectCall(p);
		}

		public void loginUsingOneTimeKey(LoginOneTimeKeyClassParam pLogin)
		{
			if (IPhonePlatform())
				iosSDKloginUsingOneTimeKey(pLogin.name, pLogin.hash);
		}
		public void requestOneTimeKey(string pName)
		{
			if (IPhonePlatform())
				iosSDKrequestOneTimeKey(pName);
		}
		public void sendDTMF(DTFMClassParam pParam)
		{
			if (IPhonePlatform())
				iosSDKsendDTFM(pParam.callId, pParam.digit);
		}
		public void sendInfo(InfoClassParam pParam)
		{
			if (IPhonePlatform())
				iosSDKsendInfo(pParam.callId,pParam.mimeType,pParam.content, Invoice.JsonUtility.ToJson(pParam.headers));
		}
		public void sendMessage(SendMessageClassParam pParam)
		{
			if (IPhonePlatform())
				iosSDKsendMessage(pParam.callId, pParam.text, Invoice.JsonUtility.ToJson(pParam.headers));
		}
		public void setCameraResolution(CameraResolutionClassParam pParam)
		{
			if (IPhonePlatform())
				iosSDKsetCameraResolution(pParam.width, pParam.height);
		}
		public void setUseLoudspeaker(bool pUseLoudSpeaker)
		{
			if (IPhonePlatform())
				iosSDKsetUseLoudspeaker(pUseLoudSpeaker);
		}

		// callbacks from iOS sdk
		public void fiosonConnectionSuccessful()
		{
			addLog("fiosonConnectionSuccessful");
			if (onConnectionSuccessful != null)
				onConnectionSuccessful();
		}
		public void fiosonConnectionFailedWithError(string p)
		{
			addLog("fiosonConnectionFailedWithError" + p);
			if (onConnectionFailedWithError != null)
				onConnectionFailedWithError(p);
		}

		public void fiosonConnectionClosed()
		{
			addLog("fiosonConnectionClosed");
			if (onConnectionClosed != null)
				onConnectionClosed();
		}

		public void fiosonLoginSuccessful(string p)
		{
			addLog("fiosonLoginSuccessful: " + p);
			if (onLoginSuccessful != null)
				onLoginSuccessful(p);
		}
		public void fiosonLoginFailed(string p)
		{
			addLog("fiosonLoginFailed: " + p);
			if (onLoginFailed != null)
				onLoginFailed(p);
		}
		public void fiosonOneTimeKeyGenerated(string p)
		{
			addLog("fiosonOneTimeKeyGenerated: " + p);
			if (onOneTimeKeyGenerated != null)
				onOneTimeKeyGenerated(p);
		}
			
		public void fiosonCallConnected(string p)
		{
			addLog("fiosonCallConnected: " + p);
			JSONNode node = GetParamList(p);
			if (onCallConnected != null)
				onCallConnected(node[0].Value, node[1].AsDictionary);
		}
		public void fiosonCallDisconnected(string p)
		{
			addLog("fiosonCallDisconnected: " + p);
			JSONNode node = GetParamList(p);
			if (onCallDisconnected != null)
				onCallDisconnected(node[0].Value, node[1].AsDictionary);
		}
		public void fiosonCallRinging(string p)
		{
			addLog("fiosonCallRinging: " + p);
			JSONNode node = GetParamList(p);
			if (onCallRinging != null)
				onCallRinging(node[0].Value, node[1].AsDictionary);
		}
		public void fiosonCallFailed(string p)
		{
			addLog("fiosonCallFailed: " + p);
			JSONNode node = GetParamList(p);
			if (onCallFailed != null)
				onCallFailed(node[0].Value, node[1].AsInt, node[2].Value, node[3].AsDictionary);
		}
		public void fiosonCallAudioStarted(string p)
		{
			addLog("fiosonCallAudioStarted" + p);
			if (onCallAudioStarted != null)
				onCallAudioStarted(p);
		}
		public void fiosonIncomingCall(string p)
		{
			addLog("fiosonIncomingCall: " + p);
			JSONNode node = GetParamList(p);
			if (onIncomingCall != null)
				onIncomingCall(node[0].Value, node[1].Value, node[2].Value, node[3].AsBool, node[4].AsDictionary);
		}
		public void fiosonSIPInfoReceivedInCall(string p)
		{
			addLog("fiosonSIPInfoReceivedInCall: " + p);
			JSONNode node = GetParamList(p);
			if (onSIPInfoReceivedInCall != null)
				onSIPInfoReceivedInCall(node[0].Value, node[1].Value, node[2].Value, node[3].AsDictionary);
		}
		public void fiosonMessageReceivedInCall(string p)
		{
			addLog("fiosonMessageReceivedInCall: " + p);
			JSONNode node = GetParamList(p);
			if (onMessageReceivedInCall != null)
				onMessageReceivedInCall(node[0].Value, node[1].Value, node[2].AsDictionary);
		}
		public void fiosonNetStatsReceived(string p)
		{
			addLog("fiosonNetStatsReceived: " + p);
			JSONNode node = GetParamList(p);
			if (onNetStatsReceived != null)
				onNetStatsReceived(node[0].Value, node[1].AsInt);
		}

		public static JSONNode GetParamList(string p)
		{
			JSONNode rootNode = JSON.Parse(p);
			return rootNode;
		}

	}
}

