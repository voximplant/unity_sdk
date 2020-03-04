/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Voximplant.Unity.Hardware;

namespace Voximplant.Unity.@internal.iOS
{
    internal class AudioManagerIOS : AudioManager
    {
        public override AudioDevice AudioDevice => (AudioDevice) voximplant_audio_manager_current_audio_device();

        public override IReadOnlyCollection<AudioDevice> AvailableAudioDevices
        {
            get
            {
                var audioDevices = JsonHelper.FromJson<int>(voximplant_audio_manager_available_audio_devices());
                var result = new HashSet<AudioDevice>();
                foreach (var deviceId in audioDevices)
                {
                    result.Add((AudioDevice) deviceId);
                }

                return result;
            }
        }

        [DllImport("__Internal")]
        private static extern void voximplant_audio_manager_select_audio_device(int audioDevice);

        public override void SelectAudioDevice(AudioDevice audioDevice)
        {
            voximplant_audio_manager_select_audio_device((int) audioDevice);
        }

        [DllImport("__Internal")]
        private static extern int voximplant_audio_manager_current_audio_device();

        [DllImport("__Internal")]
        private static extern string voximplant_audio_manager_available_audio_devices();
    }
}
