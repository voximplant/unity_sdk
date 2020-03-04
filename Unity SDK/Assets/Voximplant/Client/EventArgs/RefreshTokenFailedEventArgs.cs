/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System;
using UnityEngine;

namespace Voximplant.Unity.Client.EventArgs
{
    /// <summary>
    /// Event arguments for <see cref="IClient.RefreshTokenFailed"/>.
    /// </summary>
    [Serializable]
    public class RefreshTokenFailedEventArgs : System.EventArgs
    {
        [SerializeField]
        private int code = default;

        [SerializeField]
        private string error = default;

        /// <summary>
        /// Failure reason.
        /// </summary>
        public Error Error => new Error(code, error);
    }
}