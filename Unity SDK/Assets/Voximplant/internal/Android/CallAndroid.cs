/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Voximplant.Unity.Call;

namespace Voximplant.Unity.@internal.Android
{
    internal class CallAndroid : Call
    {
        private AndroidJavaObject _nativeCall;
        private AndroidJavaObject _customVideoSource;

        public CallAndroid(AndroidJavaObject nativeCall)
        {
            _nativeCall = nativeCall;
            CallId = _nativeCall.Call<string>("getCallId");
        }

        public override string CallId { get; }

        public override ulong Duration => _nativeCall.Call<ulong>("getCallDuration");

        protected override void DisposeImpl()
        {
            Debug.Log($"Call Disposed : {CallId}");
            _nativeCall.Dispose();
            _nativeCall = null;
        }

        protected override VideoStream CreateVideoStream(string streamId)
        {
            Debug.Log($"CreateVideoStream: {streamId}");
            var nativeStream = _nativeCall.Call<AndroidJavaObject>("getVideoStream", streamId);
            Debug.Log($"nativeStream: {nativeStream}");
            if (nativeStream == null) return null;

            var videoStream = VoximplantSdk.CreateVideoStream<VideoStreamAndroid>();
            videoStream.Local = true;
            videoStream.SetNativeStream(nativeStream);
            return videoStream;
        }

        protected override Endpoint CreateEndpoint(string endpointId)
        {
            Debug.Log($"CreateEndpoint: {endpointId}");
            var nativeEndpoint = _nativeCall.Call<AndroidJavaObject>("getEndpoint", endpointId);
            Debug.Log($"NativeEndpoint: {nativeEndpoint}");
            return nativeEndpoint == null ? null : new EndpointAndroid(nativeEndpoint);
        }

        public override void SyncEndpoints()
        {
            var nativeEndpoints = _nativeCall.Call<AndroidJavaObject[]>("getEndpoints");
            foreach (var nativeEndpoint in nativeEndpoints)
            {
                AddEndpoint(nativeEndpoint.Call<string>("getEndpointId"));
            }
        }

        public override void Start()
        {
            _nativeCall.Call("start");
        }

        public override void Answer(CallSettings callSettings)
        {
            _nativeCall.Call("answer", callSettings.VideoFlags.ReceiveVideo, callSettings.VideoFlags.SendVideo,
                callSettings.VideoCodec.ToString(), callSettings.CustomData,
                JsonHelper.ToJson(callSettings.ExtraHeaders));
        }

        public override void Reject(RejectMode rejectMode, IDictionary<string, string> headers)
        {
            _nativeCall.Call("reject", rejectMode.ToString(), JsonHelper.ToJson(headers));
        }

        public override void Hangup(IDictionary<string, string> headers)
        {
            _nativeCall.Call("hangup", JsonHelper.ToJson(headers));
        }

        public override void SendAudio(bool sendAudio)
        {
            _nativeCall.Call("sendAudio", sendAudio);
        }

        public override void SendVideo(bool sendVideo, Action<Error?> completion = null)
        {
            var requestGuid = Guid.NewGuid().ToString();
            if (!_nativeCall.Call<bool>("sendVideo", sendVideo, requestGuid))
            {
                completion?.Invoke(Error.Internal);
            }
            else
            {
                CallActions[requestGuid] = completion;
            }
        }

        public override void Hold(bool hold, Action<Error?> completion = null)
        {
            var requestGuid = Guid.NewGuid().ToString();
            if (!_nativeCall.Call<bool>("hold", hold, requestGuid))
            {
                completion?.Invoke(Error.Internal);
            }
            else
            {
                CallActions[requestGuid] = completion;
            }
        }

        public override void ReceiveVideo(Action<Error?> completion = null)
        {
            var requestGuid = Guid.NewGuid().ToString();
            if (!_nativeCall.Call<bool>("receiveVideo", requestGuid))
            {
                completion?.Invoke(Error.Internal);
            }
            else
            {
                CallActions[requestGuid] = completion;
            }
        }

        public override void SendDTMF(string dtmf)
        {
            _nativeCall.Call("sendDTMF", dtmf);
        }

        public override void SendInfo(string body, string mimeType, IDictionary<string, string> headers)
        {
            _nativeCall.Call("sendInfo", mimeType, body, JsonHelper.ToJson(headers));
        }

        public override void SendMessage(string message)
        {
            _nativeCall.Call("sendMessage", message);
        }

        private void SendFrameCallback(Texture2D texture, int width, int height)
        {
            _customVideoSource.Call("sendFrame", texture.GetNativeTexturePtr().ToInt32(), width, height);
        }

        protected override void SetVideoSourceImpl()
        {
            _customVideoSource = new AndroidJavaObject("com.voximplant.unity.VideoSourceWrapper");
            _nativeCall.Call("useCustomVideoSource", _customVideoSource);
            VideoSource.SetTextureCallback(SendFrameCallback);
        }
    }
}