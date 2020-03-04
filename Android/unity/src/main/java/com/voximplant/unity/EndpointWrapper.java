/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
package com.voximplant.unity;

import android.telecom.Call;

import com.voximplant.sdk.call.IAudioStream;
import com.voximplant.sdk.call.IEndpoint;
import com.voximplant.sdk.call.IEndpointListener;
import com.voximplant.sdk.call.IVideoStream;

import java.lang.ref.WeakReference;
import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class EndpointWrapper implements IEndpointListener {
    private final IEndpoint mEndpoint;
    private Map<String, VideoStreamWrapper> mVideoStreams;
    private WeakReference<CallWrapper> mCallWrapper;

    public EndpointWrapper(IEndpoint endpoint, CallWrapper owner) {
        mVideoStreams = new HashMap<>();

        mEndpoint = endpoint;
        mCallWrapper = new WeakReference<>(owner);
        mEndpoint.setEndpointListener(this);
    }

    @CalledByUnity
    public VideoStreamWrapper getVideoStream(String streamId) {
        return mVideoStreams.get(streamId);
    }

    private VideoStreamWrapper getVideoStreamWrapper(IVideoStream videoStream) {
        VideoStreamWrapper videoStreamWrapper = mVideoStreams.get(videoStream.getVideoStreamId());

        if (videoStreamWrapper == null) {
            videoStreamWrapper = new VideoStreamWrapper(videoStream);
            mVideoStreams.put(videoStream.getVideoStreamId(), videoStreamWrapper);
        }

        return videoStreamWrapper;
    }

    // region IEndpoint
    @CalledByUnity
    public String getEndpointId() {
        return mEndpoint.getEndpointId();
    }

    @CalledByUnity
    public String getUserName() {
        return mEndpoint.getUserName();
    }

    @CalledByUnity
    public String getUserDisplayName() {
        return mEndpoint.getUserDisplayName();
    }

    @CalledByUnity
    public String getSipUri() {
        return mEndpoint.getSipUri();
    }

    @CalledByUnity
    public VideoStreamWrapper[] getVideoStreams() {
        List<IVideoStream> videoStreamList = mEndpoint.getVideoStreams();

        VideoStreamWrapper[] videoStreams = new VideoStreamWrapper[videoStreamList.size()];
        for (int i = 0; i < videoStreamList.size(); i++) {
            videoStreams[i] = getVideoStreamWrapper(videoStreamList.get(i));
        }

        return videoStreams;
    }

    @CalledByUnity
    public String[] getAudioStreams() {
        List<IAudioStream> audioStreamList = mEndpoint.getAudioStreams();

        String[] audioStreams = new String[audioStreamList.size()];
        for (int i = 0; i < audioStreamList.size(); i++) {
            audioStreams[i] = audioStreamList.get(i).getAudioStreamId();
        }

        return audioStreams;
    }
    // endregion

    // region IEndpointListener
    @Override
    public void onRemoteVideoStreamAdded(IEndpoint endpoint, IVideoStream videoStream) {
        VideoStreamWrapper wrapper = getVideoStreamWrapper(videoStream);
        Emitter.sendEndpointMessage(endpoint.getEndpointId(), "RemoteVideoStreamAdded", Collections.singletonMap("streamId", wrapper.getVideoStreamId()));
    }

    @Override
    public void onRemoteVideoStreamRemoved(IEndpoint endpoint, IVideoStream videoStream) {
        mVideoStreams.remove(videoStream.getVideoStreamId());
        Emitter.sendEndpointMessage(endpoint.getEndpointId(), "RemoteVideoStreamRemoved", Collections.singletonMap("streamId", videoStream.getVideoStreamId()));
    }

    @Override
    public void onEndpointRemoved(IEndpoint endpoint) {
        mCallWrapper.get().removeEndpointWrapper(this);
        Emitter.sendEndpointMessage(endpoint.getEndpointId(), "Removed");
        endpoint.setEndpointListener(null);
    }

    @Override
    public void onEndpointInfoUpdated(IEndpoint endpoint) {
        Emitter.sendEndpointMessage(endpoint.getEndpointId(), "InfoUpdated");
    }
    // endregion
}
