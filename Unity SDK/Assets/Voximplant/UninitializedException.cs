/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System;
using System.Runtime.Serialization;

namespace Voximplant.Unity
{
    /// <summary>
    /// The exception that is thrown when the Voximplant Unity SDK is not initialized.
    /// 
    /// <see cref="VoximplantSdk.Initialize(Voximplant.Unity.Client.ClientConfig)"/> must be called before accessing
    /// <see cref="VoximplantSdk.GetClient()"/>, <see cref="VoximplantSdk.GetAudioManager()"/> and <see cref="VoximplantSdk.GetCameraManager()"/>.
    /// </summary>
    public class UninitializedException : Exception
    {
    }
}
