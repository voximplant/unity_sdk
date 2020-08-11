/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System;
using UnityEngine;
using Voximplant.Unity.@internal;

namespace Voximplant.Unity.Call.EventArgs
{
    /// <summary>
    /// Provide data for the <see cref="ICall.EndpointAdded"/> event.
    /// </summary>
    [Serializable]
    public class CallEndpointAddedEventArgs : System.EventArgs
    {
        [SerializeField]
        internal string endpointId = default;

        /// <summary>
        /// New endpoint
        /// </summary>
        public IEndpoint Endpoint { get; internal set; }
    }
}