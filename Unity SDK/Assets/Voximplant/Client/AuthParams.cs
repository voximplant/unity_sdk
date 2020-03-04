/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System;
using UnityEngine;

namespace Voximplant.Unity.Client
{
    /// <summary>
    /// Authentication parameters that may be used for login with access token.
    /// </summary>
    [Serializable]
    public class AuthParams
    {
        [SerializeField]
        private int accessExpired = default;

        [SerializeField]
        private string accessToken = default;

        [SerializeField]
        private int refreshExpired = default;

        [SerializeField]
        private string refreshToken = default;

        /// <summary>
        /// Time in seconds to access token expire.
        /// </summary>
        public int AccessExpire => accessExpired;

        /// <summary>
        /// Access token to use with <see cref="IClient.LoginWithToken(string, string)"/>.
        /// </summary>
        public string AccessToken => accessToken;

        /// <summary>
        /// Time in seconds to refresh token expire
        /// </summary>
        public int RefreshExpire => refreshExpired;

        /// <summary>
        /// Refresh token to use with <see cref="IClient.RefreshToken(string, string)"/>.
        ///
        /// <seealso cref="IClient.RefreshTokenSuccess"/>
        /// <seealso cref="IClient.RefreshTokenFailed"/>
        /// </summary>
        public string RefreshToken => refreshToken;
    }
}
