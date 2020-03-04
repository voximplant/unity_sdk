/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using Voximplant.Unity.Hardware;

namespace Voximplant.Unity.@internal.Android
{
    internal class AudioManagerAndroid : AudioManager
    {
        private readonly AndroidJavaObject _nativeAudioManager;

        internal AudioManagerAndroid()
        {
            _nativeAudioManager = new AndroidJavaObject("com.voximplant.unity.AudioManagerModule");
        }

        public override AudioDevice AudioDevice
        {
            get
            {
                var audioDevice = _nativeAudioManager.Call<string>("getActiveDevice");
                audioDevice = audioDevice.Replace("_", "");
                return Enum.TryParse(audioDevice, true, out AudioDevice device) ? device : AudioDevice.None;
            }
        }

        public override IReadOnlyCollection<AudioDevice> AvailableAudioDevices
        {
            get
            {
                var result = new HashSet<AudioDevice>();
                var audioDevices = _nativeAudioManager.Call<string[]>("getAudioDevices");
                foreach (var audioDevice in audioDevices)
                {
                    var deviceName = audioDevice.Replace("_", "");
                    if (Enum.TryParse(deviceName, true, out AudioDevice device))
                    {
                        result.Add(device);
                    }
                }

                return result;
            }
        }

        public override void SelectAudioDevice(AudioDevice audioDevice)
        {
            _nativeAudioManager.Call("selectAudioDevice", audioDevice.ToString());
        }
    }
}
