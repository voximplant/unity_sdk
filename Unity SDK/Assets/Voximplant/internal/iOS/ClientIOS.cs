/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System.Collections.Generic;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using UnityEngine;
using Voximplant.Unity.Call;
using Voximplant.Unity.Client;

namespace Voximplant.Unity.@internal.iOS
{
    internal class ClientIOS : Client
    {
        public ClientIOS(ClientConfig clientConfig)
        {
            voximplant_init(JsonUtility.ToJson(clientConfig));
        }

        public override ClientState State => voximplant_client_state();

        [DllImport("__Internal")]
        private static extern void voximplant_init(string clientConfig);

        [DllImport("__Internal", EntryPoint = "UnitySetAudioSessionActive")]
        private static extern void UnitySetAudioSessionActive(bool active);

        /// <summary>
        /// On iOS unity Audio is being turned off due to changes on
        /// the native
        /// [AVAudioSession sharedInstance]
        /// 
        /// This function can be used to turn unity audio on again. All sources
        /// will be set to IsPlaying = false and need to be started again after
        /// this call.
        /// 
        /// Only use this function after all voice calls ended. If not
        /// the microphone / audio might stop working.
        /// </summary>
        private static void UnitySetAudioSessionActive()
        {
            // ReSharper disable once IntroduceOptionalParameters.Local
            UnitySetAudioSessionActive(true);
        }

        [DllImport("__Internal")]
        private static extern ClientState voximplant_client_state();

        [DllImport("__Internal")]
        private static extern void voximplant_client_connect(bool connectivityCheck, string servers);

        public override void Connect(bool connectivityCheck, ICollection<string> gateways = null)
        {
            Debug.Log("connect called");
            voximplant_client_connect(connectivityCheck, gateways != null ? string.Join(";", gateways) : "");
        }

        [DllImport("__Internal")]
        private static extern void voximplant_client_disconnect();

        public override void Disconnect()
        {
            voximplant_client_disconnect();
        }

        [DllImport("__Internal")]
        private static extern void voximplant_client_login(string username, string password);

        public override void Login(string username, string password)
        {
            voximplant_client_login(username, password);
        }

        [DllImport("__Internal")]
        private static extern void voximplant_client_login_with_token(string username, string token);

        public override void LoginWithToken(string username, string token)
        {
            voximplant_client_login_with_token(username, token);
        }

        [DllImport("__Internal")]
        private static extern void voximplant_client_login_with_one_time_key(string username, string hash);

        public override void LoginWithOneTimeKey(string username, string hash)
        {
            voximplant_client_login_with_one_time_key(username, hash);
        }

        [DllImport("__Internal")]
        private static extern void voximplant_client_request_one_time_key(string username);

        public override void RequestOneTimeKey(string username)
        {
            voximplant_client_request_one_time_key(username);
        }

        [DllImport("__Internal")]
        private static extern void voximplant_client_refresh_token(string username, string refreshToken);

        public override void RefreshToken(string username, string refreshToken)
        {
            voximplant_client_refresh_token(username, refreshToken);
        }

        [DllImport("__Internal")]
        [CanBeNull]
        private static extern string voximplant_client_call(string username,
            bool receiveVideo, bool sendVideo, VideoCodec videoCodec,
            string customData, string headers);

        public override ICall Call(string username, CallSettings callSettings)
        {
            UnitySetAudioSessionActive();

            var callId = voximplant_client_call(username,
                callSettings.VideoFlags.ReceiveVideo, callSettings.VideoFlags.SendVideo, callSettings.VideoCodec,
                callSettings.CustomData, JsonHelper.ToJson(callSettings.ExtraHeaders));

            if (callId == null) return null;

            var call = new CallIOS(callId);
            CallAdded(call);

            return call;
        }

        [DllImport("__Internal")]
        [CanBeNull]
        private static extern string voximplant_client_call_conference(string conference,
            bool receiveVideo, bool sendVideo, VideoCodec videoCodec,
            string customData, string headers);

        public override ICall CallConference(string conference, CallSettings callSettings)
        {
            UnitySetAudioSessionActive();

            var callId = voximplant_client_call_conference(conference,
                callSettings.VideoFlags.ReceiveVideo, callSettings.VideoFlags.SendVideo, callSettings.VideoCodec,
                callSettings.CustomData, JsonHelper.ToJson(callSettings.ExtraHeaders));

            if (callId == null) return null;

            var call = new CallIOS(callId);
            CallAdded(call);

            return call;
        }

        protected override ICall GetIncomingCall(string callId)
        {
            UnitySetAudioSessionActive();

            var call = new CallIOS(callId);
            call.SyncEndpoints();
            CallAdded(call);

            return call;
        }
    }
}