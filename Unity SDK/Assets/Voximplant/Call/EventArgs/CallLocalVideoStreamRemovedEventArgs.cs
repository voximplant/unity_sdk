/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System;
using UnityEngine;

namespace Voximplant.Unity.Call.EventArgs
{
    /// <summary>
    /// Provide data for the <see cref="ICall.LocalVideoStreamRemoved"/> event.
    /// </summary>
    [Serializable]
    public class CallLocalVideoStreamRemovedEventArgs : System.EventArgs
    {
        [SerializeField]
        internal string streamId = default;

        /// <summary>
        /// Local video stream.
        /// </summary>
        public IVideoStream VideoStream { get; internal set; }
    }
}