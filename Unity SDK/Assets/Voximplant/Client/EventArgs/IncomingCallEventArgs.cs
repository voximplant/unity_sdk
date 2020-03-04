/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using Voximplant.Unity.Call;
using Voximplant.Unity.@internal;

namespace Voximplant.Unity.Client.EventArgs
{
    /// <summary>
    /// Event arguments for <see cref="IClient.IncomingCall"/>.
    /// </summary>
    [Serializable]
    public class IncomingCallEventArgs : System.EventArgs
    {
        private IReadOnlyDictionary<string, string> _parsedHeaders;

        [SerializeField]
        internal string callId = default;

        [SerializeField]
        private IList<string> headers = default;

        [SerializeField]
        private bool incomingVideo = default;

        /// <summary>
        /// Incoming call instance.
        /// </summary>
        public ICall Call { get; internal set; }

        /// <summary>
        /// True if the caller initiated video call.
        /// </summary>
        public bool HasIncomingVideo => incomingVideo;

        /// <summary>
        /// Optional SIP headers received with the message.
        /// </summary>
        public IReadOnlyDictionary<string, string> Headers =>
            _parsedHeaders ?? (_parsedHeaders = JsonHelper.FromList(headers));
    }
}