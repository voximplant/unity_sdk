/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using Voximplant.Unity.@internal;

namespace Voximplant.Unity.Hardware
{
    /// <summary>
    /// Enum representing audio devices.
    /// </summary>
    public enum AudioDevice
    {
        /// <summary>
        /// No audio device, generally indicates that something is wrong with audio device selection.
        ///
        /// Should not be selected via <see cref="IAudioManager.SelectAudioDevice(AudioDevice)"/>.
        /// </summary>
        None,

        /// <summary>
        /// Earpiece.
        /// </summary>
        Earpiece,

        /// <summary>
        /// Speaker.
        /// </summary>
        Speaker,

        /// <summary>
        /// Wired headset.
        /// </summary>
        WiredHeadset,

        /// <summary>
        /// Bluetooth headset.
        /// </summary>
        Bluetooth,
    }
}