using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Voximplant
{
    public abstract class VoximplantSDK : MonoBehaviour
    {
        public Action<String> LogMethod;

        protected void AddLog(String pMsg)
        {
            if (LogMethod != null)
                LogMethod(pMsg);
        }

        public delegate void LoginSuccessfulEvent(string displayName);

        public delegate void LoginFailedEvent(LoginFailureReason reason);

        public delegate void OneTimeKeyGeneratedEvent(string key);

        public delegate void ConnectionSuccessfulEvent();

        public delegate void ConnectionClosedEvent();

        public delegate void ConnectionFailedWithErrorEvent(string reason);

        public delegate void CallConnectedEvent(string callId, Dictionary<string, string> headers);

        public delegate void CallDisconnectedEvent(string callId, Dictionary<string, string> headers);

        public delegate void CallRingingEvent(string callId, Dictionary<string, string> headers);

        public delegate void CallFailedEvent(string callId, int code, string reason,
            Dictionary<string, string> headers);

        public delegate void CallAudioStartedEvent(string callId);

        public delegate void IncomingCallEvent(String callId, String from, String displayName, Boolean videoCall,
            Dictionary<String, String> headers);

        public delegate void SIPInfoReceivedInCallEvent(string callId, string type, string content,
            Dictionary<string, string> headers);

        public delegate void MessageReceivedInCallEvent(string callId, string text, Dictionary<string, string> headers);

        public delegate void NetStatsReceivedEvent(string callId, int packetLoss);

        public delegate void StartCallEvent(string callId);

        /**
        Called when packet loss data is received from the Voximplant cloud
        @event NetStatsReceived
        @param {string} callid Call identifier
        @param {int} loss Packet loss from 0 (no loss) to 100 (all lost)
        */
        public event NetStatsReceivedEvent NetStatsReceived;

        /**
        Called if Voximplant-specified 'message' is recived. It's a specialized type of a more general SIP 'info' message
        @event MessageReceivedInCall
        @param {string} callid Call identifier
        @param {string} text Message text
        @param {Dictionary} (Only for iOS) headers Optional SIP headers set by a info sender
        */
        public event MessageReceivedInCallEvent MessageReceivedInCall;

        /**
        Called if SIP 'info' message is received. That message can be sent from a Voximplant cloud scenario or forwarded from a caller
        @event SIPInfoReceivedInCall
        @param {string} callid Call identifier
        @param {string} type Data MIME type string
        @param {string} content Data string
        @param {Dictionary} headers Optional SIP headers set by a info sender
        */
        public event SIPInfoReceivedInCallEvent SIPInfoReceivedInCall;

        /**
        Called when Voximplant directs a new call to a user logged in from this SDK instance. SDK can handle multiple incoming and/or outgoing calls at once and target specified call via the 'callid' string returned by 'call()' method and received by this event
        @event IncomingCall
        @param {string} callid Call identifier
        @param {string} from Caller SIP URI
        @param {string} name Caller display name
        @param {bool} isVideo 'true' if incoming call is a video call. Video can be enabled or disabled during a call
        @param {Dictionary} headers Optional SIP headers that was set by a caller
        */
        public event IncomingCallEvent IncomingCall;

        /**
        Called when Voximplant cloud connects audio source to the call. If client previously played a progress tone, it should be stopped
        @event CallAudioStarted
        @param {string} callid Call identifier, previously returned by the call() function
        */
        public event CallAudioStartedEvent CallAudioStarted;

        /**
        Called when Voximplant cloud rejects a call
        @event CallFailed
        @param {string} callid Call identifier, previously returned by the call() function
        @param {int} code Status code
        @param {string} reason Text description while call failed
        @param {Dictionary} headers Optional SIP headers that was sent by Voximplant while call was rejected
        */
        public event CallFailedEvent CallFailed;

        /**
        Called when Voximplant cloud sends RINGING SIP notificatoin via 'call.ring()' method. As response to that event client can play some "ringing" sounds or inform user about "call in progress" some other way
        @event CallRinging
        @param {string} callid Call identifier, previously returned by the call() function
        @param {Dictionary} headers Optional SIP headers that was sent by Voximplant as an argument to the 'call.ring()' method call
        */
        public event CallRingingEvent CallRinging;

        /**
        Called after call is gracefully disconnected from the Voximplant cloud
        @event CallDisconnected
        @param {string} callid Call identifier, previously returned by the call() function
        @param {Dictionary} headers Optional SIP headers that was sent by Voximplant while disconnecting the call
        */
        public event CallDisconnectedEvent CallDisconnected;

        /**
        Called after call() method successfully established a call with the Voximplant cloud
        @event CallConnected
        @param {string} callid Connected call identifier. It's same identifier returned by the call() function and it can be used in other function to specify one of multiple calls
        @param {Dictionary} headers Optional SIP headers that was sent by Voximplant while accepting the call
        */
        public event CallConnectedEvent CallConnected;

        /**
        Called if connect() failed to establish a Voximplant cloud connection
        @event ConnectionFailedWithError
        @param {string} error Error message
        */
        public event ConnectionFailedWithErrorEvent ConnectionFailedWithError;

        /**
        Called after connection to Voximplant cloud is closed for any reason
        @event ConnectionClosed
        */
        public event ConnectionClosedEvent ConnectionClosed;

        /**
        Called after connect() successfully connects to Voximplant cloud
        @event ConnectionSuccessful
        */
        public event ConnectionSuccessfulEvent ConnectionSuccessful;

        /**
        Called after key requested with 'requestOneTimeKey' is requested and returned from the Voximplant cloud
        @event OneTimeKeyGenerated
        @param {string} key Key string that should be used in a hash for the 'loginUsingOneTimeKey'
        */
        public event OneTimeKeyGeneratedEvent OneTimeKeyGenerated;

        /**
        Called if login() call results in the failed login
        @event LoginFailed
        @param {string} error Failure reason
        */
        public event LoginFailedEvent LoginFailed;

        /**
        Called if login() call results in the successful login
        @event LoginSuccessful
        @param {string} username Display name of logged in user
        */
        public event LoginSuccessfulEvent LoginSuccessful;

        /**
        Called when you start call
        @event StartCall
        @param {string} callid Call identifier, previously returned by the call() function
        */
        public event StartCallEvent StartCall;

        protected virtual void OnLoginSuccessful(string displayName)
        {
            if (LoginSuccessful != null) {
                LoginSuccessful(displayName);
            }
        }

        protected virtual void OnLoginFailed(LoginFailureReason reason)
        {
            if (LoginFailed != null) {
                LoginFailed(reason);
            }
        }

        protected virtual void OnOneTimeKeyGenerated(string key)
        {
            if (OneTimeKeyGenerated != null) {
                OneTimeKeyGenerated(key);
            }
        }

        protected virtual void OnConnectionSuccessful()
        {
            if (ConnectionSuccessful != null) {
                ConnectionSuccessful();
            }
        }

        protected virtual void OnConnectionClosed()
        {
            if (ConnectionClosed != null) {
                ConnectionClosed();
            }
        }

        protected virtual void OnConnectionFailedWithError(string reason)
        {
            if (ConnectionFailedWithError != null) {
                ConnectionFailedWithError(reason);
            }
        }

        protected virtual void OnCallConnected(string callId, Dictionary<string, string> headers)
        {
            if (CallConnected != null) {
                CallConnected(callId, headers);
            }
        }

        protected virtual void OnCallDisconnected(string callId, Dictionary<string, string> headers)
        {
            if (CallDisconnected != null) {
                CallDisconnected(callId, headers);
            }
        }

        protected virtual void OnCallRinging(string callId, Dictionary<string, string> headers)
        {
            if (CallRinging != null) {
                CallRinging(callId, headers);
            }
        }

        protected virtual void OnCallFailed(string callId, int code, string reason,
            Dictionary<string, string> headers)
        {
            if (CallFailed != null) {
                CallFailed(callId, code, reason, headers);
            }
        }

        protected virtual void OnCallAudioStarted(string callId)
        {
            if (CallAudioStarted != null) {
                CallAudioStarted(callId);
            }
        }

        protected virtual void OnIncomingCall(String callId, String from, String displayName, Boolean videoCall,
            Dictionary<String, String> headers)
        {
            if (IncomingCall != null) {
                IncomingCall(callId, from, displayName, videoCall, headers);
            }
        }

        protected virtual void OnSIPInfoReceivedInCall(string callId, string type, string content,
            Dictionary<string, string> headers)
        {
            if (SIPInfoReceivedInCall != null) {
                SIPInfoReceivedInCall(callId, type, content, headers);
            }
        }

        protected virtual void OnMessageReceivedInCall(string callId, string text,
            Dictionary<string, string> headers)
        {
            if (MessageReceivedInCall != null) {
                MessageReceivedInCall(callId, text, headers);
            }
        }

        protected virtual void OnNetStatsReceived(string callId, int packetLoss)
        {
            if (NetStatsReceived != null) {
                NetStatsReceived(callId, packetLoss);
            }
        }

        protected virtual void OnStartCall(string callId)
        {
            if (StartCall != null) {
                StartCall(callId);
            }
        }

        /**
        Initialize the SDK
        @method init
        @param {String} pObjectNameSDK Name of Unity GameObject to receive SDK events. Script connected to that object calls this 'init' method
        */
        public abstract void init(Action<bool> initCallback);

        /**
        Close connection to the Voximplant cloud that was previously established via 'connect' call
        @method closeConnection
        */
        public abstract void closeConnection();

        /**
        Initiate connection to the Voximplant cloud. After successful connection a "login" method should be called
        @method connect
        */
        public abstract void connect();

        /**
        Login into Voximplant cloud. Should be called after "connect" in the "OnConnectionSuccessful" handler
        @method login
        @param {LoginClassParam} pLogin Username and password. Username is a fully-qualified string that includes Voximplant user, application and account names. The format is: "username@appname.accname.voximplant.com"
        */
        public abstract void login(LoginClassParam pLogin);

        /**
        Start a new outgoing call
        @method call
        @param {CallClassParam} pCall Call options: number to call, video call flag and custom data to send alongside the call. For SIP compatibility reasons number should be a non-empty string even if the number itself is not used by a Voximplant cloud scenario. "OnCallConnected" will be called on call success, or "OnCallFailed" will be called if Voximplant cloud rejects a call or network error occur
        */
        public abstract void call(CallClassParam pCall);

        /**
        Answer incoming call. Should be called from the "OnIncomingCall" handler
        @method answer
        @param {string} cameraPosition Call identifier
        @param {Dictionary} (Only for iOS) headers Optional SIP headers set by a info sender
        */
        public abstract void answer(string pCallId, Dictionary<string, string> pHeader = null);

        /**
        Decline an incoming call. Should be called from the "OnIncomingCall" handler
        @method declineCall
        @param {string} cameraPosition Call identifier
        @param {Dictionary} (Only for iOS) headers Optional SIP headers set by a info sender
        */
        public abstract void declineCall(string pCallId, Dictionary<string, string> pHeader = null);

        /**
        Hang up the call in progress. Should be called from the "OnIncomingCall" handler
        @method hangup
        @param {string} cameraPosition Call identifier
        @param {Dictionary} (Only for iOS) headers Optional SIP headers set by a info sender
        */
        public abstract void hangup(string pCallId, Dictionary<string, string> pHeader = null);

        /**
        Mute or unmute microphone. This is reset after audio interruption
        @method setMute
        @param {Boolean} pState 'true' to set mute status, 'false' to remove it
        */
        public abstract void setMute(Boolean pState);

        /**
        Enable or disable video stream transfer during the call
        @method sendVideo
        @param {Boolean} pState 'true' to enable video stream, 'false' to disable it
        */
        public abstract void sendVideo(Boolean pState);

        /**
        Select a cameraPosition to use for the video call
        @method setCamera
        @param {Camera} cameraPosition A cameraPosition to use: 'Camera.CAMERA_FACING_FRONT', 'Camera.CAMERA_FACING_BACK'
        */
        public abstract void setCamera(Camera cameraPosition);

        // Internal
        public abstract void disableTls();

        /**
        Disconnect the specified call
        @method disconnectCall
        @param {string} cameraPosition Call identifier
        @param {Dictionary} (Only for iOS) headers Optional SIP headers set by a info sender
        */
        public abstract void disconnectCall(string p, Dictionary<string, string> pHeader = null);

        /**
        If called before any other SDK methods, enables debug logging into target platform default debug log facility
        @method enableDebugLogging
        */
        public abstract void enableDebugLogging();

        /**
        Login using a hash created from one-time key requested via 'requestOneTimeKey' and password. The has can be calculated on your backend so Voximplant password is never used in the application. Hash is calculated as 'MD5(one-time-key + "|" + MD5(vox-user + ":voximplant.com:" + vox-pass))'
        @method loginUsingOneTimeKey
        @param {LoginOneTimeKeyClassParam} pLogin Fully-qualified user name and hash to authenticate. Note that hash is created not from a fully-qualified user name, but from a bare user name, without the "@app.acc.voximplant.com" part
        */
        public abstract void loginUsingOneTimeKey(LoginOneTimeKeyClassParam pLogin);

        /**
        Request a one-time key that can be used on your backend to create a login hash for the 'loginUsingOneTimeKey'. Key is returned via 'OnOneTimeKeyGenerated' event
        @method requestOneTimeKey
        @param {string} pName Fully-qualified user name to get a one-time key for. Format is 'user@app.acc.voximplant.com'
        */
        public abstract void requestOneTimeKey(string pName);

        /**
        Send DTMF signal to the specified call
        @method sendDTMF
        @param {DTFMClassParam} pParam Call identifier and DTMF digit number (0-9, 10 for '*', 11 for '#')
        */
        public abstract void sendDTMF(DTFMClassParam pParam);

        /**
        Send arbitrary data to Voximplant cloud. Data can be received in VoxEngine scenario by subscribing to the 'CallEvents.InfoReceived' event. Optional SIP headers can be automatically passed to second call via VoxEngine 'easyProcess()' method
        @method sendInfo
        @param {InfoClassParam} pParam Call identifier, data MIME type string, data string and optional SIP headers list as an array of 'PairKeyValue' objects with 'key' and 'value' properties
        */
        public abstract void sendInfo(InfoClassParam pParam);

        /**
        Simplified version of 'sendInfo' that uses predefined MIME type to send a text message. Message can be received in VoxEngine scenario by subscribing to the 'CallEvents.MessageReceived' event. Optional SIP headers can be automatically passed to second call via VoxEngine 'easyProcess()' method
        @method sendMessage
        @param {SendMessageClassParam} pParam Call identifier, message string and optional SIP headers list as an array of 'PairKeyValue' objects with 'key' and 'value' properties
        */
        public abstract void sendMessage(SendMessageClassParam pParam);

        /**
        Set local cameraPosition resolution. Increasing resolution increases video quality, but also uses more cpu and bandwidth
        @method setCameraResolution
        @param {CameraResolutionClassParam} pParam Camera resolutino as width and height, in pixels
        */
        public abstract void setCameraResolution(CameraResolutionClassParam pParam);

        /**
        Enable or disable loud speaker, if available
        @method setUseLoudspeaker
        @param {bool} pUseLoudSpeaker 'true' to enable loud speaker, 'false' to disable it
        */
        public abstract void setUseLoudspeaker(bool pUseLoudSpeaker);

        void Start()
        {
            UnityEngine.Camera.onPreRender += cam => {
                GL.IssuePluginEvent(GetRenderEventFunc(), 41);
            };
            UnityEngine.Camera.onPostRender += cam => {
                GL.IssuePluginEvent(GetRenderEventFunc(), 42);
            };
        }

#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
        [DllImport ("__Internal")]
#else
        [DllImport("VoximplantAndroidRendererPlugin")]
#endif
        private static extern IntPtr GetRenderEventFunc();

        public enum VideoStream
        {
            Remote = 0,
            Local
        }

        protected Dictionary<VideoStream, Action<Texture2D>> videoStreamCallbacks = new Dictionary<VideoStream, Action<Texture2D>>();

        /**
        Execute specified callback, passing it every frame received from a local video preview or a remote video stream. Frames are Unity textures that can be assigned to same object to represent video.
        @method beginUpdatingTextureWithVideoStream
        @param {VideoStream} stream "VoximplantSDK.VideoStream.Local" for a local camera preview video or "VoximplantSDK.VideoStream.Remote" for a remote video.
        @param {Action<Texture2D>> callback This method will be called with each video frame. Note that given texture should be assigned to same obect, since previous one is destroyed by a native code.
        */
        public void beginUpdatingTextureWithVideoStream(VideoStream stream, Action<Texture2D> callback)
        {
            Action<Texture2D> oldCallback;
            if (videoStreamCallbacks.TryGetValue(stream, out oldCallback)) {
                endUpdatingTexture(stream);
            }

            startVideoStreamRendering(stream);
            videoStreamCallbacks[stream] = callback;
        }

        protected abstract void startVideoStreamRendering(VideoStream stream);

        public virtual void endUpdatingTexture(VideoStream stream)
        {
            if (videoStreamCallbacks.ContainsKey(stream)) {
                videoStreamCallbacks.Remove(stream);
            }
        }

        protected static LoginFailureReason LoginFailureReasonFromString(String reason)
        {
            switch (reason) {
                case "INVALID_PASSWORD":
                    return LoginFailureReason.INVALID_PASSWORD;
                case "INVALID_USERNAME":
                    return LoginFailureReason.INVALID_USERNAME;
                case "ACCOUNT_FROZEN":
                    return LoginFailureReason.ACCOUNT_FROZEN;
                default:
                    return LoginFailureReason.INTERNAL_ERROR;
            }
        }
    }
}
