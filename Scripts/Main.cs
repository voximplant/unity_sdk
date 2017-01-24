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

        InvSDK inv;
		InvSDKios invios;

		private string mActiveCallId = "";

        void Start()
        {
			Dictionary<string, string> dic = new Dictionary<string, string>();
			dic.Add("key1", "value1");

            InfoClassParam param = new InfoClassParam("qwe", "asd", "zxc");
			JSONObject jsonObject = new JSONObject();
			JSONSerialize.Serialize(param, jsonObject);

			addLog("Target platform: " + Application.platform);

			invios = GameObject.FindObjectOfType<InvSDKios>();
			invios.init("SDKIOS", new SizeView(0,0, 100, 100), new SizeView(0, 150, 100, 100));
			invios.LogMethod += addLog;
			invios.onConnectionSuccessful += Invios_onConnectionSuccessful;

            inv = GameObject.FindObjectOfType<InvSDK>();
            inv.LogMethod += addLog;
            inv.onConnectionSuccessful += Inv_onConnectionSuccessful;
        }

        void Invios_onConnectionSuccessful ()
        {
			addLog("Connect from iso done!");
        }
			
        private void Inv_onConnectionSuccessful()
        {
            addLog("Connect done!");
        }

        public void onClickConnect()
        {
			invios.connect();
			inv.connect();
        }
        public void onClickLogin()
        {
			invios.login(new LoginClassParam(name.text, pass.text));
            inv.login(new LoginClassParam(name.text, pass.text));
        }
        public void onClickCall()
        {
			
			invios.call(new CallClassParamios(callNum.text, video.isOn, p2p.isOn, "", null));
			mActiveCallId = inv.call(new CallClassParam(callNum.text, video.isOn, ""));
			addLog("StartCall with ID: " + mActiveCallId);
        }
        public void onClickAnswer()
        {
			invios.answer();
			inv.answer(mActiveCallId);
        }
        public void onClickDecline()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("key", "value");
            dic.Add("key1", "value1");
            InfoClassParam param = new InfoClassParam("qwe", "asd", "zxc", dic);
            inv.sendInfo(param);
            //inv.declineCall();
        }
        public void onHangup()
        {
			invios.hangup();
			inv.hangup(mActiveCallId);
        }
        public void setMute()
        {
			invios.setMute(mute.isOn);
            inv.setMute(mute.isOn);
        }
        public void sendVideo()
        {
			invios.sendVideo(video.isOn);
            inv.sendVideo(video.isOn);
        }
        public void switchCam()
        {
            if (faceCam.isOn)
			{
				invios.setCamera(CameraSet.CAMERA_FACING_FRONT);
                inv.setCamera(CameraSet.CAMERA_FACING_FRONT);
			}
            else
			{
				invios.setCamera(CameraSet.CAMERA_FACING_BACK);
                inv.setCamera(CameraSet.CAMERA_FACING_BACK);
			}
        }

        public void addLog(string pMsg)
        {
            _log.text += _logStrNum + ": " + pMsg + "\n";
            _logStrNum += 1;
            _scroll.verticalNormalizedPosition = 0f;
        }

       /* private void onConnectSuccessful()
        {
            addLog("Connect done!");
        }
        private void onConnectFailed(string pErr)
        {
            addLog("Connect failed" + pErr);
        }
        private void onLoginSeccessful(string s)
        {
            addLog("Login seccessful: " + s);
        }
        private void onLoginFailed(string s)
        {
            addLog("Login failed: " + s);
        }
        private void onCallRinging(string p, Dictionary<string, string> p2)
        {
            addLog("Call ringing: " + p);
        }
        private void onCallFailed(string p, int p1, string p2, Dictionary<string, string> p3)
        {
            addLog("Call failed: " + p);
        }*/


    }
}