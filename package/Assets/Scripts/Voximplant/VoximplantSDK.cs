using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using Voximplant.Threading;

namespace Voximplant
{
    public abstract class VoximplantSDK : MonoBehaviour
    {
#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
        [DllImport ("__Internal")]
#else
        [DllImport("VoximplantAndroidRendererPlugin")]
#endif
        private static extern void InitializeVoximplant();

        private InvokePump pump;

        void Awake()
        {
            InitializeVoximplant();

            pump = gameObject.AddComponent<InvokePumpBehavior>().invokePump;
        }

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

        public delegate void MessageReceivedInCallEvent(string callId, string text);

        public delegate void NetStatsReceivedEvent(string callId, int packetLoss);

        public delegate void StartCallEvent(string callId);

        /**
        Called when packet loss data is received from the Voximplant cloud
        @event NetStatsReceived
        @param {string} callId Call identifier
        @param {int} loss Packet loss from 0 (no loss) to 100 (all lost)
        */
        public event NetStatsReceivedEvent NetStatsReceived;

        /**
        Called if Voximplant-specified 'message' is recived. It's a specialized type of a more general SIP 'info' message
        @event MessageReceivedInCall
        @param {string} callId Call identifier
        @param {string} text Message text
        */
        public event MessageReceivedInCallEvent MessageReceivedInCall;

        /**
        Called if SIP 'info' message is received. That message can be sent from a Voximplant cloud scenario or forwarded from a caller
        @event SIPInfoReceivedInCall
        @param {string} callId Call identifier
        @param {string} type Data MIME type string
        @param {string} content Data string
        @param {Dictionary} headers Optional SIP headers set by a info sender
        */
        public event SIPInfoReceivedInCallEvent SIPInfoReceivedInCall;

        /**
        Called when Voximplant directs a new call to a user logged in from this SDK instance. SDK can handle multiple incoming and/or outgoing calls at once and target specified createCall via the 'callId' string returned by 'createCall()' method and received by this event
        @event IncomingCall
        @param {string} callId Call identifier
        @param {string} from Caller SIP URI
        @param {string} name Caller display name
        @param {bool} isVideo 'true' if incoming call is a video createCall. Video can be enabled or disabled during a createCall
        @param {Dictionary} headers Optional SIP headers that was set by a caller
        */
        public event IncomingCallEvent IncomingCall;

        /**
        Called when Voximplant cloud connects audio source to the call. If client previously played a progress tone, it should be stopped
        @event CallAudioStarted
        @param {string} callId Call identifier, previously returned by the call() function
        */
        public event CallAudioStartedEvent CallAudioStarted;

        /**
        Called when Voximplant cloud rejects a call
        @event CallFailed
        @param {string} callId Call identifier, previously returned by the call() function
        @param {int} code Status code
        @param {string} reason Text description while call failed
        @param {Dictionary} headers Optional SIP headers that was sent by Voximplant while call was rejected
        */
        public event CallFailedEvent CallFailed;

        /**
        Called when Voximplant cloud sends RINGING SIP notificatoin via 'call.ring()' method. As response to that event client can play some "ringing" sounds or inform user about "createCall in progress" some other way
        @event CallRinging
        @param {string} callId Call identifier, previously returned by the call() function
        @param {Dictionary} headers Optional SIP headers that was sent by Voximplant as an argument to the 'call.ring()' method createCall
        */
        public event CallRingingEvent CallRinging;

        /**
        Called after call is gracefully disconnected from the Voximplant cloud
        @event CallDisconnected
        @param {string} callId Call identifier, previously returned by the call() function
        @param {Dictionary} headers Optional SIP headers that was sent by Voximplant while disconnecting the call
        */
        public event CallDisconnectedEvent CallDisconnected;

        /**
        Called after call() method successfully established a createCall with the Voximplant cloud
        @event CallConnected
        @param {string} callId Connected call identifier. It's same identifier returned by the createCall() function and it can be used in other function to specify one of multiple calls
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
        @param {string} callId Call identifier, previously returned by the call() function
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
            endSendingCameraVideoStreamForCall(callId);
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

        protected virtual void OnMessageReceivedInCall(
          string callId,
          string text,
          //! Obsolete, will be removed in new SDK
          Dictionary<string, string> headers)
        {
            if (MessageReceivedInCall != null) {
                MessageReceivedInCall(callId, text);
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
        @param {Action<bool>} callback This callback will be called after SDK is initialized. It receives 'true' boolean argument if initialization is successful.
        */
        public abstract void init(Action<bool> callback, bool preferH264);

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
        @param {string} username Username is a fully-qualified string that includes Voximplant user, application and account names. The format is: "username@appname.accname.voximplant.com"
        @param {string} password Plain text user password. Use loginUsingOneTimeKey for the better security
        */
        public abstract void login(string username, string password);

        /**
        Creates new call
        @method createCall
        @param {string} number Number to call. For SIP compatibility reasons number should be a non-empty string even if the number itself is not used by a Voximplant cloud scenario
        @param {bool} videoCall If 'true', video will will be sent and received for the call. Video can also be enabled or disabled dynamically during the createCall
        @param {string} customData Custom data to send alongside call into Voximplant JavaScript cloud scenario
        @return callId of created call
        */
        public abstract string createCall(string number, bool videoCall, string customData);

        /**
        Start outgoing call
        @method startCall
        @param {string} callId Call identifier
        @param {Dictionary<string, string>} headers Optional SIP headers to send alongside the call
        */
        public abstract void startCall(string callId, Dictionary<string, string> headers = null);

        /**
        Answer incoming call. Should be called from the "OnIncomingCall" handler
        @method answer
        @param {string} callId Call identifier
        @param {Dictionary} headers Optional SIP headers set by a info sender
        */
        public abstract void answer(string callId, Dictionary<string, string> headers = null);

        /**
        Decline an incoming call. Should be called from the "OnIncomingCall" handler
        @method declineCall
        @param {string} callId Call identifier
        @param {Dictionary} headers Optional SIP headers set by a info sender
        */
        public abstract void declineCall(string callId, Dictionary<string, string> headers = null);

        /**
        Deprecated. Use disconnectCall instead.

        Hang up the call in progress.
        @method hangup
        @param {string} callId Call identifier
        @param {Dictionary} headers Optional SIP headers set by a info sender
        */
        public abstract void hangup(string callId, Dictionary<string, string> headers = null);

        /**
        Mute or unmute microphone. This is reset after audio interruption
        @method setMute
        @param {Boolean} isMute 'true' to set mute status, 'false' to remove it
        */
        public abstract void setMute(Boolean isMute);

        /**
        Enable or disable video stream transfer during the call
        @method sendVideo
        @param {Boolean} isVideo 'true' to enable video stream, 'false' to disable it
        */
        public abstract void sendVideo(Boolean isVideo);

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
        @param {string} callId Call identifier
        @param {Dictionary} headers Optional SIP headers set by a info sender
        */
        public abstract void disconnectCall(string callId, Dictionary<string, string> headers = null);

        /**
        If called before any other SDK methods, enables debug logging into target platform default debug log facility
        @method enableDebugLogging
        */
        public abstract void enableDebugLogging();

        /**
        Login using a hash created from one-time key requested via 'requestOneTimeKey' and password. The has can be calculated on your backend so Voximplant password is never used in the application. Hash is calculated as 'MD5(one-time-key + "|" + MD5(vox-user + ":voximplant.com:" + vox-pass))'
        @method loginUsingOneTimeKey
        @param {string} username Username is a fully-qualified string that includes Voximplant user, application and account names. The format is: "username@appname.accname.voximplant.com"
        @param {string} hash Hash to authenticate. Note that hash is created not from a fully-qualified username, but from a bare user login, without the "@app.acc.voximplant.com" part
        */
        public abstract void loginUsingOneTimeKey(string username, string hash);

        /**
        Request a one-time key that can be used on your backend to create a login hash for the 'loginUsingOneTimeKey'. Key is returned via 'OnOneTimeKeyGenerated' event
        @method requestOneTimeKey
        @param {string} pName Fully-qualified user name to get a one-time key for. Format is 'user@app.acc.voximplant.com'
        */
        public abstract void requestOneTimeKey(string pName);

        /**
        Send DTMF signal to the specified call
        @method sendDTMF
        @param {string} callId Call identifier
        @param {string} digit DTMF digit number (0-9, 10 for '*', 11 for '#')
        */
        public abstract void sendDTMF(string callId, string digit);

        /**
        Send arbitrary data to Voximplant cloud. Data can be received in VoxEngine scenario by subscribing to the 'CallEvents.InfoReceived' event. Optional SIP headers can be automatically passed to second call via VoxEngine 'easyProcess()' method
        @method sendInfo
        @param {string} callid Call identifier
        @param {string} mimeType Data MIME type string
        @param {string} content Data string
        @param {Dictionary<string, string>} headers Optional SIP headers list as an array of 'PairKeyValue' objects with 'key' and 'value' properties
        */
        public abstract void sendInfo(string callId, string mimeType, string content, Dictionary<string, string> headers = null);

        /**
        Simplified version of 'sendInfo' that uses predefined MIME type to send a text message. Message can be received in VoxEngine scenario by subscribing to the 'CallEvents.MessageReceived' event. Optional SIP headers can be automatically passed to second call via VoxEngine 'easyProcess()' method
        @method sendMessage
        @param {string} callId Call identifier
        @param {string} message Message string
        @param {<Dictionary<string, string>} headers Optional SIP headers list as an array of 'PairKeyValue' objects with 'key' and 'value' properties
        */
        public abstract void sendMessage(string callId, string message, Dictionary<string, string> headers = null);

        /**
        Set local cameraPosition resolution. Increasing resolution increases video quality, but also uses more cpu and bandwidth
        @method setCameraResolution
        @param {int} width Width in pixels
        @param {int} height Height in pixels
        */
        public abstract void setCameraResolution(int width, int height);

        /**
        Enable or disable loud speaker, if available
        @method setUseLoudspeaker
        @param {bool} isLoudSpeaker 'true' to enable loud speaker, 'false' to disable it
        */
        public abstract void setUseLoudspeaker(bool isLoudSpeaker);

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

        protected struct CallStreamDescriptor
        {
            public readonly string callId;
            public readonly VideoStream stream;

            public bool Equals(CallStreamDescriptor other)
            {
                return string.Equals(callId, other.callId) && stream == other.stream;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is CallStreamDescriptor && Equals((CallStreamDescriptor) obj);
            }

            public override int GetHashCode()
            {
                unchecked {
                    return ((callId != null ? callId.GetHashCode() : 0) * 397) ^ (int) stream;
                }
            }

            public CallStreamDescriptor(string callId, VideoStream stream)
            {
                this.callId = callId;
                this.stream = stream;
            }
        }

        protected Dictionary<CallStreamDescriptor, Action<Texture2D>> videoStreamCallbacks = new Dictionary<CallStreamDescriptor, Action<Texture2D>>();

        /**
        Execute specified callback, passing it every texture created for receiving local video preview or a remote video stream.
        @method beginUpdatingTextureWithVideoStream
        @param {string} callId Call identifier
        @param {VideoStream} stream "VoximplantSDK.VideoStream.Local" for a local camera preview video or "VoximplantSDK.VideoStream.Remote" for a remote video.
        @param {Action<Texture2D>} callback This method will be called with each video frame. Note that given texture should be assigned to same obect, since previous one is destroyed by a native code.
        */
        public void beginUpdatingTextureWithVideoStream(string callId, VideoStream stream, Action<Texture2D> callback)
        {
            Action<Texture2D> oldCallback;
            var descriptor = new CallStreamDescriptor(callId, stream);
            if (videoStreamCallbacks.TryGetValue(descriptor, out oldCallback)) {
                endUpdatingTexture(callId, stream);
            }

            startVideoStreamRendering(callId, stream);
            videoStreamCallbacks[descriptor] = callback;
        }

        protected abstract void startVideoStreamRendering(string callId, VideoStream stream);

        private struct NativeTextureDescriptor
        {
            public long textureId;
            public long context;
            public Texture2D texture;
        }

        private readonly Dictionary<CallStreamDescriptor, NativeTextureDescriptor> nativeTextures =
            new Dictionary<CallStreamDescriptor, NativeTextureDescriptor>();

        protected void fonNewNativeTexture(String p)
        {
            Debug.Log(string.Format("new native texture {0}", p));
            
            var paramList = Utils.GetParamList(p);
            var callId = paramList[0].Value;
            var textureId = paramList[1].AsLong;
            var oglContext = paramList[2].AsLong;
            var width = paramList[3].AsInt;
            int height = paramList[4].AsInt;
            int stream = paramList[5].AsInt;

            var descriptor = new CallStreamDescriptor(callId, (VideoStream) stream);

            if (!videoStreamCallbacks.ContainsKey(descriptor)) {
                return;
            }

            pump.BeginInvoke(() => {
                var texture = Texture2D.CreateExternalTexture(width, height, TextureFormat.RGBA32, false, false, new IntPtr(textureId));

                var newNativeTexture = new NativeTextureDescriptor{
                    context = oglContext,
                    textureId = textureId,
                    texture = texture
                };
                videoStreamCallbacks[descriptor](texture);
                if (nativeTextures.ContainsKey(descriptor)) {
                    var nativeTexture = nativeTextures[descriptor];
                    DestroyRenderer(nativeTexture.textureId, nativeTexture.context);
                }
                nativeTextures[descriptor] = newNativeTexture;
            });
        }

        protected void CleanupAllVideoStreams()
        {
            foreach (var texture in nativeTextures.Values) {
                DestroyRenderer(texture.textureId, texture.context);
            }

            nativeTextures.Clear();
            foreach (var pair in videoStreamCallbacks) {
                pair.Value(null);
            }
            videoStreamCallbacks.Clear();
        }

#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
        [DllImport ("__Internal")]
#else
        [DllImport("VoximplantAndroidRendererPlugin")]
#endif
        private static extern void DestroyRenderer(long textureId, long context);

        protected virtual void endUpdatingTexture(string callId, VideoStream stream)
        {
            var descriptor = new CallStreamDescriptor(callId, stream);

            if (videoStreamCallbacks.ContainsKey(descriptor)) {
                videoStreamCallbacks.Remove(descriptor);
            }
        }

        #region Local video stream

        /**
        Attach specified Unity camera as video source for not yet started call.
        @method useCameraVideoStreamForCall
        @param {string} callId Call identifier
        @param {UnityEngine.Camera} Camera to be used as video source.
        */
        public void useCameraVideoStreamForCall(string callId, UnityEngine.Camera streamCamera)
        {
            Debug.Log(string.Format("useCameraVideoStreamForCall {0}", callId));
            
            var newBehaviour = streamCamera.gameObject.GetComponent<StreamingCameraBehaviour>() 
                ?? streamCamera.gameObject.AddComponent<StreamingCameraBehaviour>(); 
            var behaviours = new List<StreamingCameraBehaviour>();
            var localStreamCameraBehaviours = _behavioursByCallId.ContainsKey(callId) ? _behavioursByCallId[callId] : null;
            if (localStreamCameraBehaviours != null) {
                behaviours.InsertRange(0, localStreamCameraBehaviours);
            }
            behaviours.Insert(behaviours.Count, newBehaviour);
            _behavioursByCallId[callId] = behaviours.ToArray();

            newBehaviour.Reset();
            newBehaviour.OnResolutionDidChange = (width, height) => {
                beginCallVideoStream(callId, (uint) width, (uint) height);
            };
            newBehaviour.OnTextureRendered = texture => {
                callVideoStreamTextureUpdated(callId, texture.GetNativeTexturePtr(), texture.width, texture.height);
            };
            newBehaviour.OnResolutionWillChange = () => {
                endCallVideoStream(callId);
            };

            newBehaviour.EnsureTextures();
        }

        /**
        Stop sending camera video stream to call.
        @method endSendingCameraVideoStreamForCall
        @param {string} callId Call identifier
        */
        public void endSendingCameraVideoStreamForCall(string callId)
        {
            foreach (var behaviour in _behavioursByCallId[callId]) {
                Destroy(behaviour);
            }
            _behavioursByCallId.Remove(callId);
        }

        private readonly Dictionary<string, StreamingCameraBehaviour[]> _behavioursByCallId = new Dictionary<string, StreamingCameraBehaviour[]>();

        protected abstract void beginCallVideoStream(string pCallId, uint width, uint height);
        protected abstract void callVideoStreamTextureUpdated(string pCallId, IntPtr pTexturePtr, int width, int height);
        protected abstract void endCallVideoStream(string pCallId);

        #endregion

        protected static bool GraphicsDeviceIsSupported()
        {
            switch (SystemInfo.graphicsDeviceType) {
                case GraphicsDeviceType.OpenGLES2:
                case GraphicsDeviceType.OpenGLES3:
                case GraphicsDeviceType.Metal:
                    return true;
                default:
                    return false;
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
