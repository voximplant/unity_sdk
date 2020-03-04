/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

namespace Voximplant.Unity
{
    public struct Error
    {
        internal Error(int code, string message)
        {
            Code = code;
            Message = message;
        }

        /// <summary>
        /// Failure code.
        /// </summary>
        public int Code { get; }

        /// <summary>
        /// Failure reason.
        /// </summary>
        public string Message { get; }

        public override string ToString()
        {
            return $"{Message} ({Code})";
        }

        internal static Error Internal => new Error(500, "Internal Error");
    }
}