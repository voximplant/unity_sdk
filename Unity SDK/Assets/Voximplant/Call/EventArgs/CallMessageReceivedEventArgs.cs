/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System;
using UnityEngine;

namespace Voximplant.Unity.Call.EventArgs
{
    /// <summary>
    /// Provide data for the <see cref="ICall.MessageReceived"/> event.
    /// </summary>
    [Serializable]
    public class CallMessageReceivedEventArgs : System.EventArgs
    {
        [SerializeField]
        private string text = default;

        /// <summary>
        /// Content of the message.
        /// </summary>
        public string Text => text;
    }
}
