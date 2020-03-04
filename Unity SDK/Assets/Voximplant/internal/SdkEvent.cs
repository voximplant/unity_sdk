/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System;
using UnityEngine;

namespace Voximplant.Unity.@internal
{
    [Serializable]
    internal class SdkEvent
    {
        [SerializeField]
        private string @event = default;

        [SerializeField]
        private string payload = default;

        public string Event => @event;

        public T GetEventArgs<T>() where T : EventArgs
        {
            return JsonUtility.FromJson<T>(payload);
        }

        public EventArgs GetEventArgs()
        {
            return EventArgs.Empty;
        }
    }
}
