using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
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

        VoximplantSDK _voximplant;

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

            _voximplant = gameObject.AddVoximplantSDK();
			_voximplant.init(success => {
			        if (success) {
			            setMute();
			            sendVideo();
			            switchCam();
			        }

			        addLog(string.Format("Init finished, permissions status: ${0}", success));
			    },
			    new SizeView(0,0, 100, 100), new SizeView(0, 150, 100, 100));
            _voximplant.LogMethod += addLog;
            _voximplant.ConnectionSuccessful += InvConnectionSuccessful;
            _voximplant.IncomingCall += InvIncomingCall;
            _voximplant.CallRinging += InvCallRinging;
			_voximplant.CallFailed += InvCallFailed;
            _voximplant.MessageReceivedInCall += InvMessageReceivedInCall;
            _voximplant.CallConnected += InvCallConnected;
            _voximplant.CallDisconnected += InvCallDisconnected;
			_voximplant.StartCall += InvStartCall;
        }

        void InvStartCall (string callId)
        {
			mActiveCallId = new CallInner(callId, "own", false, video.isOn);
        }

        private void InvCallFailed (string callId, int code, string reason, Dictionary<string, string> headers)
        {
			mBtnHung.SetActive(false);
			mCallRingPanel.SetActive(false);
			mActiveCallId = null;
			mIncCallId = null; 
        }

        private void InvCallDisconnected(string callId, Dictionary<string, string> headers)
        {
            mBtnHung.SetActive(false);
            mCallRingPanel.SetActive(false);
			mActiveCallId = null;
			mIncCallId = null; 
			_voximplant.setLocalView(false);
			_voximplant.setRemoteView(false);
        }

        private void InvCallConnected(string callId, Dictionary<string, string> headers)
        {
			addLog("Call connected");
			mBtnHung.SetActive(true);
			_voximplant.setLocalView(mLocalView.isOn);
			_voximplant.setRemoteView(mRemoteView.isOn);
        }

		private void InvMessageReceivedInCall(string callId, string text, Dictionary<string, string> headers)
        {
            addLog(callId + " : " + text);
        }

        private void InvCallRinging(string callId, Dictionary<string, string> headers)
        {
			addLog("Call ringing");
        }

        private void InvIncomingCall(string callId, string from, string displayName, bool videoCall, Dictionary<string, string> headers)
        {
            mCallRingPanel.SetActive(true);
            mIncName.text = displayName;
            mIncCallId = new CallInner(callId, displayName, true, videoCall);
        }

        private void InvConnectionSuccessful()
        {
            addLog("Connect done!");
        }

        public void onClickConnect()
        {
			_voximplant.connect();
        }
        public void onClickLogin()
        {
			_voximplant.login(new LoginClassParam(loginName.text, pass.text));
        }
        public void onClickCall()
        {
			_voximplant.call(new CallClassParam(callNum.text, video.isOn, p2p.isOn, "", null));
            mBtnHung.SetActive(true);
        }
        public void onClickAnswer()
        {
            mCallRingPanel.SetActive(false);
            mActiveCallId = mIncCallId;
			_voximplant.answer(mActiveCallId.id, null);
        }
        public void onClickDecline()
        {
            mCallRingPanel.SetActive(false);
            _voximplant.declineCall(mIncCallId.id, null);
        }
        public void onHangup()
        {
			_voximplant.hangup(mActiveCallId.id, null);
        }
        public void setMute()
        {
            _voximplant.setMute(mute.isOn);
        }
        public void sendVideo()
        {
            _voximplant.sendVideo(video.isOn);
        }
        public void switchCam()
        {
            if (faceCam.isOn)
			{
                _voximplant.setCamera(CameraSet.CAMERA_FACING_FRONT);
			}
            else
			{
                _voximplant.setCamera(CameraSet.CAMERA_FACING_BACK);
			}
        }

		public void onChangeLocalView()
		{
			_voximplant.setLocalView(mLocalView.isOn);
		}
		public void onChangeRemoteView()
		{
			_voximplant.setRemoteView(mRemoteView.isOn);
		}

        public void addLog(string pMsg)
        {
            _log.text += _logStrNum + ": " + pMsg + "\n";
            _logStrNum += 1;
            _scroll.verticalNormalizedPosition = 0f;
        }
    }
}