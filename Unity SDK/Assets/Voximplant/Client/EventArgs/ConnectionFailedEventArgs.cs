/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System;
using UnityEngine;

namespace Voximplant.Unity.Client.EventArgs
{
    /// <summary>
    /// Provide data for the <see cref="IClient.ConnectionFailed"/> event.
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