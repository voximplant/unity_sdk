/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Voximplant.Unity.Call;
using Voximplant.Unity.Client;

namespace Voximplant.Unity.@internal.UnityEditor
{
    internal class ClientUnityEditor : Client
    {
        public ClientUnityEditor()
        {
            State = ClientState.Disconnected;
        }

        public override ClientState State { get; }

        public override void Connect(bool connectivityCheck, ICollection<string> gateways = null)
        {
            throw new NotImplementedException();
        }

        public override void Disconnect()
        {
            throw new NotImplementedException();
        }

        public override void Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public override void LoginWithToken(string username, string token)
        {
            throw new NotImplementedException();
        }

        public override void LoginWithOneTimeKey(string username, string hash)
        {
            throw new NotImplementedException();
        }

        public override void RequestOneTimeKey(string username)
        {
            throw new NotImplementedException();
        }

        public override void RefreshToken(string username, string refreshToken)
        {
            throw new NotImplementedException();
        }

        public override ICall Call(string username, CallSettings callSettings)
        {
            throw new NotImplementedException();
        }

        public override ICall CallConference(string conference, CallSettings callSettings)
        {
            throw new NotImplementedException();
        }

        protected override ICall GetIncomingCall(string callId)
        {
            throw new NotImplementedException();
        }
    }
}
