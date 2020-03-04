/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System;
using UnityEngine;
using Voximplant.Unity.Call.EventArgs;

namespace Voximplant.Unity.@internal
{
    [Serializable]
    internal class CallActionEventArgs : System.EventArgs
    {
        [SerializeField]
        private string requestGuid = default;

        public string RequestGuid => requestGuid;
    }
}
