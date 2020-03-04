/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System;
using UnityEngine;
using Voximplant.Unity.Call;
using Voximplant.Unity.Call.EventArgs;

namespace Voximplant.Unity.@internal
{
    [Serializable]
    internal class CallSdkEvent : SdkEvent
    {
        [SerializeField]
        private string senderId = default;

        internal string CallId => senderId;
    }
}
