/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System;
using UnityEngine;

namespace Voximplant.Unity.Client.EventArgs
{
    /// <summary>
    /// Event arguments for <see cref="IClient.OneTimeKeyGenerated"/>.
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
