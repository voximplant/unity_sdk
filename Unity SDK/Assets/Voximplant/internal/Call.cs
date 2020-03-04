/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Voximplant.Unity.Call;
using Voximplant.Unity.Call.EventArgs;

namespace Voximplant.Unity.@internal
{
    internal abstract class Call : ICall, IDisposable
    {
        private readonly Dictionary<string, Endpoint> _endpoints = new Dictionary<string, Endpoint>();

        private readonly Dictionary<string, VideoStream> _localVideoStreams = new Dictionary<string, VideoStream>();
        protected readonly Dictionary<string, Action<Error?>> CallActions = new Dictionary<string, Action<Error?>>();

        protected VideoSource VideoSource;

        public abstract string CallId { get; }
        public IReadOnlyCollection<IEndpoint> Endpoints => _endpoints.Values;

        public abstract void Start();
        public abstract void Answer(CallSettings callSettings);
        public abstract void Reject(RejectMode rejectMode, IDictionary<string, string> headers);
        public abstract void Hangup(IDictionary<string, string> headers);

        public abstract void SendAudio(bool sendAudio);

        public abstract void SendVideo(bool sendVideo, Action<Error?> completion = null);

        public abstract void Hold(bool hold, Action<Error?> completion = null);

        public abstract void ReceiveVideo(Action<Error?> completion = null);

        public abstract void SendDTMF(string dtmf);
        public abstract void SendInfo(string body, string mimeType, IDictionary<string, string> headers);
        public abstract void SendMessage(string message);
        public abstract ulong Duration { get; }

        public void SetVideoSource(Camera camera, float fps = 30)
        {
            SetVideoSource(camera, 640, 480, fps);
        }

        public void SetVideoSource(Camera camera, int width, int height, float fps = 30)
        {
            VideoSource = (new GameObject()).AddComponent<VideoSource>();
            VideoSource.Initialize(camera, width, height, fps);
            SetVideoSourceImpl();
        }

        public IReadOnlyCollection<IVideoStream> LocalVideoStreams => _localVideoStreams.Values;

        public event SdkEventHandler<ICall, CallConnectedEventArgs> Connected;
        public event SdkEventHandler<ICall, CallDisconnectedEventArgs> Disconnected;
        public event SdkEventHandler<ICall, CallRingingEventArgs> Ringing;
        public event SdkEventHandler<ICall, CallFailedEventArgs> Failed;
        public event SdkEventHandler<ICall> AudioStarted;
        public event SdkEventHandler<ICall, CallSIPInfoReceivedEventArgs> SIPInfoReceived;
        public event SdkEventHandler<ICall, CallMessageReceivedEventArgs> MessageReceived;
        public event SdkEventHandler<ICall, CallLocalVideoStreamAddedEventArgs> LocalVideoStreamAdded;
        public event SdkEventHandler<ICall, CallLocalVideoStreamRemovedEventArgs> LocalVideoStreamRemoved;
        public event SdkEventHandler<ICall> ICETimeout;
        public event SdkEventHandler<ICall> ICECompleted;
        public event SdkEventHandler<ICall, CallEndpointAddedEventArgs> EndpointAdded;

        public void Dispose()
        {
            foreach (var endpoint in _endpoints.Values)
            {
                endpoint.Dispose();
            }

            foreach (var videoStream in _localVideoStreams.Values)
            {
                videoStream.Dispose();
            }

            if (VideoSource != null)
            {
                VideoSource.Dispose();
            }

            DisposeImpl();
        }

        public abstract void SyncEndpoints();

        protected abstract void SetVideoSourceImpl();

        public void OnEvent(CallSdkEvent callEvent)
        {
            Debug.Log($"Event {callEvent.Event} received by Call {CallId}");
            if (callEvent.Event.Equals("Connected"))
            {
                Connected?.Invoke(this, callEvent.GetEventArgs<CallConnectedEventArgs>());
            }
            else if (callEvent.Event.Equals("Disconnected"))
            {
                Disconnected?.Invoke(this, callEvent.GetEventArgs<CallDisconnectedEventArgs>());
            }
            else if (callEvent.Event.Equals("Ringing"))
            {
                Ringing?.Invoke(this, callEvent.GetEventArgs<CallRingingEventArgs>());
            }
            else if (callEvent.Event.Equals("Failed"))
            {
                Failed?.Invoke(this, callEvent.GetEventArgs<CallFailedEventArgs>());
            }
            else if (callEvent.Event.Equals("AudioStarted"))
            {
                AudioStarted?.Invoke(this);
            }
            else if (callEvent.Event.Equals("SIPInfoReceived"))
            {
                SIPInfoReceived?.Invoke(this, callEvent.GetEventArgs<CallSIPInfoReceivedEventArgs>());
            }
            else if (callEvent.Event.Equals("MessageReceived"))
            {
                MessageReceived?.Invoke(this, callEvent.GetEventArgs<CallMessageReceivedEventArgs>());
            }
            else if (callEvent.Event.Equals("LocalVideoStreamAdded"))
            {
                var eventArgs = callEvent.GetEventArgs<CallLocalVideoStreamAddedEventArgs>();
                var videoStream = CreateVideoStream(eventArgs.streamId);
                _localVideoStreams[eventArgs.streamId] = videoStream;
                eventArgs.VideoStream = videoStream;

                LocalVideoStreamAdded?.Invoke(this, eventArgs);
            }
            else if (callEvent.Event.Equals("LocalVideoStreamRemoved"))
            {
                var eventArgs = callEvent.GetEventArgs<CallLocalVideoStreamRemovedEventArgs>();
                if (!_localVideoStreams.ContainsKey(eventArgs.streamId)) return;

                var videoStream = _localVideoStreams[eventArgs.streamId];
                _localVideoStreams.Remove(eventArgs.streamId);

                eventArgs.VideoStream = videoStream;
                videoStream.Dispose();

                LocalVideoStreamRemoved?.Invoke(this, eventArgs);
            }
            else if (callEvent.Event.Equals("ICETimeout"))
            {
                ICETimeout?.Invoke(this);
            }
            else if (callEvent.Event.Equals("ICECompleted"))
            {
                ICECompleted?.Invoke(this);
            }
            else if (callEvent.Event.Equals("EndpointAdded"))
            {
                var eventArgs = callEvent.GetEventArgs<CallEndpointAddedEventArgs>();

                if (_endpoints.ContainsKey(eventArgs.endpointId)) return;

                eventArgs.Endpoint = AddEndpoint(eventArgs.endpointId);

                EndpointAdded?.Invoke(this, eventArgs);
            }
            else if (callEvent.Event.Equals("ActionCompleted"))
            {
                var eventArgs = callEvent.GetEventArgs<CallActionEventArgs>();
                var completion = CallActions[eventArgs.RequestGuid];
                completion?.Invoke(null);
                CallActions.Remove(eventArgs.RequestGuid);
            }
            else if (callEvent.Event.Equals("ActionFailed"))
            {
                var eventArgs = callEvent.GetEventArgs<CallActionFailedEventArgs>();
                var completion = CallActions[eventArgs.RequestGuid];
                completion?.Invoke(new Error(eventArgs.Code, eventArgs.Error));
                CallActions.Remove(eventArgs.RequestGuid);
            }
            else
            {
                Debug.LogError($"Unexpected Event {callEvent.Event} for Call {CallId}");
            }
        }

        protected Endpoint AddEndpoint(string endpointId)
        {
            var endpoint = CreateEndpoint(endpointId);
            endpoint.Removed += EndpointRemoved;
            _endpoints[endpointId] = endpoint;

            CallManager.Instance.AddEndpoint(this, endpoint);
            return endpoint;
        }

        protected abstract void DisposeImpl();

        private void EndpointRemoved(IEndpoint sender)
        {
            if (!_endpoints.ContainsKey(sender.EndpointId)) return;

            var endpoint = _endpoints[sender.EndpointId];
            endpoint.Dispose();
            _endpoints.Remove(sender.EndpointId);
        }

        protected abstract VideoStream CreateVideoStream(string streamId);
        protected abstract Endpoint CreateEndpoint(string endpointId);
    }
}
