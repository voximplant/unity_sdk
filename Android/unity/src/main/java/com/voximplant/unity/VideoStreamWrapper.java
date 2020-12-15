/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
package com.voximplant.unity;

import android.util.Log;

import com.voximplant.sdk.call.IVideoStream;
import com.voximplant.sdk.call.RenderScaleType;

import org.webrtc.VideoFrame;
import org.webrtc.VideoSink;

import java.lang.ref.WeakReference;

public class VideoStreamWrapper implements VideoSink {
    private VideoFrame mLastFrame;
    private final String mStreamId;

    public VideoStreamWrapper(IVideoStream stream) {
        mStreamId = stream.getVideoStreamId();
        stream.addVideoRenderer(this, RenderScaleType.SCALE_FILL);
    }

    public String getVideoStreamId() {
        return mStreamId;
    }

    @Override
    public void onFrame(VideoFrame videoFrame) {
        mLastFrame = videoFrame;
    }

    @CalledByUnity
    public int getWidth() {
        if (mLastFrame != null) {
            return mLastFrame.getRotatedWidth();
        }
        return 0;
    }

    @CalledByUnity
    public int getHeight() {
        if (mLastFrame != null) {
            return mLastFrame.getRotatedHeight();
        }
        return 0;
    }

    @CalledByUnity
    public int getRotation() {
        if (mLastFrame != null) {
            return mLastFrame.getRotation();
        }
        return 0;
    }

    @CalledByUnity
    public int getTextureId() {
        int textureId = 0;
        if (mLastFrame != null && mLastFrame.getBuffer() instanceof VideoFrame.TextureBuffer) {
            textureId = ((VideoFrame.TextureBuffer) mLastFrame.getBuffer()).getTextureId();
        }
        return textureId;
    }
}
