/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
package com.voximplant.unity;

import android.opengl.EGL14;
import android.opengl.EGLContext;
import android.opengl.EGLDisplay;
import android.opengl.EGLSurface;
import android.util.Log;

import org.webrtc.EglBase;

import javax.microedition.khronos.egl.EGL10;

public class GLContextHelper {
    private static final String TAG = "GLContextHelper";
    private static EglBase rootEglBase;

    public static EGLContext unityContext = null;
    public static EGLDisplay unityDisplay = null;
    public static EGLSurface unityDrawSurface = null;
    public static EGLSurface unityReadSurface = null;

    public static EglBase getRootEglBase() {
        if (rootEglBase != null) {
            return rootEglBase;
        }

        Log.d(TAG, "eglContext: " + unityContext);
        EGLContext eglContext = unityContext; // getEglContext();
        Log.d(TAG, "eglDisplay: " + unityDisplay);
        EGLDisplay eglDisplay = unityDisplay; // getEglDisplay();
        int[] configAttributes = EglBase.CONFIG_PLAIN;// getEglConfigAttr(eglDisplay, eglContext);

        rootEglBase = EglBase.createEgl14(eglContext, configAttributes);
        rootEglBase.createDummyPbufferSurface();
        return rootEglBase;
    }

    public static void setUnityContext() {
        if (!Thread.currentThread().getName().equals("UnityMain")) {
            Log.d(TAG, "eglContextSet: wrong thread " + Thread.currentThread().getName());
            return;
        }

        unityContext = EGL14.eglGetCurrentContext();
        if (unityContext == EGL14.EGL_NO_CONTEXT) {
        }
        unityDisplay = EGL14.eglGetCurrentDisplay();
        Log.d(TAG, "eglContextSet: glContext " + unityContext.getHandle() + ", display " + unityDisplay.getNativeHandle());
        unityDrawSurface = EGL14.eglGetCurrentSurface(EGL14.EGL_DRAW);
        unityReadSurface = EGL14.eglGetCurrentSurface(EGL14.EGL_READ);

        if (unityContext == EGL14.EGL_NO_CONTEXT) {
            Log.d(TAG, "eglContextSet: unityContext == EGL_NO_CONTEXT");
        }
        if (unityDisplay == EGL14.EGL_NO_DISPLAY) {
            Log.d(TAG, "eglContextSet: unityDisplay == EGL_NO_DISPLAY");
        }
        if (unityDrawSurface == EGL14.EGL_NO_SURFACE) {
            Log.d(TAG, "eglContextSet: unityDrawSurface == EGL_NO_SURFACE");
        }
        if (unityReadSurface == EGL14.EGL_NO_SURFACE) {
            Log.d(TAG, "eglContextSet: unityReadSurface == EGL_NO_SURFACE");
        }
        Log.d(TAG, "eglContextSet: DONE");
    }

    private static int[] getEglConfigAttr(EGLDisplay eglDisplay, EGLContext eglContext) {
        int[] keys = {EGL14.EGL_CONFIG_ID};
        int[] configAttributes = new int[keys.length * 2 + 1];

        for (int i = 0; i < keys.length; i++) {
            configAttributes[i * 2] = keys[i];
            if (!EGL14.eglQueryContext(eglDisplay, eglContext, keys[i], configAttributes, i * 2 + 1)) {
                throw new RuntimeException("eglQueryContext failed: 0x" + Integer.toHexString(EGL14.eglGetError()));
            }
        }

        configAttributes[configAttributes.length - 1] = EGL14.EGL_NONE;
        return configAttributes;
    }
}
