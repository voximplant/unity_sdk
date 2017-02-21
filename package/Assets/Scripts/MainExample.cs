using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using SimpleJSON;
using Voximplant;

namespace Invoice
{
    public class MainExample : MonoBehaviour
    {
        public Text _log;
        public ScrollRect _scroll;
        int _logStrNum = 0;

        public InputField loginName;
        public InputField pass;
        public InputField callNum;
        public Toggle p2p;
        public Toggle video;
        public Toggle mute;
        public Toggle faceCam;
        public GameObject mBtnHung;
        public Text mIncName;

		public Toggle mLocalView;
		public Toggle mRemoteView;

        public GameObject mCallRingPanel;

        InvSDK inv;

		private CallInner mActiveCallId;
        private CallInner mIncCallId;

        private class CallInner
        {
            public string id;
            public string fromName;
            public bool incoming;
            public bool video;

            public CallInner(string pId, string pFrom, bool pInc, bool pVideo)
            {
                id = pId;
                fromName = pFrom;
                incoming = pInc;
                video = pVideo;
            }
        }

        void Start()
        {
			addLog("Target platform: " + Application.platform);

            inv = gameObject.AddComponent<InvSDK>();
			inv.init(gameObject.name, success => {
			        if (success) {
			            setMute();
			            sendVideo();
			            switchCam();
			        }

			        addLog(string.Format("Init finished, permissions status: ${0}", success));
			    },
			    new SizeView(0,0, 100, 100), new SizeView(0, 150, 100, 100));
            inv.LogMethod += addLog;
            inv.onConnectionSuccessful += Inv_onConnectionSuccessful;
            inv.onIncomingCall += Inv_onIncomingCall;
            inv.onCallRinging += Inv_onCallRinging;
			inv.onCallFailed += Inv_onCallFailed;
            inv.onMessageReceivedInCall += Inv_onMessageReceivedInCall;
            inv.onCallConnected += Inv_onCallConnected;
            inv.onCallDisconnected += Inv_onCallDisconnected;
			inv.onStartCall += Inv_onStartCall;
        }

        void Inv_onStartCall (string callId)
        {
			mActiveCallId = new CallInner(callId, "own", false, video.isOn);
        }

        private void Inv_onCallFailed (string callId, int code, string reason, Dictionary<string, string> headers)
        {
			mBtnHung.SetActive(false);
			mCallRingPanel.SetActive(false);
			mActiveCallId = null;
			mIncCallId = null; 
        }

        private void Inv_onCallDisconnected(string callId, Dictionary<string, string> headers)
        {
            mBtnHung.SetActive(false);
            mCallRingPanel.SetActive(false);
			mActiveCallId = null;
			mIncCallId = null; 
			inv.setLocalView(false);
			inv.setRemoteView(false);
        }

        private void Inv_onCallConnected(string callId, Dictionary<string, string> headers)
        {
			addLog("Call connected");
			mBtnHung.SetActive(true);
			inv.setLocalView(mLocalView.isOn);
			inv.setRemoteView(mRemoteView.isOn);
        }

		private void Inv_onMessageReceivedInCall(string callId, string text, Dictionary<string, string> headers)
        {
            addLog(callId + " : " + text);
        }

        private void Inv_onCallRinging(string callId, Dictionary<string, string> headers)
        {
			addLog("Call ringing");
        }

        private void Inv_onIncomingCall(string callId, string from, string displayName, bool videoCall, Dictionary<string, string> headers)
        {
            mCallRingPanel.SetActive(true);
            mIncName.text = displayName;
            mIncCallId = new CallInner(callId, displayName, true, videoCall);
        }

        private void Inv_onConnectionSuccessful()
        {
            addLog("Connect done!");
        }

        public void onClickConnect()
        {
			inv.connect();
        }
        public void onClickLogin()
        {
			inv.login(new LoginClassParam(loginName.text, pass.text));
        }
        public void onClickCall()
        {
			inv.call(new CallClassParam(callNum.text, video.isOn, p2p.isOn, "", null));
            mBtnHung.SetActive(true);
        }
        public void onClickAnswer()
        {
            mCallRingPanel.SetActive(false);
            mActiveCallId = mIncCallId;
			inv.answer(mActiveCallId.id, null);
        }
        public void onClickDecline()
        {
            mCallRingPanel.SetActive(false);
            inv.declineCall(mIncCallId.id, null);
        }
        public void onHangup()
        {
			inv.hangup(mActiveCallId.id, null);
        }
        public void setMute()
        {
            inv.setMute(mute.isOn);
        }
        public void sendVideo()
        {
            inv.sendVideo(video.isOn);
        }
        public void switchCam()
        {
            if (faceCam.isOn)
			{
                inv.setCamera(CameraSet.CAMERA_FACING_FRONT);
			}
            else
			{
                inv.setCamera(CameraSet.CAMERA_FACING_BACK);
			}
        }

		public void onChangeLocalView()
		{
			inv.setLocalView(mLocalView.isOn);
		}
		public void onChangeRemoteView()
		{
			inv.setRemoteView(mRemoteView.isOn);
		}

        public void addLog(string pMsg)
        {
            _log.text += _logStrNum + ": " + pMsg + "\n";
            _logStrNum += 1;
            _scroll.verticalNormalizedPosition = 0f;
        }
    }
}