/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System;
using UnityEngine;
using Voximplant.Unity.@internal;

namespace Voximplant.Unity.@internal
{
    [Serializable]
    internal class CallActionFailedEventArgs : CallActionEventArgs
    {
        [SerializeField]
        private int code = default;

        [SerializeField]
        private string error = default;

        public int Code => code;
        public string Error => error;
    }
}
