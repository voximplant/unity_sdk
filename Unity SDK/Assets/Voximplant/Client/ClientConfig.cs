/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

using System;
using UnityEngine;
using Voximplant.Unity.Call;

namespace Voximplant.Unity.Client
{
    /// <summary>
    /// Request audio focus modes.
    ///
    /// Android only.
    /// </summary>
    public enum RequestAudioFocusMode
    {
        /// <summary>
        /// Request of audio focus is performed when a call is started.
        /// </summary>
        OnCallStart,

        /// <summary>
        /// Request of audio focus is performed when a call is established.
        /// </summary>
        OnCallConnected
    }

    /// <summary>
    /// Logging level.
    ///
    /// iOS only.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Mutes all log messages.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// Log verbosity level to include only error messages.
        /// </summary>
        Error = (1 << 20),

        /// <summary>
        /// Log verbosity level to include error and warnings messages.
        /// </summary>
        Warning = Error | (1 << 21),

        /// <summary>
        /// Log verbosity level to include error, warnings and info messages.
        /// </summary>
        Info = Warning | (1 << 22),

        /// <summary>
        /// Log verbosity level to include error, warnings, info and debug messages.
        /// </summary>
        Debug = Info | (1 << 23),

        /// <summary>
        /// Log verbosity level to include error, warnings, info, debug and verbose messages.
        /// </summary>
        Verbose = Debug | (1 << 24)
    }

    /// <summary>
    /// Configuration for <see cref="IClient"/> instance.
    /// </summary>
    [Serializable]
    public class ClientConfig
    {
        [SerializeField]
        private bool enableDebugLogging = false;

        [SerializeField]
        private bool enableLogcatLogging = true;

        [SerializeField]
        private RequestAudioFocusMode audioFocusMode = RequestAudioFocusMode.OnCallStart;

        [SerializeField]
        private LogLevel logLevel = LogLevel.Info;

        /// <summary>
        /// Enables debug logging on Android. False by default.
        /// </summary>
        public bool EnableDebugLogging
        {
            get => enableDebugLogging;
            set => enableDebugLogging = value;
        }

        /// <summary>
        /// Enables log output to logcat on Android. True by default.
        /// </summary>
        public bool EnableLogcatLogging
        {
            get => enableLogcatLogging;
            set => enableLogcatLogging = value;
        }

        /// <summary>
        /// Specifies when the audio focus request is performed: when a call is started
        /// or established.
        ///
        /// <see cref="RequestAudioFocusMode.OnCallStart"/> by default.
        ///
        /// If the application plays some audio, it may result in audio interruptions.
        /// To avoid this behavior, this option should be set to
        /// <see cref="RequestAudioFocusMode.OnCallConnected"/> and application's audio should
        /// be stopped/paused on <see cref="ICall.AudioStarted"/> callback.
        /// </summary>
        public RequestAudioFocusMode AudioFocusMode
        {
            get => audioFocusMode;
            set => audioFocusMode = value;
        }

        /// <summary>
        /// Specifies log level on iOS.
        /// </summary>
        public LogLevel LogLevel
        {
            get => logLevel;
            set => logLevel = value;
        }
    }
}