/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Voximplant.Unity.Call.EventArgs;

namespace Voximplant.Unity.Call
{
    /// <summary>
    /// Interface that may be used for call operations like answer, reject, hang up and mid-call operations like hold, start/stop video and others.
    /// </summary>
    public interface ICall
    {
        /// <summary>
        /// The call id.
        /// </summary>
        string CallId { get; }

        /// <summary>
        /// An collection of the endpoints associated with the call.
        /// </summary>
        IReadOnlyCollection<IEndpoint> Endpoints { get; }

        /// <summary>
        /// The call duration.
        /// </summary>
        ulong Duration { get; }

        /// <summary>
        /// The local video streams associated with the call.
        /// </summary>
        IReadOnlyCollection<IVideoStream> LocalVideoStreams { get; }

        /// <summary>
        /// Start outgoing call.
        /// </summary>
        void Start();

        /// <summary>
        /// Answer incoming call.
        /// </summary>
        /// <param name="callSettings">Call settings with additional call parameters, such as preferred video codec, custom data, extra headers etc.</param>
        void Answer(CallSettings callSettings);

        /// <summary>
        /// Reject incoming call.
        /// </summary>
        /// <param name="rejectMode">Specify mode of call rejection.</param>
        /// <param name="headers">Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK.</param>
        void Reject(RejectMode rejectMode, [CanBeNull] IDictionary<string, string> headers = null);

        /// <summary>
        /// Terminates call. Call should be either established or outgoing progressing.
        /// </summary>
        /// <param name="headers">Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK.</param>
        void Hangup([CanBeNull] IDictionary<string, string> headers = null);

        /// <summary>
        /// Enables or disables audio transfer from microphone into the call.
        /// </summary>
        /// <param name="sendAudio">True if audio should be sent, false otherwise.</param>
        void SendAudio(bool sendAudio);

        /// <summary>
        /// Start or stop sending video for the call.
        /// </summary>
        /// <param name="sendVideo">True if video should be sent, false otherwise.</param>
        /// <param name="completion">Completion to handle the result of the operation.</param>
        void SendVideo(bool sendVideo, [CanBeNull] Action<Error?> completion = null);

        /// <summary>
        /// Hold or unhold the call.
        /// </summary>
        /// <param name="hold">True if the call should be put on hold, false for unhold.</param>
        /// <param name="completion">Completion to handle the result of the operation.</param>
        void Hold(bool hold, [CanBeNull] Action<Error?> completion = null);

        /// <summary>
        /// Start receive video if video receive was disabled before. Stop receiving video during the call is not supported.
        /// </summary>
        /// <param name="completion">Completion to handle the result of the operation.</param>
        void ReceiveVideo([CanBeNull] Action<Error?> completion = null);

        /// <summary>
        /// Send DTMF within the call.
        /// </summary>
        /// <param name="dtmf">DTMFs.</param>
        void SendDTMF(string dtmf);

        /// <summary>
        /// Send INFO message within the call.
        /// </summary>
        /// <param name="body">Custom string data.</param>
        /// <param name="mimeType">MIME type of info.</param>
        /// <param name="headers">Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK.</param>
        void SendInfo(string body, string mimeType, [CanBeNull] IDictionary<string, string> headers = null);

        /// <summary>
        /// Send message within the call.
        /// Implemented atop SIP INFO for communication between call endpoint and Voximplant cloud, and is separated from Voximplant messaging API.
        /// </summary>
        /// <param name="message">Message text.</param>
        void SendMessage(string message);

        /// <summary>
        /// Use <see cref="Camera"/> instance for the call to send video frames. It will be used instead of camera.
        /// </summary>
        /// <param name="camera">Unity camera.</param>
        /// <param name="fps">Video frame rate.</param>
        void SetVideoSource([NotNull] Camera camera, float fps = 30);
        
        /// <summary>
        /// Use <see cref="Camera"/> instance for the call to send video frames. It will be used instead of camera.
        /// </summary>
        /// <param name="camera">Unity camera.</param>
        /// <param name="width">Video frame width.</param>
        /// <param name="height">Video frame height</param>
        /// <param name="fps">Video frame rate.</param>
        void SetVideoSource([NotNull] Camera camera, int width, int height, float fps = 30);

        /// <summary>
        /// Invoked after call was connected.
        /// </summary>
        event SdkEventHandler<ICall, CallConnectedEventArgs> Connected;
        /// <summary>
        /// Invoked after the call was disconnected.
        /// </summary>
        event SdkEventHandler<ICall, CallDisconnectedEventArgs> Disconnected;
        /// <summary>
        /// Call ringing. You should start playing call progress tone now.
        /// </summary>
        event SdkEventHandler<ICall, CallRingingEventArgs> Ringing;
        /// <summary>
        /// Invoked if call is failed.
        /// </summary>
        event SdkEventHandler<ICall, CallFailedEventArgs> Failed;
        /// <summary>
        /// Invoked after audio is started in the call.
        /// </summary>
        event SdkEventHandler<ICall> AudioStarted;
        /// <summary>
        /// Invoked when INFO message is received.
        /// </summary>
        event SdkEventHandler<ICall, CallSIPInfoReceivedEventArgs> SIPInfoReceived;
        /// <summary>
        /// Invoked when message is received within the call.
        ///
        /// Implemented atop SIP INFO for communication between call endpoint and Voximplant cloud, and is separated from Voximplant messaging API.
        /// </summary>
        event SdkEventHandler<ICall, CallMessageReceivedEventArgs> MessageReceived;
        /// <summary>
        /// Invoked when local video is added to the call.
        /// </summary>
        event SdkEventHandler<ICall, CallLocalVideoStreamAddedEventArgs> LocalVideoStreamAdded;
        /// <summary>
        /// Invoked when local video is removed from the call.
        /// </summary>
        event SdkEventHandler<ICall, CallLocalVideoStreamRemovedEventArgs> LocalVideoStreamRemoved;
        /// <summary>
        /// Invoked if connection was not established due to a network connection problem between 2 peers.
        /// </summary>
        event SdkEventHandler<ICall> ICETimeout;
        /// <summary>
        /// Invoked when ICE connection is complete.
        /// </summary>
        event SdkEventHandler<ICall> ICECompleted;
        /// <summary>
        ///  Invoked after endpoint is added to the call.
        /// </summary>
        event SdkEventHandler<ICall, CallEndpointAddedEventArgs> EndpointAdded;
    }
}
