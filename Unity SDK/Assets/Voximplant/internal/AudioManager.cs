/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System.Collections.Generic;
using UnityEngine;
using Voximplant.Unity.Hardware;
using Voximplant.Unity.Hardware.EventArgs;

namespace Voximplant.Unity.@internal
{
    internal abstract class AudioManager : IAudioManager
    {
        public abstract void SelectAudioDevice(AudioDevice audioDevice);
        public abstract AudioDevice AudioDevice { get; }
        public abstract IReadOnlyCollection<AudioDevice> AvailableAudioDevices { get; }
        public event SdkEventHandler<IAudioManager, AudioDeviceChangedEventArgs> AudioDeviceChanged;
        public event SdkEventHandler<IAudioManager, AudioDeviceListChangedEventArgs> AudioDevicesListChanged;
        public event SdkEventHandler<IAudioManager, AudioDeviceUnavailableEventArgs> AudioDeviceUnavailable; 

        public void OnEvent(SdkEvent audioManagerEvent)
        {
            if (audioManagerEvent.Event.Equals("AudioDeviceChanged"))
            {
                var eventArgs = audioManagerEvent.GetEventArgs<AudioDeviceChangedEventArgs>();
                AudioDeviceChanged?.Invoke(this, eventArgs);
            }
            else if (audioManagerEvent.Event.Equals("AudioDevicesListChanged"))
            {
                var eventArgs = audioManagerEvent.GetEventArgs<AudioDeviceListChangedEventArgs>();
                AudioDevicesListChanged?.Invoke(this, eventArgs);
            }
            else if (audioManagerEvent.Event.Equals("AudioDeviceUnavailable"))
            {
                var eventArgs = audioManagerEvent.GetEventArgs<AudioDeviceUnavailableEventArgs>();
                AudioDeviceUnavailable?.Invoke(this, eventArgs);
            }
            else
            {
                Debug.LogError($"Unexpected Event {audioManagerEvent.Event}");
            }
        }
    }
}
