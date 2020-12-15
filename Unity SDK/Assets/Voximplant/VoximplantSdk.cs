/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using JetBrains.Annotations;
using UnityEngine;
using Voximplant.Unity.Client;
using Voximplant.Unity.Hardware;
using Voximplant.Unity.@internal;
using Voximplant.Unity.@internal.Android;
using Voximplant.Unity.@internal.iOS;
using Voximplant.Unity.@internal.UnityEditor;

namespace Voximplant.Unity
{
    /// <summary>
    /// VoximplantSdk is the primary interface of Voximplant Unity SDK
    /// </summary>
    public class VoximplantSdk : MonoBehaviour
    {
        private static VoximplantSdk _instance;
        private AudioManager _audioManager;
        private ICameraManager _cameraManager;

        private @internal.Client _client;

        /// <summary>
        /// Get Client instance to connect and login to the Voximplant Cloud, make and receive calls.
        /// </summary>
        /// <returns>Client instance.</returns>
        /// <exception cref="UninitializedException"><see cref="VoximplantSdk.Initialize(ClientConfig)"/> was not called.</exception>
        public static IClient GetClient()
        {
            if (_instance == null || _instance._client == null)
                throw new UninitializedException();
            return _instance._client;
        }

        /// <summary>
        /// Get AudioManager instance to control audio hardware settings.
        /// </summary>
        /// <returns>Manager instance</returns>
        /// <exception cref="UninitializedException"><see cref="VoximplantSdk.Initialize(ClientConfig)"/> was not called.</exception>
        public static IAudioManager GetAudioManager()
        {
            if (_instance == null || _instance._audioManager == null)
                throw new UninitializedException();
            return _instance._audioManager;
        }

        /// <summary>
        /// Get CameraManager instance to control camera hardware settings.
        /// </summary>
        /// <returns>Manager instance.</returns>
        /// <exception cref="UninitializedException"><see cref="VoximplantSdk.Initialize(ClientConfig)"/> was not called.</exception>
        public static ICameraManager GetCameraManager()
        {
            if (_instance == null || _instance._cameraManager == null)
                throw new UninitializedException();
            return _instance._cameraManager;
        }

        [RuntimeInitializeOnLoadMethod]
        internal static void RuntimeInitializeOnLoadMethod()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                ThreadUtils.RunOnUpdate(() =>
                {
                    var androidUnityHelper = new AndroidJavaClass("com.voximplant.unity.GLContextHelper");
                    androidUnityHelper.CallStatic("setUnityContext");
                });
            }
        }

        /// <summary>
        /// Initialize SDK, must be called first.
        /// </summary>
        /// <param name="clientConfig">ClientConfig instance with configuration for IClient instance</param>
        /// <exception cref="UnsupportedPlatformException">Current platform is not supported</exception>
        public static void Initialize([CanBeNull] ClientConfig clientConfig = null)
        {
            if (_instance != null) return;
            if (clientConfig == null)
            {
                clientConfig = new ClientConfig();
            }

            var gameObject = new GameObject("Voximplant");
            _instance = gameObject.AddComponent<VoximplantSdk>();
            _instance.SetImplementation(Application.platform, clientConfig);
        }

        internal static T CreateVideoStream<T>() where T : Component
        {
            if (_instance == null)
                throw new UninitializedException();
            return _instance.gameObject.AddComponent<T>();
        }

        private void SetImplementation(RuntimePlatform platform, ClientConfig clientConfig)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    _client = new ClientAndroid(clientConfig);
                    _audioManager = new AudioManagerAndroid();
                    _cameraManager = new CameraManagerAndroid();
                    break;
                case RuntimePlatform.IPhonePlayer:
                    _client = new ClientIOS(clientConfig);
                    _audioManager = new AudioManagerIOS();
                    _cameraManager = new CameraManagerIOS();
                    break;
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.LinuxEditor:
                    _client = new ClientUnityEditor();
                    break;
                default:
                    throw new UnsupportedPlatformException();
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }

            else if (_instance != this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        [UsedImplicitly]
        internal void OnClientEvent(string payload)
        {
            var sdkEvent = JsonUtility.FromJson<SdkEvent>(payload);
            _client.OnEvent(sdkEvent);
        }

        [UsedImplicitly]
        internal void OnCallEvent(string payload)
        {
            var sdkEvent = JsonUtility.FromJson<CallSdkEvent>(payload);
            _client.OnCallEvent(sdkEvent);
        }

        [UsedImplicitly]
        internal void OnEndpointEvent(string payload)
        {
            var sdkEvent = JsonUtility.FromJson<EndpointSdkEvent>(payload);
            _client.OnEndpointEvent(sdkEvent);
        }

        [UsedImplicitly]
        internal void OnAudioManagerEvent(string payload)
        {
            var sdkEvent = JsonUtility.FromJson<SdkEvent>(payload);
            _audioManager.OnEvent(sdkEvent);
        }
    }
}