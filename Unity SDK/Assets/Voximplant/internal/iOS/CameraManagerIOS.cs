/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System.Runtime.InteropServices;
using Voximplant.Unity.Hardware;

namespace Voximplant.Unity.@internal.iOS
{
    internal class CameraManagerIOS : ICameraManager
    {
        private CameraType _cameraType = CameraType.Front;

        public CameraType Camera
        {
            get => _cameraType;
            set
            {
                _cameraType = value;
                voximplant_camera_manager_switch_camera((int) _cameraType);
            }
        }

        public void SetCameraResolution(int width, int height)
        {
            voximplant_camera_manager_set_camera_resolution(width, height);
        }

        [DllImport("__Internal")]
        private static extern void voximplant_camera_manager_switch_camera(int cameraType);

        [DllImport("__Internal")]
        private static extern void voximplant_camera_manager_set_camera_resolution(int width, int height);
    }
}
