/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System;
using UnityEngine;

namespace Voximplant.Unity.Client.EventArgs
{
    /// <summary>
    /// Event arguments for <see cref="IClient.ConnectionFailed"/>.
    /// </summary>
    [Serializable]
    public class ConnectionFailedEventArgs : System.EventArgs
    {
        [SerializeField]
        private int code = default;

        [SerializeField]
        private string error = default;

        /// <summary>
        /// Failure reason description 
        /// </summary>
        public Error Error => new Error(code, error);
    }
}