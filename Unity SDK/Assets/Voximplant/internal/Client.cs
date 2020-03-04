/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using Voximplant.Unity.Call;
using Voximplant.Unity.Call.EventArgs;
using Voximplant.Unity.Client;
using Voximplant.Unity.Client.EventArgs;

namespace Voximplant.Unity.@internal
{
    internal abstract class Client : IClient
    {
        public abstract ClientState State { get; }

        public void Connect()
        {
            Connect(false);
        }

        public abstract void Connect(bool connectivityCheck, ICollection<string> gateways = null);
        public abstract void Disconnect();
        public abstract void Login(string username, string password);
        public abstract void LoginWithToken(string username, string token);
        public abstract void LoginWithOneTimeKey(string username, string hash);
        public abstract void RequestOneTimeKey(string username);
        public abstract void RefreshToken(string username, string refreshToken);
        public abstract ICall Call(string username, CallSettings callSettings);
        public abstract ICall CallConference(string conference, CallSettings callSettings);

        public event SdkEventHandler<IClient> Connected;
        public event SdkEventHandler<IClient, ConnectionFailedEventArgs> ConnectionFailed;
        public event SdkEventHandler<IClient> Disconnected;
        public event SdkEventHandler<IClient, LoginSuccessEventArgs> LoginSuccess;
        public event SdkEventHandler<IClient, LoginFailedEventArgs> LoginFailed;
        public event SdkEventHandler<IClient, RefreshTokenSuccessEventArgs> RefreshTokenSuccess;
        public event SdkEventHandler<IClient, RefreshTokenFailedEventArgs> RefreshTokenFailed;
        public event SdkEventHandler<IClient, OneTimeKeyGeneratedEventArgs> OneTimeKeyGenerated;
        public event SdkEventHandler<IClient, IncomingCallEventArgs> IncomingCall;

        protected void CallAdded(Call call)
        {
            CallManager.Instance.AddCall(call);
            call.EndpointAdded += CallOnEndpointAdded;
            call.Disconnected += CallOnRemoved;
            call.Failed += CallOnRemoved;
        }

        private void CallOnRemoved(ICall sender, EventArgs e)
        {
            Debug.Log($"CallOnRemoved: {sender.CallId}");
            CallManager.Instance.RemoveCall(sender.CallId);
        }

        private void CallOnEndpointAdded(ICall sender, CallEndpointAddedEventArgs e)
        {
            var endpoint = (Endpoint) e.Endpoint;
            endpoint.Removed += EndpointOnRemoved;
            CallManager.Instance.AddEndpoint((Call) sender, endpoint);
        }

        private void EndpointOnRemoved(IEndpoint sender)
        {
            CallManager.Instance.RemoveEndpoint(sender.EndpointId);
        }

        protected abstract ICall GetIncomingCall(string callId);

        public void OnEvent(SdkEvent sdkEvent)
        {
            if (sdkEvent.Event.Equals("Connected"))
            {
                Connected?.Invoke(this);
            }
            else if (sdkEvent.Event.Equals("ConnectionFailed"))
            {
                ConnectionFailed?.Invoke(this, sdkEvent.GetEventArgs<ConnectionFailedEventArgs>());
            }
            else if (sdkEvent.Event.Equals("Disconnected"))
            {
                Disconnected?.Invoke(this);
            }
            else if (sdkEvent.Event.Equals("LoginSuccess"))
            {
                LoginSuccess?.Invoke(this, sdkEvent.GetEventArgs<LoginSuccessEventArgs>());
            }
            else if (sdkEvent.Event.Equals("LoginFailed"))
            {
                LoginFailed?.Invoke(this, sdkEvent.GetEventArgs<LoginFailedEventArgs>());
            }
            else if (sdkEvent.Event.Equals("RefreshTokenSuccess"))
            {
                RefreshTokenSuccess?.Invoke(this, sdkEvent.GetEventArgs<RefreshTokenSuccessEventArgs>());
            }
            else if (sdkEvent.Event.Equals("RefreshTokenFailed"))
            {
                RefreshTokenFailed?.Invoke(this, sdkEvent.GetEventArgs<RefreshTokenFailedEventArgs>());
            }
            else if (sdkEvent.Event.Equals("OneTimeKeyGenerated"))
            {
                OneTimeKeyGenerated?.Invoke(this, sdkEvent.GetEventArgs<OneTimeKeyGeneratedEventArgs>());
            }
            else if (sdkEvent.Event.Equals("IncomingCall"))
            {
                var eventArgs = sdkEvent.GetEventArgs<IncomingCallEventArgs>();
                eventArgs.Call = GetIncomingCall(eventArgs.callId);

                IncomingCall?.Invoke(this, eventArgs);
            }
            else
            {
                Debug.LogError($"Unknown Event '{sdkEvent.Event}'");
            }
        }

        public void OnCallEvent(CallSdkEvent sdkEvent)
        {
            var call = CallManager.Instance.GetCall(sdkEvent.CallId);
            if (call != null)
            {
                call.OnEvent(sdkEvent);
            }
            else
            {
                Debug.LogError($"Unknown Call with CallId = {sdkEvent.CallId}");
            }
        }

        public void OnEndpointEvent(EndpointSdkEvent sdkEvent)
        {
            var endpoint = CallManager.Instance.GetEndpoint(sdkEvent.EndpointId);
            if (endpoint != null)
            {
                endpoint.OnEvent(sdkEvent);
            }
            else
            {
                Debug.LogError($"Unknown Endpoint with EndpointId = {sdkEvent.EndpointId}");
            }
        }
    }
}