/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
﻿using System;
 using System.Collections.Generic;
 using System.Linq;
using UnityEngine;
using Voximplant.Unity.Call;
using Voximplant.Unity.Client;

namespace Voximplant.Unity.@internal.Android
{
    internal class ClientAndroid : Client, IDisposable
    {
        private readonly AndroidJavaObject _clientModule;

        public ClientAndroid(ClientConfig clientConfig)
        {
            _clientModule = new AndroidJavaObject("com.voximplant.unity.ClientModule");
            _clientModule.Call("init", JsonUtility.ToJson(clientConfig));
        }

        public override ClientState State
        {
            get
            {
                var clientState = _clientModule.Call<string>("getClientState");
                clientState = clientState.Replace("_", "");
                return Enum.TryParse(clientState, true, out ClientState state) ? state : ClientState.Disconnected;
            }
        }

        public void Dispose()
        {
            _clientModule.Dispose();
        }

        public override void Connect(bool connectivityCheck, ICollection<string> gateways = null)
        {
            _clientModule.Call("connect", connectivityCheck, gateways == null ? new string[0] : gateways.ToArray());
        }

        public override void Disconnect()
        {
            _clientModule.Call("disconnect");
        }

        public override void Login(string username, string password)
        {
            _clientModule.Call("login", username, password);
        }

        public override void LoginWithToken(string username, string token)
        {
            _clientModule.Call("loginWithAccessToken", username, token);
        }

        public override void LoginWithOneTimeKey(string username, string hash)
        {
            _clientModule.Call("loginWithOneTimeKey", username, hash);
        }

        public override void RequestOneTimeKey(string username)
        {
            _clientModule.Call("requestOneTimeKey", username);
        }

        public override void RefreshToken(string username, string refreshToken)
        {
            _clientModule.Call("refreshToken", username, refreshToken);
        }

        public override ICall Call(string username, CallSettings callSettings)
        {
            var callJavaObject = _clientModule.Call<AndroidJavaObject>("call", username,
                callSettings.VideoFlags.ReceiveVideo, callSettings.VideoFlags.SendVideo,
                callSettings.VideoCodec.ToString(), callSettings.CustomData,
                JsonHelper.ToJson(callSettings.ExtraHeaders));

            if (callJavaObject == null) return null;

            var call = new CallAndroid(callJavaObject);
            CallAdded(call);

            return call;
        }

        public override ICall CallConference(string conference, CallSettings callSettings)
        {
            var callJavaObject = _clientModule.Call<AndroidJavaObject>("callConference", conference,
                callSettings.VideoFlags.ReceiveVideo, callSettings.VideoFlags.SendVideo,
                callSettings.VideoCodec.ToString(), callSettings.CustomData,
                JsonHelper.ToJson(callSettings.ExtraHeaders));

            if (callJavaObject == null) return null;

            var call = new CallAndroid(callJavaObject);
            CallAdded(call);

            return call;
        }

        protected override ICall GetIncomingCall(string callId)
        {
            var callJavaObject = _clientModule.Call<AndroidJavaObject>("getIncomingCall", callId);

            var call = new CallAndroid(callJavaObject);
            call.SyncEndpoints();
            CallAdded(call);

            return call;
        }
    }
}
