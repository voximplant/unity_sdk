/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using Voximplant.Unity.@internal;

namespace Voximplant.Unity.Call.EventArgs
{
    /// <summary>
    /// Provide data for the <see cref="ICall.Ringing"/> event.
    /// </summary>
    [Serializable]
    public class CallRingingEventArgs : System.EventArgs
    {
        private IReadOnlyDictionary<string, string> _parsedHeaders;

        [SerializeField]
        private IList<string> headers = default;

        /// <summary>
        /// Optional SIP headers received with message.
        /// </summary>
        public IReadOnlyDictionary<string, string> Headers =>
            _parsedHeaders ?? (_parsedHeaders = JsonHelper.FromList(headers));
    }
}