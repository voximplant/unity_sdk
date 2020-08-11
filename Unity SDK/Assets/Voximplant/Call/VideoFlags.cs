/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

namespace Voximplant.Unity.Call
{
    /// <summary>
    /// Specifies video direction for a call.
    /// </summary>
    public class VideoFlags
    {
        public VideoFlags()
        {
            SendVideo = false;
            ReceiveVideo = false;
        }

        /// <summary>
        /// Specify if video receiving is enabled for a call.
        /// </summary>
        public bool ReceiveVideo { get; set; }

        /// <summary>
        /// Specify if video sending is enabled for a call.
        /// </summary>
        public bool SendVideo { get; set; }
    }
}