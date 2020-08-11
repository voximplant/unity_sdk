/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System;
using UnityEngine;
using Voximplant.Unity.@internal;

namespace Voximplant.Unity.Call.EventArgs
{
    /// <summary>
    /// Provide data for the <see cref="ICall.LocalVideoStreamAdded"/> event.
    /// </summary>
    [Serializable]
    public class CallLocalVideoStreamAddedEventArgs : System.EventArgs
    {
        [SerializeField]
        internal string streamId = default;

        /// <summary>
        /// Local video stream.
        /// </summary>
        public IVideoStream VideoStream { get; internal set; }
    }
}
