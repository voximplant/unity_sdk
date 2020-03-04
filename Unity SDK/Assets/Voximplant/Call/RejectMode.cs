/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

namespace Voximplant.Unity.Call
{
    /// <summary>
    /// Enum of incoming call reject modes.
    /// </summary>
    public enum RejectMode
    {
        /// <summary>
        /// Indicates that user can't answer the call right now, and VoxEngine will terminate the call and any pending calls to other devices of current user.
        /// </summary>
        Decline,

        /// <summary>
        /// Indicates that the user is not available only at a particular device.
        /// </summary>
        Busy
    }
}