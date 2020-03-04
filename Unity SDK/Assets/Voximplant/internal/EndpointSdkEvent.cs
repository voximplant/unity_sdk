/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System;
using UnityEngine;

namespace Voximplant.Unity.@internal
{
    [Serializable]
    internal class EndpointSdkEvent : SdkEvent
    {
        [SerializeField]
        internal string senderId = default;

        internal string EndpointId => senderId;
    }
}
