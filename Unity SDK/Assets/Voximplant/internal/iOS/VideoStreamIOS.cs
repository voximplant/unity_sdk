/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

namespace Voximplant.Unity.@internal.iOS
{
    internal class VideoStreamIOS : VideoStream
    {
        private string _streamId;

        public override string StreamId => _streamId;

        public override int Width => voximplant_video_stream_width(StreamId);

        public override int Height => voximplant_video_stream_height(StreamId);

        public override int Rotation => voximplant_video_stream_rotation(StreamId);

        protected override IntPtr TexturePtr => new IntPtr(voximplant_video_stream_texture_ptr(StreamId));

        internal void SetStreamId(string streamId)
        {
            _streamId = streamId;
        }

        [DllImport("__Internal")]
        private static extern int voximplant_video_stream_width(string streamId);

        [DllImport("__Internal")]
        private static extern int voximplant_video_stream_height(string streamId);

        [DllImport("__Internal")]
        private static extern int voximplant_video_stream_rotation(string streamId);

        [DllImport("__Internal")]
        private static extern long voximplant_video_stream_texture_ptr(string streamId);

        [DllImport("__Internal")]
        private static extern void voximplant_video_stream_update_texture(string streamId);

        public override void AddRenderer(RawImage target)
        {
            base.AddRenderer(target);
            target.material = new Material(Shader.Find("Hidden/Voximplant/VideoRotateIOS"));
        }

        public override void AddRenderer(Material target)
        {
            base.AddRenderer(target);
            target.shader = Shader.Find("Hidden/Voximplant/VideoRotateIOS");;
        }
        
        private static readonly int LocalProperty = Shader.PropertyToID("local");
        private static readonly int FrontCameraProperty = Shader.PropertyToID("front");
        
        protected override void UpdateImpl()
        {
            voximplant_video_stream_update_texture(StreamId);
            foreach (var o in Renderers)
            {
                switch (o)
                {
                    case RawImage rawImage:
                        rawImage.material.SetFloat(LocalProperty, Local ? 1.0f : 0.0f);
                        rawImage.material.SetFloat(FrontCameraProperty,
                            VoximplantSdk.GetCameraManager().Camera == Hardware.CameraType.Front && Local ? 1.0f : 0.0f);
                        break;
                    case Material material:
                        material.SetFloat(LocalProperty, Local ? 1.0f : 0.0f);
                        material.SetFloat(FrontCameraProperty,
                            VoximplantSdk.GetCameraManager().Camera == Hardware.CameraType.Front && Local ? 1.0f : 0.0f);
                        break;
                }
            }
        }

        [DllImport("__Internal")]
        private static extern void voximplant_video_stream_dispose_texture(string streamId);

        protected override void DisposeImpl()
        {
            voximplant_video_stream_dispose_texture(StreamId);
        }
    }
}
