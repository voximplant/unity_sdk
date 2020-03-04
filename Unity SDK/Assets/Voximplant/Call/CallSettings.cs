/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System.Collections.Generic;

namespace Voximplant.Unity.Call
{
    /// <summary>
    /// Call settings with additional call parameters, such as preferred video codec, custom data, extra headers etc.
    /// </summary>
    public class CallSettings
    {
        public CallSettings()
        {
            VideoFlags = new VideoFlags();
            VideoCodec = VideoCodec.Auto;
        }

        /// <summary>
        /// A custom string associated with the call session.
        ///
        /// It can be passed to the cloud to be obtained from the <a href="https://voximplant.com/docs/references/voxengine/appevents#callalerting">CallAlerting</a>
        /// event or <a href="https://voximplant.com/docs/references/httpapi/managing_history#getcallhistory">Call History</a> using HTTP API.
        ///
        /// Maximum size is 200 bytes. Use the <see cref="ICall.SendMessage(string)"/> method to pass a string over the limit;
        /// in order to pass a large data use <a href="https://voximplant.com/docs/references/httpapi/managing_scenarios#startscenarios">media_session_access_url</a>
        /// on your backend.
        /// </summary>
        public string CustomData { get; set; }
        
        /// <summary>
        /// An optional set of headers to be sent to the Voximplant cloud. Names must begin with "X-" to be processed by SDK.
        /// </summary>
        public IDictionary<string, string> ExtraHeaders { get; set; }
        /// <summary>
        /// Specify video settings (send and receive) for the new call. Video is disabled by default.
        /// </summary>
        public VideoFlags VideoFlags { get; set; }
        /// <summary>
        /// A preferred video codec for a particular call that this VICallSettings are applied to <see cref="VideoCodec.Auto"/> by default.
        /// </summary>
        public VideoCodec VideoCodec { get; set; }
    }
}
