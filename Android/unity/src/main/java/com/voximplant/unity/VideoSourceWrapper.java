/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
package com.voximplant.unity;

import com.voximplant.sdk.Voximplant;
import com.voximplant.sdk.hardware.ICustomVideoSource;
import com.voximplant.sdk.hardware.ICustomVideoSourceListener;

@CalledByUnity
class VideoSourceWrapper implements ICustomVideoSourceListener {
    private final ICustomVideoSource mVideoSource;
    private ICustomVideoSourceListener mVideoSourceListener;
    private boolean mIsStarted;

    public VideoSourceWrapper() {
        mVideoSource = Voximplant.getCustomVideoSource();
        mVideoSource.setCustomVideoSourceListener(this);
    }

    @CalledByUnity
    public void sendFrame(int i, int i1, int i2) throws IllegalArgumentException {
        if (mIsStarted) {
            mVideoSource.sendFrame(i, i1, i2);
        }
    }

    @Override
    public void onStarted() {
        mIsStarted = true;
    }

    @Override
    public void onStopped() {
        mIsStarted = false;
    }

    public ICustomVideoSource getVideoSource() {
        return mVideoSource;
    }
}
