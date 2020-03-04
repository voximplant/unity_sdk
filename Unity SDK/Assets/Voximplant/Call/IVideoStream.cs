/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System;
using UnityEngine;
using UnityEngine.UI;
using Voximplant.Unity.Call.EventArgs;
using Voximplant.Unity.@internal;
using Object = UnityEngine.Object;

namespace Voximplant.Unity.Call
{
    /// <summary>
    /// Interface representing local and remote video streams. It may be used to add or remove video renderers.
    /// </summary>
    public interface IVideoStream : IDisposable
    {
        /// <summary>
        /// The video stream id.
        /// </summary>
        string StreamId { get; }

        /// <summary>
        /// Video frame width.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Video frame height.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Video frame rotation.
        /// </summary>
        int Rotation { get; }

        /// <summary>
        /// Add new video renderer to video stream.
        /// </summary>
        /// <param name="target">Target to apply video stream texture and shader.</param>
        void AddRenderer(RawImage target);

        /// <summary>
        /// Add new video renderer to video stream.
        /// </summary>
        /// <param name="target">Target to apply video stream texture and shader.</param>
        void AddRenderer(Material target);

        /// <summary>
        /// Remove previously added video renderer.
        /// </summary>
        /// <param name="target"><see cref="RawImage"/> or <see cref="Material"/></param>
        void RemoveRenderer(Object target);

        /// <summary>
        /// Invoked when video frame size of rotation is changed.
        /// </summary>
        event SdkEventHandler<IVideoStream, VideoStreamChangedEventArgs> VideoStreamChanged;
    }
}