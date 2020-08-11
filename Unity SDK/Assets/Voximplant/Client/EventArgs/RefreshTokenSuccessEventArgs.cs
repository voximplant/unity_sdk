/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System;
using UnityEngine;

namespace Voximplant.Unity.Client.EventArgs
{
    /// <summary>
    /// Provide data for the <see cref="IClient.RefreshTokenSuccess"/> event.
    /// </summary>
    [Serializable]
    public class RefreshTokenSuccessEventArgs : System.EventArgs
    {
        [SerializeField]
        private AuthParams authParams = default;

        /// <summary>
        /// Auth parameters that can be used to log in using <see cref="Voximplant.Unity.Client.AuthParams.AccessToken"/>.
        /// </summary>
        public AuthParams AuthParams => authParams;
    }
}
