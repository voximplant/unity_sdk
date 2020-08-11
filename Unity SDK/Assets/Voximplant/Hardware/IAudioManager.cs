/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System;
using System.Collections.Generic;
using Voximplant.Unity.Hardware.EventArgs;
using Voximplant.Unity.@internal;

namespace Voximplant.Unity.Hardware
{
    /// <summary>
    /// Interface that may be used to manage audio devices, i.e. see current active device, select another active device and get the list of available devices.
    /// </summary>
    public interface IAudioManager
    {
        /// <summary>
        /// Current active audio device during the call or audio device that will be used for a call if there is no calls at this moment.
        /// </summary>
        AudioDevice AudioDevice { get; }

        /// <summary>
        /// A collection of available audio devices.
        /// </summary>
        IReadOnlyCollection<AudioDevice> AvailableAudioDevices { get; }

        /// <summary>
        /// Change the selection of the current active audio device. There are two cases:
        /// <list type="bullet">
        /// <item><description>before a call. The method doesn't <b>activate</b> an audio device, it just <b>selects</b> (i.e. points to) the audio device that will be activated.</description></item>
        /// <item><description>during a call. If the selected audio device is available, the method <b>activates</b> this audio device.</description></item>
        /// </list>
        ///
        /// When the call is ended, selected audio device is reset to the default one.
        /// Please note that active audio device can be later changed if new device is connected.
        /// In this case <see cref="IAudioManager.AudioDeviceChanged"/> will be triggered to notify about new active device.
        /// </summary>
        /// <param name="audioDevice">Audio device to be set active.</param>
        void SelectAudioDevice(AudioDevice audioDevice);

        /// <summary>
        /// Invoked when active audio device or audio device that will be used for a further call is changed.
        ///
        /// If the event is triggered during the call, <b>currentAudioDevice</b> is the audio device that is currently used.
        ///
        /// If the event is triggered when there is no call, <b>currentAudioDevice</b> is the audio device that will be used for the next call.
        /// </summary>
        event SdkEventHandler<IAudioManager, AudioDeviceChangedEventArgs> AudioDeviceChanged;

        /// <summary>
        /// Invoked when a new audio device is connected or previously connected audio device is disconnected.
        /// </summary>
        event SdkEventHandler<IAudioManager, AudioDeviceListChangedEventArgs> AudioDevicesListChanged;

        /// <summary>
        /// Invoked when audio device can not be selected due to it is not available at this moment.
        /// </summary>
        event SdkEventHandler<IAudioManager, AudioDeviceUnavailableEventArgs> AudioDeviceUnavailable;
    }
}