/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System.Collections.Generic;
using Voximplant.Unity.Call.EventArgs;
using Voximplant.Unity.@internal;

namespace Voximplant.Unity.Call
{
    /// <summary>
    /// Interface that represents the remote call party.
    /// </summary>
    public interface IEndpoint
    {
        /// <summary>
        /// The identifier for the endpoint.
        /// </summary>
        string EndpointId { get; }

        /// <summary>
        /// A user name of the endpoint.
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// A user display name of the endpoint.
        /// </summary>
        string UserDisplayName { get; }

        /// <summary>
        /// A SIP URI of the endpoint.
        /// </summary>
        string SipUri { get; }

        /// <summary>
        /// A collection of active video streams associated with the endpoint.
        /// </summary>
        IReadOnlyCollection<IVideoStream> VideoStreams { get; }

        /// <summary>
        /// Invoked after the endpoint added a video stream to the call.
        /// </summary>
        event SdkEventHandler<IEndpoint, EndpointRemoteVideoStreamAddedEventArgs> RemoteVideoStreamAdded;

        /// <summary>
        /// Invoked after the endpoint removed a video stream from the call. Event is not triggered on the call end.
        /// </summary>
        event SdkEventHandler<IEndpoint, EndpointRemoteVideoStreamRemovedEventArgs> RemoteVideoStreamRemoved;

        /// <summary>
        /// Invoked after the endpoint is removed from the call. Event is not triggered on the call end.
        /// </summary>
        event SdkEventHandler<IEndpoint> Removed;

        /// <summary>
        /// Invoked when the endpoint information such as display name, user name and sip uri is updated.
        /// </summary>
        event SdkEventHandler<IEndpoint> InfoUpdated;
    }
}