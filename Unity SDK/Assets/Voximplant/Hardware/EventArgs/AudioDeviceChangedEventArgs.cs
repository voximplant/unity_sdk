/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System;
using UnityEngine;

namespace Voximplant.Unity.Hardware.EventArgs
{
    /// <summary>
    /// Event arguments for <see cref="IAudioManager.AudioDeviceChanged"/>.
    /// </summary>
    [Serializable]
    public class AudioDeviceChangedEventArgs : System.EventArgs
    {
        [SerializeField] 
        private int device = default;

        /// <summary>
        /// Activated audio device.
        /// </summary>
        public AudioDevice Device => (AudioDevice) device;
    }
}
