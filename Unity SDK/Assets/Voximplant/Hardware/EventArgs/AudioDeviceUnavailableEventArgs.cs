using System;
using UnityEngine;

namespace Voximplant.Unity.Hardware.EventArgs
{
    /// <summary>
    /// Provide data for the <see cref="IAudioManager.AudioDeviceUnavailable"/> event.
    /// </summary>
    [Serializable]
    public class AudioDeviceUnavailableEventArgs : System.EventArgs
    {
        [SerializeField]
        private int device = default;
        
        /// <summary>
        /// Failed audio device.
        /// </summary>
        public AudioDevice Device => (AudioDevice) device;
    }
}