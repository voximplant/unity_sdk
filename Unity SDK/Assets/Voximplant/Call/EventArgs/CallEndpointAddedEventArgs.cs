/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System;
using UnityEngine;
using Voximplant.Unity.@internal;

namespace Voximplant.Unity.Call.EventArgs
{
    /// <summary>
    /// Event arguments for <see cref="ICall.EndpointAdded"/>.
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