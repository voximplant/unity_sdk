/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
﻿using System;
using System.Runtime.Serialization;

namespace Voximplant.Unity.@internal
{
    [Serializable]
    internal class UninitializedException : Exception
    {
        public UninitializedException()
        {
        }

        public UninitializedException(string message) : base(message)
        {
        }

        public UninitializedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UninitializedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
