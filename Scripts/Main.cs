using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using SimpleJSON;

namespace Invoice
{
    public class Main : MonoBehaviour
    {

        public Text _log;
        public ScrollRect _scroll;
        int _logStrNum = 0;

        public InputField name;
        public InputField pass;
        public InputField callNum;
        public Toggle p2p;
        public Toggle video;
        public Toggle mute;
        public Toggle faceCam;
        public GameObject mBtnHung;
        public Text mIncName;

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

//			invios = GameObject.FindObjectOfType<InvSDKios>();
//			invios.init("SDKIOS", new SizeView(0,0, 100, 100), new SizeView(0, 150, 100, 100));
//			invios.LogMethod += addLog;
//			invios.onConnectionSuccessful += Invios_onConnectionSuccessful;

            inv = GameObject.FindObjectOfType<InvSDK>();
			inv.init("SDK", new SizeView(0,0, 100, 100), new SizeView(0, 150, 100, 100));
            inv.LogMethod += addLog;
            inv.onConnectionSuccessful += Inv_onConnectionSuccessful;
            inv.onIncomingCall += Inv_onIncomingCall;
            inv.onCallRinging += Inv_onCallRinging;
            inv.onMessageReceivedInCall += Inv_onMessageReceivedInCall;
            inv.onCallConnected += Inv_onCallConnected;
            inv.onCallDisconnected += Inv_onCallDisconnected;
        }

        private void Inv_onCallDisconnected(string callId, Dictionary<string, string> headers)
        {
            mBtnHung.SetActive(false);
            mCallRingPanel.SetActive(false);
        }

        private void Inv_onCallConnected(string callId, Dictionary<string, string> headers)
        {
            mBtnHung.SetActive(true);
        }

		private void Inv_onMessageReceivedInCall(string callId, string text, Dictionary<string, string> headers)
        {
            addLog(callId + " : " + text);
        }

        private void Inv_onCallRinging(string callId, Dictionary<string, string> headers)
        {
            mCallRingPanel.SetActive(true);
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
            inv.login(new LoginClassParam(name.text, pass.text));
        }
        public void onClickCall()
        {
			string callID = inv.call(new CallClassParam(callNum.text, video.isOn, p2p.isOn, "", null));
            mActiveCallId = new CallInner(callID, "own", false, video.isOn);
            mBtnHung.SetActive(true);
            addLog("StartCall with ID: " + mActiveCallId.id);
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

        public void addLog(string pMsg)
        {
            _log.text += _logStrNum + ": " + pMsg + "\n";
            _logStrNum += 1;
            _scroll.verticalNormalizedPosition = 0f;
        }
    }
}