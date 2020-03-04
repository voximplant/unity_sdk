/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voximplant.Unity.@internal;

namespace Voximplant.Unity.Hardware.EventArgs
{
    /// <summary>
    /// Event arguments for <see cref="IAudioManager.AudioDevicesListChanged"/>.
    /// </summary>
    [Serializable]
    public class AudioDeviceListChangedEventArgs : System.EventArgs
    {
        private IReadOnlyCollection<AudioDevice> _audioDevices;

        [SerializeField]
        private IList<int> devices = default;

        /// <summary>
        /// Collection of newly available audio devices.
        /// </summary>
        public IReadOnlyCollection<AudioDevice> Devices =>
            _audioDevices ?? (_audioDevices = devices.Select(device => (AudioDevice) device).ToList());
    }
}