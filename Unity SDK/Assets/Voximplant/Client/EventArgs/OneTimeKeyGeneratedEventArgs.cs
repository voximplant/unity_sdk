/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System;
using UnityEngine;

namespace Voximplant.Unity.Client.EventArgs
{
    /// <summary>
    /// Provide data for the <see cref="IClient.OneTimeKeyGenerated"/> event.
    /// </summary>
    [Serializable]
    public class OneTimeKeyGeneratedEventArgs : System.EventArgs
    {
        [SerializeField]
        private string oneTimeKey = default;

        /// <summary>
        /// One time key.
        /// </summary>
        public string OneTimeKey => oneTimeKey;
    }
}
