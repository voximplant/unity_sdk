/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Voximplant.Unity.Call;
using Voximplant.Unity.Client.EventArgs;

namespace Voximplant.Unity.Client
{
    /// <summary>
    /// Interface that may be used to connect and login to the Voximplant Cloud, and make and receive audio and video calls.
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Current client state.
        /// </summary>
        ClientState State { get; }

        /// <summary>
        /// Connect to the Voximplant cloud.
        ///
        /// Connectivity check is disabled.
        /// </summary>
        void Connect();

        /// <summary>
        /// Connect to the Voximplant cloud with additional configuration.
        /// </summary>
        /// <param name="connectivityCheck">Checks whether UDP traffic will flow correctly between a device and the Voximplant cloud. This check reduces connection speed.</param>
        /// <param name="gateways">A collection of server names of particular media gateways for connection.</param>
        void Connect(bool connectivityCheck, [CanBeNull] ICollection<string> gateways = null);

        /// <summary>
        /// Close the connection with the Voximplant Cloud.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Log in to the specified Voximplant application with password.
        /// </summary>
        /// <param name="username">Full user name, including app and account name, like someuser@someapp.youraccount.voximplant.com.</param>
        /// <param name="password">User password</param>
        void Login(string username, string password);

        /// <summary>
        /// Perform log in using the specified username and the <see cref="AuthParams.AccessToken"/>.
        /// </summary>
        /// <param name="username">Full user name, including app and account name, like someuser@someapp.youraccount.voximplant.com.</param>
        /// <param name="token">Access token that was obtained in <see cref="IClient.LoginSuccess"/> event.</param>
        void LoginWithToken(string username, string token);

        /// <summary>
        /// Perform login using one time key that was generated before.
        /// </summary>
        /// <param name="username">Full user name, including app and account name, like someuser@someapp.youraccount.voximplant.com.</param>
        /// <param name="hash">Hash that was generated using following formula:
        /// <code>MD5(oneTimeKey + "|" + MD5(user + ":voximplant.com:"+password))</code>
        ///
        /// Please note that here user is just a user name, without app name, account name or anything else after "@".
        /// 
        /// So if you pass myuser@myapp.myacc.voximplant.com as a username, you should only use myuser while computing this hash.
        /// </param>
        void LoginWithOneTimeKey(string username, string hash);

        /// <summary>
        /// Generates one time login key to be used for automated login process.
        /// </summary>
        /// <param name="username">Full user name, including app and account name, like someuser@someapp.youraccount.voximplant.com.</param>
        void RequestOneTimeKey(string username);

        /// <summary>
        /// Perform refresh of the login tokens required for login using <see cref="AuthParams.AccessToken"/>.
        /// </summary>
        /// <param name="username">Full user name, including app and account name, like someuser@someapp.youraccount.voximplant.com.</param>
        /// <param name="refreshToken"><see cref="AuthParams.RefreshToken"/> that was obtained in <see cref="IClient.LoginSuccess"/> event.</param>
        void RefreshToken(string username, string refreshToken);

        /// <summary>
        /// Create a new call instance. Call must be then started using <see cref="ICall.Start()"/>.
        /// </summary>
        /// <param name="username">SIP URI, username or phone number to make call to. Actual routing is then performed by VoxEngine scenario.</param>
        /// <param name="callSettings">Call settings with additional call parameters, such as preferred video codec, custom data, extra headers etc.</param>
        /// <returns>Call instance or null if the client is not logged in.</returns>
        [CanBeNull]
        ICall Call(string username, CallSettings callSettings);

        /// <summary>
        /// Create call to a dedicated conference without a proxy session.
        ///
        /// For details see <a href="https://voximplant.com/docs/tutorials/video-conference-through-voximplant-media-servers">the video conferencing guide</a>.
        /// </summary>
        /// <param name="conference">The number to call. For SIP compatibility reasons it should be a non-empty string even if the number itself is not used by a Voximplant cloud scenario.</param>
        /// <param name="callSettings">Call settings with additional call parameters, such as preferred video codec, custom data, extra headers etc.</param>
        /// <returns>Call instance or null if the client is not logged in.</returns>
        [CanBeNull]
        ICall CallConference(string conference, CallSettings callSettings);

        /// <summary>
        /// Invoked after connection to the Voximplant Cloud was established successfully.
        /// </summary>
        event SdkEventHandler<IClient> Connected;

        /// <summary>
        /// Invoked if connection to then Voximplant Cloud couldn't be established.
        /// </summary>
        event SdkEventHandler<IClient, ConnectionFailedEventArgs> ConnectionFailed;

        /// <summary>
        /// Invoked if connection to the Voximplant Cloud was closed as a result of <see cref="IClient.Disconnect()"/> method call or due to network problems
        /// </summary>
        event SdkEventHandler<IClient> Disconnected;

        /// <summary>
        /// Invoked when login process finished successfully.
        /// </summary>
        event SdkEventHandler<IClient, LoginSuccessEventArgs> LoginSuccess;
        /// <summary>
        /// Invoked when login process failed.
        /// </summary>
        event SdkEventHandler<IClient, LoginFailedEventArgs> LoginFailed;
        /// <summary>
        /// Invoked when refresh of login tokens finished successfully.
        /// </summary>
        event SdkEventHandler<IClient, RefreshTokenSuccessEventArgs> RefreshTokenSuccess;
        /// <summary>
        /// Invoked when refresh of login tokens failed.
        /// </summary>
        event SdkEventHandler<IClient, RefreshTokenFailedEventArgs> RefreshTokenFailed;
        /// <summary>
        /// Invoked when one time key generated by the login server as a result of <see cref="IClient.RequestOneTimeKey(string)"/>.
        /// </summary>
        event SdkEventHandler<IClient, OneTimeKeyGeneratedEventArgs> OneTimeKeyGenerated;
        /// <summary>
        /// Invoked when there is a new incoming call to the current user.
        /// </summary>
        event SdkEventHandler<IClient, IncomingCallEventArgs> IncomingCall;
    }
}