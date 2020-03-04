/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Voximplant.Unity.@internal
{
    internal class CallManager
    {
        private static CallManager _instance;

        private readonly Dictionary<string, Call> _calls = new Dictionary<string, Call>();
        private readonly Dictionary<string, Endpoint> _endpoints = new Dictionary<string, Endpoint>();
        private readonly Dictionary<string, string> _endpointToCall = new Dictionary<string, string>();
        private readonly Dictionary<string, VideoStream> _localVideoStreams = new Dictionary<string, VideoStream>();
        private readonly Dictionary<string, VideoStream> _remoteVideoStreams = new Dictionary<string, VideoStream>();
        private readonly Dictionary<string, string> _videoStreamToCall = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _videoStreamToEndpoint = new Dictionary<string, string>();

        private CallManager()
        {
        }

        public static CallManager Instance => _instance ?? (_instance = new CallManager());

        public void AddCall(Call call)
        {
            _calls[call.CallId] = call;
        }

        public Call GetCall(string callId)
        {
            return _calls[callId];
        }

        public void RemoveCall(string callId)
        {
            foreach (var pair in _endpointToCall.Where(pair => pair.Value == callId).ToList())
            {
                RemoveEndpoint(pair.Key);
                _endpointToCall.Remove(pair.Key);
            }

            foreach (var pair in _videoStreamToCall.Where(pair => pair.Value == callId).ToList())
            {
                RemoveLocalVideoStream(pair.Key);
            }

            _calls[callId].Dispose();
            _calls.Remove(callId);
        }

        public void AddEndpoint(Call call, Endpoint endpoint)
        {
            Debug.Log($"AddEndpoint: {call.CallId} {endpoint.EndpointId}");
            _endpoints[endpoint.EndpointId] = endpoint;
            _endpointToCall[endpoint.EndpointId] = call.CallId;
        }

        public Endpoint GetEndpoint(string endpointId)
        {
            return _endpoints[endpointId];
        }

        public void SyncEndpoints(Call call, IEnumerable<Endpoint> endpoints)
        {
            foreach (var pair in _endpointToCall.Where(pair => pair.Value == call.CallId).ToList())
            {
                RemoveEndpoint(pair.Key);
            }

            foreach (var endpoint in endpoints)
            {
                AddEndpoint(call, endpoint);
            }
        }

        public void RemoveEndpoint(string endpointId)
        {
            Debug.Log($"RemoveEndpoint {endpointId}");
            foreach (var pair in _videoStreamToEndpoint.Where(pair => pair.Value == endpointId).ToList())
            {
                RemoveRemoteVideoStream(pair.Key);
                _videoStreamToEndpoint.Remove(pair.Key);
            }

            _endpoints.Remove(endpointId);
        }

        public void AddVideoStream(Call call, VideoStream videoStream)
        {
            Debug.Log($"AddLocalVideoStream {call.CallId} {videoStream.StreamId}");
            _localVideoStreams[videoStream.StreamId] = videoStream;
            _videoStreamToCall[videoStream.StreamId] = call.CallId;
        }

        public VideoStream GetLocalVideoStream(string streamId)
        {
            return _localVideoStreams[streamId];
        }

        public void SyncLocalVideoStreams(Call call, IEnumerable<VideoStream> videoStreams)
        {
            foreach (var pair in _videoStreamToCall.Where(pair => pair.Value == call.CallId).ToList())
            {
                RemoveLocalVideoStream(pair.Key);
            }

            foreach (var videoStream in videoStreams)
            {
                AddVideoStream(call, videoStream);
            }
        }

        public void RemoveLocalVideoStream(string streamId)
        {
            Debug.Log($"RemoveLocalVideoStream: {streamId}");
            var videoStream = _localVideoStreams[streamId];
            videoStream.Dispose();

            _localVideoStreams.Remove(streamId);
            _videoStreamToCall.Remove(streamId);
        }

        public void AddVideoStream(Endpoint endpoint, VideoStream videoStream)
        {
            Debug.Log($"AddVideoStream remote {endpoint.EndpointId} {videoStream.StreamId}");
            _remoteVideoStreams[videoStream.StreamId] = videoStream;
            _videoStreamToEndpoint[videoStream.StreamId] = endpoint.EndpointId;
        }

        public VideoStream GetRemoteVideoStream(string streamId)
        {
            return _remoteVideoStreams[streamId];
        }

        public void SyncRemoteVideoStreams(Endpoint endpoint, IEnumerable<VideoStream> videoStreams)
        {
            foreach (var pair in _videoStreamToEndpoint.Where(pair => pair.Value == endpoint.EndpointId).ToList())
            {
                RemoveRemoteVideoStream(pair.Key);
            }

            foreach (var videoStream in videoStreams)
            {
                AddVideoStream(endpoint, videoStream);
            }
        }

        public void RemoveRemoteVideoStream(string streamId)
        {
            Debug.Log($"RemoveRemoteVideoStream: {streamId}");
            var videoStream = _remoteVideoStreams[streamId];
            videoStream.Dispose();

            _remoteVideoStreams.Remove(streamId);
            _videoStreamToEndpoint.Remove(streamId);
        }
    }
}