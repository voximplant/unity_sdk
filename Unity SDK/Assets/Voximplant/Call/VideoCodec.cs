/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

namespace Voximplant.Unity.Call
{
    public enum VideoCodec
    {
        /// <summary>
        /// Video codec for call will be chosen automatically
        /// </summary>
        Auto,

        /// <summary>
        /// Call will try to use VP8 video codec
        /// </summary>
        VP8,

        /// <summary>
        /// Call will try to use H264 video codec
        /// </summary>
        H264
    }
}