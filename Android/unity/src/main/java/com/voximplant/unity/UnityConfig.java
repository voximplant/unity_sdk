/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
package com.voximplant.unity;

import com.google.gson.annotations.SerializedName;
import com.voximplant.sdk.client.RequestAudioFocusMode;

public class UnityConfig {
    @SerializedName("enableDebugLogging")
    private boolean mEnableDebugLogging;

    @SerializedName("enableLogcatLogging")
    private boolean mEnableLogcatLogging;

    @SerializedName("audioFocusMode")
    private int mAudioFocusMode;

    public boolean isEnableDebugLogging() {
        return mEnableDebugLogging;
    }

    public boolean isEnableLogcatLogging() {
        return mEnableLogcatLogging;
    }

    public RequestAudioFocusMode getAudioFocusMode() {
        switch (mAudioFocusMode) {
            default:
            case 0:
                return RequestAudioFocusMode.REQUEST_ON_CALL_START;
            case 1:
                return RequestAudioFocusMode.REQUEST_ON_CALL_CONNECTED;
        }
    }
}
