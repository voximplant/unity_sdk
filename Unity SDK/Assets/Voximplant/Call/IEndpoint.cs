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
        /// The endpoint id.
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
        /// The SIP URI of the endpoint.
        /// </summary>
        string SipUri { get; }

        /// <summary>
        /// The active video streams associated with the endpoint.
        /// </summary>
        IReadOnlyCollection<IVideoStream> VideoStreams { get; }

        /// <summary>
        /// Invoked after endpoint added video stream to the call.
        /// </summary>
        event SdkEventHandler<IEndpoint, EndpointRemoteVideoStreamAddedEventArgs> RemoteVideoStreamAdded;

        /// <summary>
        /// Invoked after endpoint removed video stream from the call. Event is not triggered on call end.
        /// </summary>
        event SdkEventHandler<IEndpoint, EndpointRemoteVideoStreamRemovedEventArgs> RemoteVideoStreamRemoved;

        /// <summary>
        /// Invoked after endpoint is removed from the call. Event is not triggered on call end.
        /// </summary>
        event SdkEventHandler<IEndpoint> Removed;

        /// <summary>
        /// Invoked when endpoint information such as display name, user name and sip uri is updated.
        /// </summary>
        event SdkEventHandler<IEndpoint> InfoUpdated;
    }
}