using System;
using UnityEngine;

namespace Voximplant.Unity.Hardware.EventArgs
{
    /// <summary>
    /// Event arguments for <see cref="IAudioManager.AudioDeviceUnavailable"/>.
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