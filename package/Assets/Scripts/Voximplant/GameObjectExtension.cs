using System;
using UnityEngine;

namespace Voximplant
{
    public static class GameObjectExtensions
    {
        public static VoximplantSDK AddVoximplantSDK(this GameObject gameobject)
        {
            switch (Application.platform) {
                case RuntimePlatform.IPhonePlayer:
                    return gameobject.AddComponent<iOSSDK>();
                case RuntimePlatform.Android:
                    return gameobject.AddComponent<AndroidSDK>();
                default:
                    throw new NotSupportedException(string.Format("Voximplant SDK doesnt support: {0}", Application.platform));
            }
        }
    }
}