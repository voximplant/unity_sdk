/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
package com.voximplant.unity;

import com.voximplant.sdk.Voximplant;
import com.voximplant.sdk.hardware.AudioDevice;
import com.voximplant.sdk.hardware.IAudioDeviceEventsListener;
import com.voximplant.sdk.hardware.IAudioDeviceManager;

import java.util.Collections;
import java.util.List;

public class AudioManagerModule implements IAudioDeviceEventsListener {
    private final IAudioDeviceManager mAudioManager;

    @CalledByUnity
    public AudioManagerModule() {
        mAudioManager = Voximplant.getAudioDeviceManager();
        mAudioManager.addAudioDeviceEventsListener(this);
    }

    @CalledByUnity
    public void selectAudioDevice(String audioDevice) {
        AudioDevice device = AudioDevice.NONE;
        switch (audioDevice) {
            case "Earpiece":
                device = AudioDevice.EARPIECE;
                break;
            case "Speaker":
                device = AudioDevice.SPEAKER;
                break;
            case "WiredHeadset":
                device = AudioDevice.WIRED_HEADSET;
                break;
            case "Bluetooth":
                device = AudioDevice.BLUETOOTH;
                break;
        }
        mAudioManager.selectAudioDevice(device);
    }

    private static int getAudioDeviceCode(AudioDevice audioDevice) {
        switch (audioDevice) {
            case BLUETOOTH:
                return 4;
            case EARPIECE:
                return 1;
            case NONE:
            default:
                return 0;
            case SPEAKER:
                return 2;
            case WIRED_HEADSET:
                return 3;
        }
    }

    @CalledByUnity
    public int getActiveDevice() {
        return getAudioDeviceCode(mAudioManager.getActiveDevice());
    }

    @CalledByUnity
    public int[] getAudioDevices() {
        List<AudioDevice> audioDeviceList = mAudioManager.getAudioDevices();

        int[] audioDevices = new int[audioDeviceList.size()];
        for (int idx = 0; idx < audioDevices.length; idx++) {
            audioDevices[idx] = getAudioDeviceCode(audioDeviceList.get(idx));
        }
        return audioDevices;
    }

    @Override
    public void onAudioDeviceChanged(AudioDevice audioDevice) {
        Emitter.sendAudioManagerMessage("AudioDeviceChanged", Collections.singletonMap("device", getAudioDeviceCode(audioDevice)));
    }

    @Override
    public void onAudioDeviceListChanged(List<AudioDevice> audioDeviceList) {
        int[] audioDevices = new int[audioDeviceList.size()];
        for (int idx = 0; idx < audioDevices.length; idx++) {
            audioDevices[idx] = getAudioDeviceCode(audioDeviceList.get(idx));
        }

        Emitter.sendAudioManagerMessage("AudioDeviceListChanged", Collections.singletonMap("devices", audioDevices));
    }
}
