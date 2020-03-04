/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
package com.voximplant.unity;

import com.unity3d.player.UnityPlayer;
import com.voximplant.sdk.Voximplant;
import com.voximplant.sdk.hardware.ICameraManager;


public class CameraManagerModule {
    private final ICameraManager mCameraManager;
    private int mCameraId = 1;
    private int mCameraWidth = 640;
    private int mCameraHeight = 480;

    @CalledByUnity
    public CameraManagerModule() {
        mCameraManager = Voximplant.getCameraManager(UnityPlayer.currentActivity.getApplicationContext());
    }

    @CalledByUnity
    public void switchCamera(int cameraId) {
        if (mCameraId == cameraId) return;
        mCameraId = cameraId;

        mCameraManager.setCamera(mCameraId, mCameraWidth, mCameraHeight);
    }

    @CalledByUnity
    public void setCameraResolution(int width, int height) {
        if (mCameraWidth == width && mCameraHeight == height) return;
        mCameraWidth = width;
        mCameraHeight = height;

        mCameraManager.setCamera(mCameraId, mCameraWidth, mCameraHeight);
    }
}
