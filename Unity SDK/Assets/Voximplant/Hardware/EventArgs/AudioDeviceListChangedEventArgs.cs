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
    /// Provide data for the <see cref="IAudioManager.AudioDevicesListChanged"/> event.
    /// </summary>
    [Serializable]
    public class AudioDeviceListChangedEventArgs : System.EventArgs
    {
        private IReadOnlyCollection<AudioDevice> _audioDevices;

        [SerializeField]
        private IList<int> devices = default;

        /// <summary>
        /// A collection with newly available audio devices.
        /// </summary>
        public IReadOnlyCollection<AudioDevice> Devices =>
            _audioDevices ?? (_audioDevices = devices.Select(device => (AudioDevice) device).ToList());
    }
}