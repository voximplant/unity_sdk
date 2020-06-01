/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using Voximplant.Unity.Call;
using Object = UnityEngine.Object;

namespace Voximplant.Unity.@internal.iOS
{
    internal class CallIOS : Call
    {
        private Texture2D _targetTexture2D;

        public CallIOS(string callId)
        {
            CallId = callId;
        }

        public override string CallId { get; }

        public override ulong Duration => (ulong) voximplant_call_duration(CallId);

        [DllImport("__Internal")]
        private static extern string voximplant_call_endpoints(string callId);

        [DllImport("__Internal")]
        private static extern double voximplant_call_duration(string callId);

        [DllImport("__Internal")]
        private static extern void voximplant_call_start(string callId);

        public override void Start()
        {
            voximplant_call_start(CallId);
        }

        [DllImport("__Internal")]
        private static extern void voximplant_call_answer(string callId, bool receiveVideo, bool sendVideo,
            VideoCodec videoCodec, string customData, string headers);

        public override void Answer(CallSettings callSettings)
        {
            voximplant_call_answer(CallId, callSettings.VideoFlags.ReceiveVideo, callSettings.VideoFlags.SendVideo,
                callSettings.VideoCodec, callSettings.CustomData, JsonHelper.ToJson(callSettings.ExtraHeaders));
        }

        [DllImport("__Internal")]
        private static extern void voximplant_call_reject(string callId, RejectMode mode, string headers);

        public override void Reject(RejectMode rejectMode, IDictionary<string, string> headers)
        {
            voximplant_call_reject(CallId, rejectMode, JsonHelper.ToJson(headers));
        }

        [DllImport("__Internal")]
        private static extern void voximplant_call_hangup(string callId, string headers);

        public override void Hangup(IDictionary<string, string> headers)
        {
            voximplant_call_hangup(CallId, JsonHelper.ToJson(headers));
        }

        [DllImport("__Internal")]
        private static extern void voximplant_call_set_send_audio(string callId, bool sendAudio);

        public override void SendAudio(bool sendAudio)
        {
            voximplant_call_set_send_audio(CallId, sendAudio);
        }

        [DllImport("__Internal")]
        private static extern bool voximplant_call_set_send_video(string callId, bool sendVideo, string requestGuid);

        public override void SendVideo(bool sendVideo, Action<Error?> completion = null)
        {
            var requestGuid = Guid.NewGuid().ToString();
            if (!voximplant_call_set_send_video(CallId, sendVideo, requestGuid))
            {
                completion?.Invoke(Error.Internal);
            }
            else
            {
                CallActions[requestGuid] = completion;
            }
        }

        [DllImport("__Internal")]
        private static extern bool voximplant_call_set_hold(string callId, bool hold, string requestGuid);

        public override void Hold(bool hold, Action<Error?> completion = null)
        {
            var requestGuid = Guid.NewGuid().ToString();
            if (!voximplant_call_set_hold(CallId, hold, requestGuid))
            {
                completion?.Invoke(Error.Internal);
            }
            else
            {
                CallActions[requestGuid] = completion;
            }
        }

        [DllImport("__Internal")]
        private static extern bool voximplant_call_start_receive_video(string callId, string requestGuid);

        public override void ReceiveVideo(Action<Error?> completion = null)
        {
            var requestGuid = Guid.NewGuid().ToString();
            if (!voximplant_call_start_receive_video(CallId, requestGuid))
            {
                completion?.Invoke(Error.Internal);
            }
            else
            {
                CallActions[requestGuid] = completion;
            }
        }

        [DllImport("__Internal")]
        private static extern void voximplant_call_send_dtmf(string callId, string dtmf);

        public override void SendDTMF(string dtmf)
        {
            voximplant_call_send_dtmf(CallId, dtmf);
        }

        [DllImport("__Internal")]
        private static extern void voximplant_call_send_info(string callId, string content, string mimeType,
            string headers);

        public override void SendInfo(string body, string mimeType, IDictionary<string, string> headers)
        {
            voximplant_call_send_info(CallId, body, mimeType, JsonHelper.ToJson(headers));
        }

        [DllImport("__Internal")]
        private static extern void voximplant_call_send_message(string callId, string message);

        public override void SendMessage(string message)
        {
            voximplant_call_send_message(CallId, message);
        }

        [DllImport("__Internal")]
        private static extern string voximplant_call_video_streams(string callId);

        [DllImport("__Internal")]
        private static extern string voximplant_call_custom_video_source_create(string callId, int width, int height,
            int fps);

        [DllImport("__Internal")]
        private static extern void voximplant_call_custom_video_source_send_frame(byte[] data, string sourceId);

        [DllImport("__Internal")]
        private static extern void voximplant_call_custom_video_source_release(string sourceId);

        private void SendFrameCallback(Texture2D texture2D, int width, int height)
        {
            if (_targetTexture2D == null)
            {
                _targetTexture2D = new Texture2D(width, height, TextureFormat.BGRA32, false);
            }

            texture2D = FlipTexture(texture2D);
            var pixels = texture2D.GetPixels32();
            _targetTexture2D.SetPixels32(pixels);
            Object.Destroy(texture2D);
            var rawTextureData = _targetTexture2D.GetRawTextureData();
            voximplant_call_custom_video_source_send_frame(rawTextureData, VideoSource.SourceId);
        }

        private static Texture2D FlipTexture(Texture2D original)
        {
            var flipped = new Texture2D(original.width, original.height);
            var xN = original.width;
            var yN = original.height;
            for (var i = 0; i < xN; i++)
            {
                for (var j = 0; j < yN; j++)
                {
                    flipped.SetPixel(i, yN - j - 1, original.GetPixel(i, j));
                }
            }
            flipped.Apply();
            return flipped;
        }

        protected override void SetVideoSourceImpl()
        {
            VideoSource.SourceId = voximplant_call_custom_video_source_create(CallId, VideoSource.Width,
                VideoSource.Height, (int) Math.Round(VideoSource.FPS));
            VideoSource.SetTextureCallback(SendFrameCallback);
        }

        protected override void DisposeImpl()
        {
            if (_targetTexture2D != null)
            {
                Object.Destroy(_targetTexture2D);
            }

            if (VideoSource != null)
            {
                voximplant_call_custom_video_source_release(VideoSource.SourceId);
            }
        }

        protected override VideoStream CreateVideoStream(string streamId)
        {
            var videoStream = VoximplantSdk.CreateVideoStream<VideoStreamIOS>();
            videoStream.SetStreamId(streamId);
            videoStream.Local = true;
            return videoStream;
        }

        protected override Endpoint CreateEndpoint(string endpointId)
        {
            return new EndpointIOS(endpointId);
        }

        public override void SyncEndpoints()
        {
            var endpointIds = JsonHelper.FromJson<string>(voximplant_call_endpoints(CallId));
            foreach (var endpointId in endpointIds)
            {
                AddEndpoint(endpointId);
            }
        }
    }
}