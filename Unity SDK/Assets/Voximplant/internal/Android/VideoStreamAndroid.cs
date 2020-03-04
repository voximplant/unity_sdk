/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using CameraType = Voximplant.Unity.Hardware.CameraType;
using Object = UnityEngine.Object;

namespace Voximplant.Unity.@internal.Android
{
    internal class VideoStreamAndroid : VideoStream
    {
        private static readonly int RotationProperty = Shader.PropertyToID("rotation");
        private static readonly int LocalProperty = Shader.PropertyToID("local");
        private static readonly int FrontCameraProperty = Shader.PropertyToID("front");
        private AndroidJavaObject _nativeStream;
        private string _streamId;

        public override string StreamId => _streamId;
        public override int Width => _nativeStream.Call<int>("getWidth");
        public override int Height => _nativeStream.Call<int>("getHeight");
        public override int Rotation => _nativeStream.Call<int>("getRotation");
        protected override IntPtr TexturePtr => new IntPtr(_nativeStream.Call<int>("getTextureId"));

        internal void SetNativeStream(AndroidJavaObject nativeStream)
        {
            _nativeStream = nativeStream;
            _streamId = _nativeStream.Call<string>("getVideoStreamId");
        }

        protected override void UpdateImpl()
        {
            foreach (var o in Renderers)
            {
                switch (o)
                {
                    case RawImage rawImage:
                        rawImage.material.SetFloat(RotationProperty, -Mathf.Deg2Rad * Rotation);
                        rawImage.material.SetFloat(LocalProperty, Local ? 1.0f : 0.0f);
                        rawImage.material.SetFloat(FrontCameraProperty,
                            VoximplantSdk.GetCameraManager().Camera == CameraType.Front && Local ? 1.0f : 0.0f);
                        break;
                    case Material material:
                        material.SetFloat(RotationProperty, -Mathf.Deg2Rad * Rotation);
                        material.SetFloat(LocalProperty, Local ? 1.0f : 0.0f);
                        material.SetFloat(FrontCameraProperty,
                            VoximplantSdk.GetCameraManager().Camera == CameraType.Front && Local ? 1.0f : 0.0f);
                        break;
                }
            }
        }

        protected override void DisposeImpl()
        {
            foreach (var o in Renderers.ToList())
            {
                RemoveRenderer(o);
            }

            _nativeStream.Dispose();
        }

        public override void AddRenderer(RawImage target)
        {
            base.AddRenderer(target);
            target.material = new Material(Shader.Find("Hidden/Voximplant/VideoDecodeAndroid"));
        }

        public override void AddRenderer(Material target)
        {
            base.AddRenderer(target);
            target.shader = Shader.Find("Hidden/Voximplant/VideoDecodeAndroid");
        }

        public override void RemoveRenderer(Object target)
        {
            Debug.Log($"RemoveRenderer({target}");
            base.RemoveRenderer(target);
            switch (target)
            {
                case RawImage rawImage:
                    rawImage.material.mainTexture = null;
                    rawImage.material = null;
                    break;
                case Material material:
                    material.mainTexture = null;
                    material.shader = Shader.Find("Standard");
                    break;
            }
        }
    }
}