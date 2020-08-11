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
        /// Seconds before the access token expiration.
        /// </summary>
        public int AccessExpire => accessExpired;

        /// <summary>
        /// You can use this token for <see cref="IClient.LoginWithToken(string, string)"/> before <see cref="AccessExpire"/>.
        /// </summary>
        public string AccessToken => accessToken;

        /// <summary>
        /// Seconds before the refresh token expiration.
        /// </summary>
        public int RefreshExpire => refreshExpired;

        /// <summary>
        /// You can use this token for <see cref="IClient.RefreshToken(string, string)"/> before <see cref="RefreshExpire"/>.
        /// </summary>
        public string RefreshToken => refreshToken;
    }
}
