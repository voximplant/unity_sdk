/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using UnityEngine;
using Voximplant.Unity.Hardware;
using CameraType = Voximplant.Unity.Hardware.CameraType;

namespace Voximplant.Unity.@internal.Android
{
    internal class CameraManagerAndroid : ICameraManager
    {
        private readonly AndroidJavaObject _nativeAudioManager;
        private CameraType _cameraType = CameraType.Front;

        internal CameraManagerAndroid()
        {
            _nativeAudioManager = new AndroidJavaObject("com.voximplant.unity.CameraManagerModule");
        }

        public CameraType Camera
        {
            get => _cameraType;
            set
            {
                _cameraType = value;
                _nativeAudioManager.Call("switchCamera", (int) _cameraType);
            }
        }

        public void SetCameraResolution(int width, int height)
        {
            _nativeAudioManager.Call("setCameraResolution", width, height);
        }
    }
}
