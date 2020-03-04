/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
package com.voximplant.unity;

import android.util.Log;

import com.voximplant.sdk.call.CallException;
import com.voximplant.sdk.call.CallSettings;
import com.voximplant.sdk.call.IAudioStream;
import com.voximplant.sdk.call.ICall;
import com.voximplant.sdk.call.ICallCompletionHandler;
import com.voximplant.sdk.call.ICallListener;
import com.voximplant.sdk.call.IEndpoint;
import com.voximplant.sdk.call.IVideoStream;
import com.voximplant.sdk.call.RejectMode;
import com.voximplant.sdk.call.VideoCodec;
import com.voximplant.sdk.call.VideoFlags;

import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class CallWrapper implements ICallListener {
    private final ICall mCall;
    private Map<String, EndpointWrapper> mEndpoints;
    private Map<String, VideoStreamWrapper> mVideoStreams;

    public CallWrapper(ICall call) {
        mEndpoints = new HashMap<>();
        mVideoStreams = new HashMap<>();

        mCall = call;
        mCall.addCallListener(this);
    }

    @CalledByUnity
    public EndpointWrapper getEndpoint(String endpointId) {
        return mEndpoints.get(endpointId);
    }

    @CalledByUnity
    public VideoStreamWrapper getVideoStream(String streamId) {
        Log.d("Bridge", "getVideoStream(" + streamId + ")");
        return mVideoStreams.get(streamId);
    }

    private EndpointWrapper getEndpointWrapper(IEndpoint endpoint) {
        EndpointWrapper endpointWrapper = mEndpoints.get(endpoint.getEndpointId());

        if (endpointWrapper == null) {
            endpointWrapper = new EndpointWrapper(endpoint, this);
            mEndpoints.put(endpoint.getEndpointId(), endpointWrapper);
        }

        return endpointWrapper;
    }

    private VideoStreamWrapper getVideoStreamWrapper(IVideoStream videoStream) {
        VideoStreamWrapper videoStreamWrapper = mVideoStreams.get(videoStream.getVideoStreamId());

        if (videoStreamWrapper == null) {
            videoStreamWrapper = new VideoStreamWrapper(videoStream);
            mVideoStreams.put(videoStream.getVideoStreamId(), videoStreamWrapper);
        }

        return videoStreamWrapper;
    }

    public void removeEndpointWrapper(EndpointWrapper endpointWrapper) {
        mEndpoints.remove(endpointWrapper.getEndpointId());
    }

    // region ICall
    @CalledByUnity
    public String getCallId() {
        if (mCall == null) return null;

        return mCall.getCallId();
    }

    @CalledByUnity
    private EndpointWrapper[] getEndpoints() {
        if (mCall == null) return null;

        List<IEndpoint> endpointList = mCall.getEndpoints();

        EndpointWrapper[] endpoints = new EndpointWrapper[endpointList.size()];
        for (int i = 0; i < endpointList.size(); i++) {
            endpoints[i] = getEndpointWrapper(endpointList.get(i));
        }

        return endpoints;
    }

    @CalledByUnity
    public boolean isVideoEnabled() {
        if (mCall == null) return false;

        return mCall.isVideoEnabled();
    }

    @CalledByUnity
    public void start() {
        if (mCall == null) return;

        try {
            mCall.start();
        } catch (CallException e) {
            Map<String, Object> payload = new HashMap<>();
            payload.put("code", ErrorHelper.CodeForCallError(e.getErrorCode()));
            payload.put("error", e.getMessage());
            Emitter.sendCallMessage(mCall.getCallId(), "Failed", payload);
        }
    }

    @CalledByUnity
    public void answer(boolean receiveVideo, boolean sendVideo, String videoCodec, String customData, String headers) {
        if (mCall == null) return;

        @SuppressWarnings("unchecked") Map<String, String> hdrs = Emitter.getGson().fromJson(headers, Map.class);

        CallSettings callSettings = new CallSettings();
        callSettings.videoFlags = new VideoFlags(receiveVideo, sendVideo);
        try {
            callSettings.preferredVideoCodec = VideoCodec.valueOf(videoCodec.toUpperCase());
        } catch (IllegalArgumentException exc) {
            callSettings.preferredVideoCodec = VideoCodec.AUTO;
        }
        callSettings.customData = customData;
        callSettings.extraHeaders = hdrs;

        try {
            mCall.answer(callSettings);
        } catch (CallException e) {
            Map<String, Object> payload = new HashMap<>();
            payload.put("code", ErrorHelper.CodeForCallError(e.getErrorCode()));
            payload.put("error", e.getMessage());
            Emitter.sendCallMessage(mCall.getCallId(), "Failed", payload);
        }
    }

    @CalledByUnity
    public void reject(String rejectMode, String headers) {
        if (mCall == null) return;

        @SuppressWarnings("unchecked") Map<String, String> hdrs = Emitter.getGson().fromJson(headers, Map.class);

        RejectMode mode;
        try {
            mode = RejectMode.valueOf(rejectMode.toUpperCase());
        } catch (IllegalArgumentException exc) {
            mode = RejectMode.DECLINE;
        }

        try {
            mCall.reject(mode, hdrs);
        } catch (CallException e) {
            Map<String, Object> payload = new HashMap<>();
            payload.put("code", ErrorHelper.CodeForCallError(e.getErrorCode()));
            payload.put("error", e.getMessage());
            Emitter.sendCallMessage(mCall.getCallId(), "Failed", payload);
        }
    }

    @CalledByUnity
    public void hangup(String headers) {
        if (mCall == null) return;

        @SuppressWarnings("unchecked") Map<String, String> hdrs = Emitter.getGson().fromJson(headers, Map.class);

        mCall.hangup(hdrs);
    }

    @CalledByUnity
    public void sendAudio(boolean sendAudio) {
        if (mCall == null) return;

        mCall.sendAudio(sendAudio);
    }

    @CalledByUnity
    public boolean sendVideo(boolean sendVideo, final String requestGuid) {
        if (mCall == null) {
            return false;
        }

        mCall.sendVideo(sendVideo, new ICallCompletionHandler() {
            @Override
            public void onComplete() {
                Map<String, Object> payload = new HashMap<>();
                payload.put("requestGuid", requestGuid);
                Emitter.sendCallMessage(mCall.getCallId(), "ActionCompleted", payload);
            }

            @Override
            public void onFailure(CallException e) {
                Map<String, Object> payload = new HashMap<>();
                payload.put("requestGuid", requestGuid);
                payload.put("code", ErrorHelper.CodeForCallError(e.getErrorCode()));
                payload.put("error", e.getMessage());
                Emitter.sendCallMessage(mCall.getCallId(), "ActionFailed", payload);
            }
        });

        return true;
    }

    @CalledByUnity
    public boolean hold(boolean setHold, final String requestGuid) {
        if (mCall == null) {
            return false;
        }

        mCall.hold(setHold, new ICallCompletionHandler() {
            @Override
            public void onComplete() {
                Map<String, Object> payload = new HashMap<>();
                payload.put("requestGuid", requestGuid);
                Emitter.sendCallMessage(mCall.getCallId(), "ActionCompleted", payload);
            }

            @Override
            public void onFailure(CallException e) {
                Map<String, Object> payload = new HashMap<>();
                payload.put("requestGuid", requestGuid);
                payload.put("code", ErrorHelper.CodeForCallError(e.getErrorCode()));
                payload.put("error", e.getMessage());
                Emitter.sendCallMessage(mCall.getCallId(), "ActionFailed", payload);
            }
        });

        return true;
    }

    @CalledByUnity
    public boolean receiveVideo(final String requestGuid) {
        if (mCall == null) {
            return false;
        }

        mCall.receiveVideo(new ICallCompletionHandler() {
            @Override
            public void onComplete() {
                Map<String, Object> payload = new HashMap<>();
                payload.put("requestGuid", requestGuid);
                Emitter.sendCallMessage(mCall.getCallId(), "ActionCompleted", payload);
            }

            @Override
            public void onFailure(CallException e) {
                Map<String, Object> payload = new HashMap<>();
                payload.put("requestGuid", requestGuid);
                payload.put("code", ErrorHelper.CodeForCallError(e.getErrorCode()));
                payload.put("error", e.getMessage());
                Emitter.sendCallMessage(mCall.getCallId(), "ActionFailed", payload);
            }
        });

        return true;
    }

    @CalledByUnity
    public void sendDTMF(String dtmf) {
        if (mCall == null) return;

        mCall.sendDTMF(dtmf);
    }

    @CalledByUnity
    public void sendInfo(String mimeType, String content, String headers) {
        if (mCall == null) return;

        @SuppressWarnings("unchecked") Map<String, String> hdrs = Emitter.getGson().fromJson(headers, Map.class);
        mCall.sendInfo(mimeType, content, hdrs);
    }

    @CalledByUnity
    public void sendMessage(String message) {
        if (mCall == null) return;

        mCall.sendMessage(message);
    }

    @CalledByUnity
    public void useCustomVideoSource(VideoSourceWrapper videoSourceWrapper) {
        if (mCall == null) return;

        mCall.useCustomVideoSource(videoSourceWrapper.getVideoSource());
    }

    @CalledByUnity
    public long getCallDuration() {
        if (mCall == null) return 0;

        return mCall.getCallDuration();
    }

    @CalledByUnity
    public VideoStreamWrapper[] getLocalVideoStreams() {
        if (mCall == null) return null;

        List<IVideoStream> videoStreamList = mCall.getLocalVideoStreams();

        VideoStreamWrapper[] videoStreams = new VideoStreamWrapper[videoStreamList.size()];
        for (int i = 0; i < videoStreamList.size(); i++) {
            videoStreams[i] = getVideoStreamWrapper(videoStreamList.get(i));
        }

        return videoStreams;
    }

    @CalledByUnity
    public String[] getLocalAudioStreams() {
        if (mCall == null) return null;

        List<IAudioStream> audioStreamList = mCall.getLocalAudioStreams();

        String[] audioStreams = new String[audioStreamList.size()];
        for (int i = 0; i < audioStreamList.size(); i++) {
            audioStreams[i] = audioStreamList.get(i).getAudioStreamId();
        }

        return audioStreams;
    }
    // endregion

    // region ICallListener
    @Override
    public void onCallConnected(ICall call, Map<String, String> headers) {
        Map<String, Object> payload = new HashMap<>();
        if (headers != null) {
            payload.put("headers", Emitter.flatten(headers));
        }
        Emitter.sendCallMessage(mCall.getCallId(), "Connected", payload);
    }

    @Override
    public void onCallDisconnected(ICall call, Map<String, String> headers, boolean answeredElsewhere) {
        Map<String, Object> payload = new HashMap<>();
        payload.put("answeredElsewhere", answeredElsewhere);
        if (headers != null) {
            payload.put("headers", Emitter.flatten(headers));
        }
        Emitter.sendCallMessage(mCall.getCallId(), "Disconnected", payload);
        mCall.removeCallListener(this);
    }

    @Override
    public void onCallRinging(ICall call, Map<String, String> headers) {
        Map<String, Object> payload = new HashMap<>();
        if (headers != null) {
            payload.put("headers", Emitter.flatten(headers));
        }
        Emitter.sendCallMessage(mCall.getCallId(), "Ringing", payload);
    }

    @Override
    public void onCallFailed(ICall call, int code, String description, Map<String, String> headers) {
        Map<String, Object> payload = new HashMap<>();
        payload.put("code", code);
        payload.put("error", description);
        if (headers != null) {
            payload.put("headers", Emitter.flatten(headers));
        }
        Emitter.sendCallMessage(mCall.getCallId(), "Failed", payload);
        mCall.removeCallListener(this);
    }

    @Override
    public void onCallAudioStarted(ICall call) {
        Emitter.sendCallMessage(mCall.getCallId(), "AudioStarted");
    }

    @Override
    public void onSIPInfoReceived(ICall call, String type, String content, Map<String, String> headers) {
        Map<String, Object> payload = new HashMap<>();
        payload.put("type", type);
        payload.put("body", content);
        if (headers != null) {
            payload.put("headers", Emitter.flatten(headers));
        }
        Emitter.sendCallMessage(mCall.getCallId(), "SIPInfoReceived", payload);
    }

    @Override
    public void onMessageReceived(ICall call, String text) {
        Map<String, Object> payload = new HashMap<>();
        payload.put("text", text);
        Emitter.sendCallMessage(mCall.getCallId(), "MessageReceived", payload);
    }

    @Override
    public void onLocalVideoStreamAdded(ICall call, IVideoStream videoStream) {
        getVideoStreamWrapper(videoStream);
        Emitter.sendCallMessage(mCall.getCallId(), "LocalVideoStreamAdded", Collections.singletonMap("streamId", videoStream.getVideoStreamId()));
    }

    @Override
    public void onLocalVideoStreamRemoved(ICall call, IVideoStream videoStream) {
        mVideoStreams.remove(videoStream.getVideoStreamId());
        Emitter.sendCallMessage(mCall.getCallId(), "LocalVideoStreamRemoved", Collections.singletonMap("streamId", videoStream.getVideoStreamId()));
    }

    @Override
    public void onICETimeout(ICall call) {
        Emitter.sendCallMessage(mCall.getCallId(), "ICETimeout");
    }

    @Override
    public void onICECompleted(ICall call) {
        Emitter.sendCallMessage(mCall.getCallId(), "ICECompleted");
    }

    @Override
    public void onEndpointAdded(ICall call, IEndpoint endpoint) {
        getEndpointWrapper(endpoint);
        Emitter.sendCallMessage(mCall.getCallId(), "EndpointAdded", Collections.singletonMap("endpointId", endpoint.getEndpointId()));
    }
    // endregion
}
