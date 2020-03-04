/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

namespace Voximplant.Unity.Client
{
    /// <summary>
    /// <see cref="IClient"/> states.
    /// </summary>
    public enum ClientState
    {
        /// <summary>
        /// The client is currently disconnected.
        /// </summary>
        Disconnected,

        /// <summary>
        /// The client is currently connecting.
        /// </summary>
        Connecting,

        /// <summary>
        /// The client is currently connected.
        /// </summary>
        Connected,

        /// <summary>
        /// The client is currently logging in.
        /// </summary>
        LoggingIn,

        /// <summary>
        /// The client is currently logged in.
        /// </summary>
        LoggedIn
    }
}