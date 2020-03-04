/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
﻿using System;
using System.Runtime.Serialization;

namespace Voximplant.Unity.@internal
{
    [Serializable]
    internal class UnsupportedPlatformException : Exception
    {
        public UnsupportedPlatformException()
        {
        }

        public UnsupportedPlatformException(string message) : base(message)
        {
        }

        public UnsupportedPlatformException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnsupportedPlatformException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
