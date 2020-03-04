/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using Voximplant.Unity.Call;
using Voximplant.Unity.Call.EventArgs;

namespace Voximplant.Unity.@internal
{
    internal abstract class Endpoint : IEndpoint, IDisposable
    {
        private readonly Dictionary<string, VideoStream> _videoStreams = new Dictionary<string, VideoStream>();

        public void Dispose()
        {
            foreach (var videoStream in _videoStreams.Values)
            {
                videoStream.Dispose();
            }

            DisposeImpl();
        }

        public abstract string EndpointId { get; }
        public abstract string UserName { get; }
        public abstract string UserDisplayName { get; }
        public abstract string SipUri { get; }
        public IReadOnlyCollection<IVideoStream> VideoStreams => _videoStreams.Values;

        public event SdkEventHandler<IEndpoint, EndpointRemoteVideoStreamAddedEventArgs> RemoteVideoStreamAdded;
        public event SdkEventHandler<IEndpoint, EndpointRemoteVideoStreamRemovedEventArgs> RemoteVideoStreamRemoved;
        public event SdkEventHandler<IEndpoint> Removed;
        public event SdkEventHandler<IEndpoint> InfoUpdated;

        protected abstract VideoStream CreateVideoStream(string streamId);

        public void OnEvent(EndpointSdkEvent endpointEvent)
        {
            Debug.Log($"Event {endpointEvent.Event} received by Endpoint {EndpointId}");
            if (endpointEvent.Event.Equals("Removed"))
            {
                Dispose();
                Removed?.Invoke(this);
            }
            else if (endpointEvent.Event.Equals("InfoUpdated"))
            {
                InfoUpdated?.Invoke(this);
            }
            else if (endpointEvent.Event.Equals("RemoteVideoStreamAdded"))
            {
                var eventArgs = endpointEvent.GetEventArgs<EndpointRemoteVideoStreamAddedEventArgs>();
                var videoStream = CreateVideoStream(eventArgs.streamId);
                _videoStreams[eventArgs.streamId] = videoStream;
                eventArgs.VideoStream = videoStream;

                RemoteVideoStreamAdded?.Invoke(this, eventArgs);
            }
            else if (endpointEvent.Event.Equals("RemoteVideoStreamRemoved"))
            {
                var eventArgs = endpointEvent.GetEventArgs<EndpointRemoteVideoStreamRemovedEventArgs>();
                if (!_videoStreams.ContainsKey(eventArgs.streamId)) return;

                var videoStream = _videoStreams[eventArgs.streamId];
                _videoStreams.Remove(eventArgs.streamId);

                eventArgs.VideoStream = videoStream;
                videoStream.Dispose();

                RemoteVideoStreamRemoved?.Invoke(this, eventArgs);
            }
            else
            {
                Debug.LogError($"Unexpected Event {endpointEvent.Event} for Endpoint {EndpointId}");
            }
        }

        protected abstract void DisposeImpl();
    }
}
