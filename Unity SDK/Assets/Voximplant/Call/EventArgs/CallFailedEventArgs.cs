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
    /// Provide data for the <see cref="ICall.Failed"/> event.
    /// </summary>
    [Serializable]
    public class CallFailedEventArgs : System.EventArgs
    {
        private IReadOnlyDictionary<string, string> _parsedHeaders;

        [SerializeField]
        private int code = default;

        [SerializeField]
        private string error = default;

        [SerializeField]
        private IList<string> headers = default;

        /// <summary>
        /// Details about an error occured. 
        /// </summary>
        public Error Error => new Error(code, error);

        /// <summary>
        /// Optional SIP headers received with message.
        /// </summary>
        public IReadOnlyDictionary<string, string> Headers =>
            _parsedHeaders ?? (_parsedHeaders = JsonHelper.FromList(headers));
    }
}