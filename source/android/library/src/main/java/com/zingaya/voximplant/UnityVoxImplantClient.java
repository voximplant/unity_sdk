/*
 * Copyright (c) 2017, Zingaya, Inc. All rights reserved.
 */

package com.zingaya.voximplant;

import java.security.AccessControlException;

import android.Manifest;
import android.content.Context;
import android.content.pm.PackageManager;
import android.util.Log;

import com.voximplant.sdk.call.ICallCompletionHandler;
import com.voximplant.sdk.call.RenderScaleType;
import com.voximplant.sdk.client.ClientConfig;
import com.voximplant.sdk.client.IClient;
import com.voximplant.sdk.hardware.ICustomVideoSource;
import com.voximplant.sdk.hardware.VideoQuality;

import org.webrtc.VideoRenderer;

import java.util.List;
import java.util.Map;
import java.util.concurrent.Executor;

/**
 * @deprecated Use {@link IClient} instead
 */
@Deprecated
public class UnityVoxImplantClient {

	/**
	 * @deprecated Use {@link com.voximplant.sdk.client.ClientConfig} instead
	 */
	@Deprecated
	public static class VoxImplantClientConfig {

		/**
		 * Enable video functionality.&nbsp;Set to true by default.
		 * @deprecated Use {@link com.voximplant.sdk.client.ClientConfig#enableVideo} instead
		 */
		@Deprecated
		public boolean enableVideo;
		/**
		 * Enable hardware video acceleration.&nbsp;Set to true by default.
		 * Should be set to false, if provideLocalFramesInByteBuffers is set to true
		 * @deprecated Use {@link com.voximplant.sdk.client.ClientConfig#enableHWAcceleration} instead
		 */
		@Deprecated
		public boolean enableHWAcceleration;
		/**
		 * Request video frames from camera in I420 format with byte buffers.&nbsp;Set to false by default.<br>
		 * Should be used only in case of custom implementation of video renderer (VideoRenderer.Callbacks class).<br>
		 * If set to true, VideoRenderer.Callbacks.renderFrame() will always provide the frames from
		 * camera in I420 format with byte buffers, and <i>enableHWAcceleration</i> should be set to false.<br>
		 * If set to false, video frames from camera will be provided in I420 format with textures.
		 * @deprecated Use {@link com.voximplant.sdk.client.ClientConfig#provideLocalFramesInByteBuffers} instead.
		 */
		@Deprecated
		public boolean provideLocalFramesInByteBuffers;
		/**
		 * Enable debug logging.&nbsp;Set to false by default.
		 * @deprecated Use {@link com.voximplant.sdk.client.ClientConfig#enableDebugLogging} instead
		 */
		@Deprecated
		public boolean enableDebugLogging;

		/**
		 * @deprecated Use {@link ClientConfig#ClientConfig()} instead
		 */
		@Deprecated
		public VoxImplantClientConfig() {
			enableVideo = true;
			enableHWAcceleration = true;
			provideLocalFramesInByteBuffers = false;
			enableDebugLogging = false;
		}
	}

	private static String TAG = "VOXSDK";
	private static UnityVoxImplantClient inst = null;
	private UnityVoxImplantClientImp unityVoxImplantClientImp = null;
	private Context context = null;
	private String firebaseToken = null;
	private VoxImplantClientConfig clientConfig;


	/**
	 * @deprecated Use {@link com.voximplant.sdk.Voximplant#getClientInstance(Executor, Context, ClientConfig)} instead
	 * @return UnityVoxImplantClient shared instance
	 */
	@Deprecated
	public static UnityVoxImplantClient instance() {
		if (inst == null)
			inst = new UnityVoxImplantClient();
		return inst;
	}

	private UnityVoxImplantClientImp imp() {
		if (unityVoxImplantClientImp == null && context != null) {
			unityVoxImplantClientImp = new UnityVoxImplantClientImp(context, clientConfig);
			if (firebaseToken != null) {
				unityVoxImplantClientImp.registerForPushNotifications(firebaseToken);
				firebaseToken = null;
			}
		}
		return unityVoxImplantClientImp;
	}

	private UnityVoxImplantClient() {
	}

	/**
	 * Set Android context
	 *
	 * @param context Android context
	 * @throws AccessControlException
	 * @deprecated Android context is provided to SDK via {@link com.voximplant.sdk.Voximplant#getClientInstance(Executor, Context, ClientConfig)} instead
	 */
	@Deprecated
	public void setAndroidContext(Context context) throws AccessControlException {
		setAndroidContext(context, new VoxImplantClientConfig());
	}

	/**
	 * Set Android context and UnityVoxImplantClient config
	 *
	 * @param context        Android context
	 * @param clientConfig   VoxImplantClientConfig instance with configuration for Android SDK
	 * @throws AccessControlException
	 * @see UnityVoxImplantClient.VoxImplantClientConfig
	 * @deprecated Android context and client configuration are provided to SDK via {@link com.voximplant.sdk.Voximplant#getClientInstance(Executor, Context, ClientConfig)} instead
	 */
	@Deprecated
	public void setAndroidContext(Context context, VoxImplantClientConfig clientConfig) throws AccessControlException {
		this.context = context;
		this.clientConfig = clientConfig;

		if (context.getPackageManager().checkPermission(Manifest.permission.RECORD_AUDIO, context.getPackageName()) == PackageManager.PERMISSION_DENIED) {
			throw new AccessControlException(Manifest.permission.RECORD_AUDIO);
		}
		if (context.getPackageManager().checkPermission(Manifest.permission.MODIFY_AUDIO_SETTINGS, context.getPackageName()) == PackageManager.PERMISSION_DENIED) {
			throw new AccessControlException(Manifest.permission.MODIFY_AUDIO_SETTINGS);
		}
		if (context.getPackageManager().checkPermission(Manifest.permission.INTERNET, context.getPackageName()) == PackageManager.PERMISSION_DENIED) {
			throw new AccessControlException(Manifest.permission.INTERNET);
		}

		if (this.clientConfig.enableVideo) {
			if (context.getPackageManager().checkPermission(Manifest.permission.CAMERA, context.getPackageName()) == PackageManager.PERMISSION_DENIED) {
				throw new AccessControlException(Manifest.permission.CAMERA);
			}
		}
	}

	/**
	 * Sets handler for VoxImplant events
	 *
	 * @param cb Callback object
	 * @deprecated Use {@link com.voximplant.sdk.client.IClientSessionListener}, {@link com.voximplant.sdk.client.IClientLoginListener},
	 * {@link com.voximplant.sdk.client.IClientIncomingCallListener} and {@link com.voximplant.sdk.call.ICallListener} instead
	 */
	@Deprecated
	public void setCallback(VoxImplantCallback cb) {
		Log.i(TAG, "UnityVoxImplantClient: setCallback()");
		imp().setCallback(cb);
	}

	/**
	 * Connect to VoxImplant cloud
	 * @deprecated Use {@link IClient#connect()} instead
	 */
	@Deprecated
	public void connect() {
		Log.i(TAG, "UnityVoxImplantClient: connect()");
		imp().connect(true, null);
	}

	/**
	 * Connect to VoxImplant cloud
	 *
	 * @param connectivityCheck Checks whether UDP traffic will flow correctly between device and VoxImplant cloud. This check reduces connection speed.
	 * @param serverNames        Server name of particular media gateway for connection
	 * @deprecated Use {@link IClient#connect(boolean, List)} instead
	 */
	@Deprecated
	public void connect(boolean connectivityCheck, List<String> serverNames) {
		Log.i(TAG, "UnityVoxImplantClient: connect()");
		imp().connect(connectivityCheck, serverNames);
	}

	/**
	 * Closes connection with media server
	 * @deprecated Use {@link IClient#disconnect()} instead
	 */
	@Deprecated
	public void closeConnection() {
		Log.i(TAG, "UnityVoxImplantClient: closeConnection()");
		imp().disconnect();
	}

	/**
	 * Login to specified VoxImplant application
	 *
	 * @param username Full user name, including app and account name, like <i>someuser@someapp.youraccount.voximplant.com</i>
	 * @param password User password
	 * @deprecated Use {@link IClient#login(String, String)} instead
	 */
	@Deprecated
	public void login(String username, String password) {
		Log.i(TAG, "UnityVoxImplantClient: login()");
		imp().login(username, password);
	}

	/**
	 * Perform login using one time key that was generated before
	 *
	 * @param user Full user name, including app and account name, like <i>someuser@someapp.youraccount.voximplant.com</i>
	 * @param hash Hash that was generated using following formula:
	 *             MD5(oneTimeKey+"|"+MD5(user+":voximplant.com:"+password)).
	 *             <b>Please note that here user is just a user name, without app name,
	 *             account name or anything else after "@"</b>. So if you pass <i>myuser@myapp.myacc.voximplant.com</i> as a <b>username</b>,
	 *             you should only use <i>myuser</i> while computing this hash.
	 * @deprecated Use {@link IClient#loginWithOneTimeKey(String, String)} instead
	 */
	@Deprecated
	public void loginUsingOneTimeKey(String user, String hash) {
		Log.i(TAG, "UnityVoxImplantClient: loginUsingOneTimeKey()");
		imp().loginUsingOneTimeKey(user, hash);
	}

	/**
	 * Generates one time login key to be used for automated login process.
	 *
	 * @param user Full user name, including app and account name, like <i>someuser@someapp.youraccount.voximplant.com</i>
	 * @deprecated Use {@link IClient#requestOneTimeKey(String)} instead
	 * @see <a href="http://voximplant.com/docs/quickstart/24/automated-login/">Information about automated login on VoxImplant website</a>
	 * @see UnityVoxImplantClient#loginUsingOneTimeKey
	 */
	@Deprecated
	public void requestOneTimeKey(String user) {
		Log.i(TAG, "UnityVoxImplantClient: requestOneTimeKey()");
		imp().requestOneTimeKey(user);
	}

	/**
	 * Perform login using specified username and access token that was obtained in onLoginSuccessful callback before
	 *
	 * @param user Full user name, including app and account name, like <i>someuser@someapp.youraccount.voximplant.com</i>
	 * @param accessToken access token that was obtained in onLoginSuccessful callback
	 * @deprecated Use {@link IClient#loginWithAccessToken(String, String)} instead
     * @see VoxImplantCallback#onLoginSuccessful(String, VoxImplantCallback.LoginTokens)
     * @see VoxImplantCallback.LoginTokens#getAccessToken()
	 */
	@Deprecated
	public void loginUsingAccessToken(String user, String accessToken) {
		Log.i(TAG, "UnityVoxImplantClient: loginUsingAccessToken");
		imp().loginUsingAccessToken(user, accessToken);
	}

	/**
	 * Perform refresh of login tokens required for login using access token
	 *
	 * @param user Full user name, including app and account name, like <i>someuser@someapp.youraccount.voximplant.com</i>
	 * @param refreshToken refresh token that was obtained in onLoginSuccessful callback
	 * @deprecated Use {@link IClient#refreshToken(String, String)} instead
     * @see UnityVoxImplantClient#loginUsingAccessToken(String, String)
     * @see VoxImplantCallback.LoginTokens#getRefreshToken()
	 */
	@Deprecated
	public void refreshToken(String user, String refreshToken) {
		Log.i(TAG, "UnityVoxImplantClient: refreshToken");
		imp().refreshToken(user, refreshToken);
	}

	/**
	 * Create new createCall instance.
	 * Call must be then started using startCall
	 *
	 * @param to         SIP URI, username or phone number to make createCall to. Actual routing is then performed by VoxEngine scenario
	 * @param video      Enable video support in createCall
	 * @param customData Optional custom data passed with createCall. Will be available in VoxEngine scenario
	 * @return Call id of newly created createCall, null if the createCall was not created. Created createCall id should be used to perform any subsequent createCall operations
	 * @deprecated Use {@link IClient#callTo(String, boolean, String)} instead
	 */
	@Deprecated
	public String createCall(String to, boolean video, String customData) {
		Log.i(TAG, "UnityVoxImplantClient: createCall(to = " + to + ", video = " + video + ", customData = " + customData + ")");
		return imp().createCall(to, video, customData);
	}

	/**
	 * Send start createCall request
	 * If createCall with specified id is not found - returns false
	 *
	 * @param callId  id of previously created createCall
	 * @param headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
	 * @return true on success, false if operation has failed
	 * @deprecated Use {@link com.voximplant.sdk.call.ICall#start(Map)} instead
	 */
	@Deprecated
	public boolean startCall(String callId, Map<String, String> headers) {
		Log.i(TAG, "UnityVoxImplantClient: startCall(callId = " + callId + ", headers = " + headers + ")");
		return imp().startCall(callId, headers);
	}

	/**
	 * Send start createCall request
	 * If createCall with specified id is not found - returns false
	 *
	 * @param callId id of previously created createCall
	 * @return true on success, false if operation has failed
	 * @deprecated Use {@link com.voximplant.sdk.call.ICall#start(Map)} instead
	 */
	@Deprecated
	public boolean startCall(String callId) {
		Log.i(TAG, "UnityVoxImplantClient: startCall()");
		return startCall(callId, null);
	}

	/**
	 * Sends DTMF digit in specified createCall.
	 *
	 * @param callId id of previously created createCall
	 * @param digit  Digit can be 0-9 for 0-9, 10 for * and 11 for #
	 * @deprecated Use {@link com.voximplant.sdk.call.ICall#sendDTMF(String)} instead
	 */
	@Deprecated
	public void sendDTMF(String callId, int digit) {
		Log.i(TAG, "UnityVoxImplantClient: sendDTMF()");
		imp().sendDTMF(callId, digit);
	}

	/**
	 * Terminate specified createCall. Call must be either established, or outgoing progressing
	 *
	 * @param callId  id of previously created createCall
	 * @param headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by Voximplant
	 * @deprecated Use {@link com.voximplant.sdk.call.ICall#hangup(Map)} instead
	 */
	@Deprecated
	public void disconnectCall(String callId, Map<String, String> headers) {
		Log.i(TAG, "UnityVoxImplantClient: disconnectCall()");
		imp().hangupCall(callId, headers);
	}

	/**
	 * Terminate specified createCall. Call must be either established, or outgoing progressing
	 *
	 * @param callId id of previously created createCall
	 * @deprecated Use {@link com.voximplant.sdk.call.ICall#hangup(Map)} instead
	 */
	@Deprecated
	public void disconnectCall(String callId) {
		Log.i(TAG, "UnityVoxImplantClient: disconnectCall()");
		disconnectCall(callId, null);
	}

	/**
	 * Reject incoming alerting createCall
	 *
	 * @param callId  id of previously created createCall
	 * @param headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
	 * @deprecated Use {@link com.voximplant.sdk.call.ICall#reject(Map)} instead
	 */
	@Deprecated
	public void declineCall(String callId, Map<String, String> headers) {
		Log.i(TAG, "UnityVoxImplantClient: declineCall(callId = " + callId + ",headers = " + headers + ")");
		imp().declineCall(callId, headers);
	}

	/**
	 * Reject incoming alerting createCall
	 *
	 * @param callId id of previously created createCall
	 * @deprecated Use {@link com.voximplant.sdk.call.ICall#reject(Map)} instead
	 */
	@Deprecated
	public void declineCall(String callId) {
		Log.i(TAG, "UnityVoxImplantClient: declineCall(callId = " + callId + ")");
		declineCall(callId, null);
	}

	/**
	 * Answer incoming createCall
	 *
	 * @param callId  id of previously created createCall
	 * @param headers Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
	 * @deprecated Use {@link com.voximplant.sdk.call.ICall#answer(Map)} instead
	 */
	@Deprecated
	public void answerCall(String callId, Map<String, String> headers) {
		Log.i(TAG, "UnityVoxImplantClient: answerCall(callId = " + callId + ", headers = " + headers + ")");
		imp().answerCall(callId, null, headers);
	}

	/**
	 * Answer incoming createCall
	 *
	 * @param callId id of previously created createCall
	 * @deprecated Use {@link com.voximplant.sdk.call.ICall#answer(Map)} instead
	 */
	@Deprecated
	public void answerCall(String callId) {
		Log.i(TAG, "UnityVoxImplantClient: answerCall()");
		imp().answerCall(callId, null, null);
	}

	/**
	 * Sends instant message within established createCall
	 *
	 * @param callId id of previously created createCall
	 * @param text   Message text
	 * @deprecated Use {@link com.voximplant.sdk.call.ICall#sendMessage(String)} instead
	 */
	@Deprecated
	public void sendMessage(String callId, String text) {
		Log.i(TAG, "UnityVoxImplantClient: sendMessage()");
		imp().sendMessage(callId, text);
	}

	/**
	 * Sends info within established createCall
	 *
	 * @param callId   id of previously created createCall
	 * @param mimeType MIME type of info
	 * @param content  Custom string data
	 * @param headers  Optional set of headers to be sent with message. Names must begin with "X-" to be processed by SDK
	 * @deprecated Use {@link com.voximplant.sdk.call.ICall#sendInfo(String, String, Map)} instead
	 */
	@Deprecated
	public void sendInfo(String callId, String mimeType, String content, Map<String, String> headers) {
		Log.i(TAG, "UnityVoxImplantClient: sendInfo()");
		imp().sendInfo(callId, mimeType, content, headers);
	}

	/**
	 * Sends info within established createCall
	 *
	 * @param callId   id of previously created createCall
	 * @param mimeType MIME type of info
	 * @param content  Custom string data
	 * @deprecated Use {@link com.voximplant.sdk.call.ICall#sendInfo(String, String, Map)} instead
	 */
	@Deprecated
	public void sendInfo(String callId, String mimeType, String content) {
		Log.i(TAG, "UnityVoxImplantClient: sendInfo()");
		sendInfo(callId, mimeType, content, null);
	}

	/**
	 * Get createCall duration for established createCall
	 *
	 * @param callId id of previously created createCall
	 * @return createCall duration in milliseconds
	 */
	@Deprecated
	long getCallDuration(String callId) {
		return imp().getCallDuration(callId);
	}

	/**
	 * Set view for local stream
	 *
	 * @param videoView org.webrtc.SurfaceViewRenderer or custom implementation of org.webrtc.VideoRenderer.Callbacks<br>
	 *                  It is recommended to use SurfaceViewRenderer for the most of cases.<br>
	 *                  In case of custom implementation of VideoRenderer.Callback class:
	 *                  <ul>
	 *                  	<li>1. VideoRenderer.renderFrameDone(i420Frame) must be called after every VideoRenderer.renderFrame()</li>
	 *                  	<li>2. See <i>VoxImplamtClientConfig</i> parameters to set up video frame format</li>
	 *                  </ul>
	 * @deprecated Use {@link com.voximplant.sdk.call.IVideoStream#addVideoRenderer(VideoRenderer.Callbacks, RenderScaleType)} instead
	 */
	@Deprecated
	public void setLocalPreview(VideoRenderer.Callbacks videoView) {
		Log.i(TAG, "UnityVoxImplantClient: setLocalPreview()");
		imp().setLocalPreview(videoView);
	}

	/**
	 * Set view for remote stream
	 *
	 * @param videoView org.webrtc.SurfaceViewRenderer or custom implementation of org.webrtc.VideoRenderer.Callbacks<br>
	 *                  It is recommended to use SurfaceViewRenderer for the most of cases.<br>
	 *                  In case of custom implementation of VideoRenderer.Callback class:
	 *                  <ul>
	 *                  	<li>1. VideoRenderer.renderFrameDone(i420Frame) must be called after every VideoRenderer.renderFrame()</li>
	 *                  	<li>2. See <i>VoxImplamtClientConfig</i> parameters to set up video frame format</li>
	 *                  </ul>
	 * @deprecated Use {@link com.voximplant.sdk.call.IVideoStream#addVideoRenderer(VideoRenderer.Callbacks, RenderScaleType)} instead
	 */
	@Deprecated
	public void setRemoteView(VideoRenderer.Callbacks videoView) {
		Log.i(TAG, "UnityVoxImplantClient: setRemoteView()");
		imp().setRemoteView(videoView);
	}

	/**
	 * Set view for remote stream for createCall
	 *
	 * @param videoView org.webrtc.SurfaceViewRenderer or custom implementation of org.webrtc.VideoRenderer.Callbacks<br>
	 *                  It is recommended to use SurfaceViewRenderer for the most of cases.<br>
	 *                  In case of custom implementation of VideoRenderer.Callback class:
	 *                  <ul>
	 *                  	<li>1. VideoRenderer.renderFrameDone(i420Frame) must be called after every VideoRenderer.renderFrame()</li>
	 *                  	<li>2. See <i>VoxImplamtClientConfig</i> parameters to set up video frame format</li>
	 *                  </ul>
	 * @param callId    id of the createCall
	 * @deprecated Use {@link com.voximplant.sdk.call.IVideoStream#addVideoRenderer(VideoRenderer.Callbacks, RenderScaleType)} instead
	 */
	@Deprecated
	public void setRemoteView(String callId, VideoRenderer.Callbacks videoView) {
		Log.i(TAG, "UnityVoxImplantClient: setRemoteView( callId = " + callId + ")");
		imp().setRemoteView(callId, videoView);
	}

	/**
	 * Set local camera resolution
	 *
	 * @param width  camera resolution width
	 * @param height camera resolution height
	 * @deprecated Use {@link com.voximplant.sdk.hardware.ICameraManager#setCamera(int, int, int)} or {@link com.voximplant.sdk.hardware.ICameraManager#setCamera(int, VideoQuality)} instead
	 */
	@Deprecated
	public void setCameraResolution(int width, int height) {
		Log.i(TAG, "UnityVoxImplantClient: setCameraResolution(" + width + "x" + height + ")");
		imp().setCameraResolution(width, height);
	}

	/**
	 * Select camera
	 *
	 * @param cam Must be Camera.CameraInfo.CAMERA_FACING_FRONT or Camera.CameraInfo.CAMERA_FACING_BACK
	 * @deprecated Use {@link com.voximplant.sdk.hardware.ICameraManager#setCamera(int, int, int)} or {@link com.voximplant.sdk.hardware.ICameraManager#setCamera(int, VideoQuality)} instead
	 */
	@Deprecated
	public void setCamera(int cam) {
		Log.i(TAG, "UnityVoxImplantClient: setCamera(" + cam + ")");
		imp().setCamera(cam);
	}

	/**
	 * Start/stop sending video from local camera
	 *
	 * @param doSendVideo Specify if video should be sent
	 * @deprecated Use {@link com.voximplant.sdk.call.ICall#sendVideo(boolean, ICallCompletionHandler)} instead
	 */
	@Deprecated
	public void sendVideo(boolean doSendVideo) {
		Log.i(TAG, "UnityVoxImplantClient: sendVideo(" + doSendVideo + ")");
		imp().sendVideo(doSendVideo);
	}

	/**
	 * Mute or unmute microphone. This is reset after audio interruption
	 *
	 * @param doMute Enable/disable flag
	 * @deprecated Use {@link com.voximplant.sdk.call.ICall#sendAudio(boolean)} instead
	 */
	@Deprecated
	public void setMute(boolean doMute) {
		Log.i(TAG, "UnityVoxImplantClient: setMute(" + doMute + ")");
		imp().setMute(doMute);
	}

	/**
	 * Enable/disable loudspeaker
	 *
	 * @param useLoudSpeaker Enable/disable loudspeaker
	 * @deprecated Use {@link com.voximplant.sdk.hardware.IAudioDeviceManager#enableLoudspeaker(boolean)} instead
	 */
	@Deprecated
	public boolean setUseLoudspeaker(boolean useLoudSpeaker) {
		Log.i(TAG, "UnityVoxImplantClient: setUseLoudspeaker(" + useLoudSpeaker + ")");
		return imp().setUseLoudspeaker(useLoudSpeaker);
	}

	/**
	 * Returns the list of permissions that have not been granted by user yet
	 *
	 * @param context             Android context
	 * @param videoSupportEnabled Specify if permissions for video calls are required.
	 * @return List of disallowed permissions
	 * @deprecated Use {@link com.voximplant.sdk.Voximplant#getMissingPermissions(Context, boolean)} instead
	 */
	@Deprecated
	public static List<String> getMissingPermissions(Context context, boolean videoSupportEnabled) {
		return UnityVoxImplantClientImp.getMissingPermissions(context, videoSupportEnabled);
	}


	/**
	 * Handle incoming push notification
	 *
	 * @param notification Incoming push notification that comes from
	 *                     FirebaseMessagingService.onMessageReceived(RemoteMessage remoteMessage)
	 * @deprecated Use {@link IClient#handlePushNotification(Map)} instead
	 */
	@Deprecated
	public void handlePushNotification(Map<String, String> notification) {
		imp().handlePushNotification(notification);
	}

	/**
	 * Register for push notifications. Application will receive push notifications from VoxImplant
	 * Server after first log in.
	 *
	 * @param pushRegistrationToken FCM registration token that can be retrieved by calling FirebaseInstanceID.getToken()
	 * @deprecated Use {@link IClient#registerForPushNotifications(String)} instead
	 */
	@Deprecated
	public void registerForPushNotifications(String pushRegistrationToken) {
		if (imp() == null) {
			firebaseToken = pushRegistrationToken;
		} else {
			imp().registerForPushNotifications(pushRegistrationToken);
		}
	}

	/**
	 * Unregister from push notifications. Application will no longer receive push notifications from
	 * VoxImplant server
	 *
	 * @param pushRegistrationToken FCM registration token that was used to register for push notifications
	 * @deprecated Use {@link IClient#unregisterFromPushNotifications(String)} instead
	 * @see UnityVoxImplantClient#registerForPushNotifications(String)
     */
	@Deprecated
	public void unregisterFromPushNotifications(String pushRegistrationToken) {
        if (imp() != null) {
            imp().unregisterFromPushNotifications(pushRegistrationToken);
        }
    }

	public void useCustomVideoSource(String callId, ICustomVideoSource customVideoSource) {
		if (imp() != null) {
			imp().useCustomVideoSource(callId, customVideoSource);
		}
	}
}
