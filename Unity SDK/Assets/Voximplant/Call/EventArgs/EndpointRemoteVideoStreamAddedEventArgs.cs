/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System;
using UnityEngine;
using Voximplant.Unity.@internal;

namespace Voximplant.Unity.Call.EventArgs
{
    /// <summary>
    /// Event arguments for <see cref="IEndpoint.RemoteVideoStreamAdded"/>.
    /// </summary>
    [Serializable]
    public class EndpointRemoteVideoStreamAddedEventArgs : System.EventArgs
    {
        [SerializeField]
        internal string streamId = default;

        /// <summary>
        /// Remote video stream.
        /// </summary>
        public IVideoStream VideoStream { get; internal set; }
    }
}