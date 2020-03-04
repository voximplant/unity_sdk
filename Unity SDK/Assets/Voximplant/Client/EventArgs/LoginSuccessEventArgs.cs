/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System;
using UnityEngine;

namespace Voximplant.Unity.Client.EventArgs
{
    /// <summary>
    /// Event arguments for <see cref="IClient.LoginSuccess"/>.
    /// </summary>
    [Serializable]
    public class LoginSuccessEventArgs : System.EventArgs
    {
        [SerializeField]
        private AuthParams authParams = default;

        [SerializeField]
        private string displayName = default;

        /// <summary>
        /// Display name of logged in user.
        /// </summary>
        public string DisplayName => displayName;

        /// <summary>
        /// Auth parameters that can be used to login using access token.
        /// </summary>
        public AuthParams AuthParams => authParams;
    }
}