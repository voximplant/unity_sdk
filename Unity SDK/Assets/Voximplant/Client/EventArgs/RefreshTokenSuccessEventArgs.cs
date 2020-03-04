/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System;
using UnityEngine;

namespace Voximplant.Unity.Client.EventArgs
{
    /// <summary>
    /// Event arguments for <see cref="IClient.RefreshTokenSuccess"/>.
    /// </summary>
    [Serializable]
    public class RefreshTokenSuccessEventArgs : System.EventArgs
    {
        [SerializeField]
        private AuthParams authParams = default;

        /// <summary>
        /// Auth parameters that can be used to login using access token.
        /// </summary>
        public AuthParams AuthParams => authParams;
    }
}
