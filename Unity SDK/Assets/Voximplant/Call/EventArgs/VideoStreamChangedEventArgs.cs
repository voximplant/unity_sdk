/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

namespace Voximplant.Unity.Call.EventArgs
{
    /// <summary>
    /// Event arguments for <see cref="IVideoStream.VideoStreamChanged"/>.
    /// </summary>
    public class VideoStreamChangedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Video frame width.
        /// </summary>
        public int Width { get; internal set; }

        /// <summary>
        /// Video frame height.
        /// </summary>
        public int Height { get; internal set; }

        /// <summary>
        /// Video frame rotation.
        /// </summary>
        public int Rotation { get; internal set; }
    }
}