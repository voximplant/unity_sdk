/*
 *  Copyright 2015 The WebRTC project authors. All Rights Reserved.
 *
 *  Use of this source code is governed by a BSD-style license
 *  that can be found in the LICENSE file in the root of the source
 *  tree. An additional intellectual property rights grant can be found
 *  in the file PATENTS.  All contributing project authors may
 *  be found in the AUTHORS file in the root of the source tree.
 */

package com.voximplant.sdk.render;

import android.annotation.SuppressLint;
import android.graphics.SurfaceTexture;
import android.opengl.EGL14;
import android.view.Surface;

import javax.microedition.khronos.egl.EGL10;
import javax.microedition.khronos.egl.EGLContext;

/**
 * Holds EGL state and utility methods for handling an egl 1.0 EGLContext, an EGLDisplay,
 * and an EGLSurface.
 */
@SuppressWarnings("StaticOrDefaultInterfaceMethod")
public abstract class EglBase {
    // EGL wrapper for an actual EGLContext.
    public static class Context {
        @SuppressLint("NewApi")
        public static Context getCurrent() {
            if (EglBase14.isEGL14Supported()) {
                return new EglBase14.Context(EGL14.eglGetCurrentContext());
            } else {
                EGL10 egl = (EGL10) EGLContext.getEGL();
                return new EglBase10.Context(egl.eglGetCurrentContext());
            }
        }
    }

    // According to the documentation, EGL can be used from multiple threads at the same time if each
    // thread has its own EGLContext, but in practice it deadlocks on some devices when doing this.
    // Therefore, synchronize on this global lock before calling dangerous EGL functions that might
    // deadlock. See https://bugs.chromium.org/p/webrtc/issues/detail?id=5702 for more info.
    public static final Object lock = new Object();

    // These constants are taken from EGL14.EGL_OPENGL_ES2_BIT and EGL14.EGL_CONTEXT_CLIENT_VERSION.
    // https://android.googlesource.com/platform/frameworks/base/+/master/opengl/java/android/opengl/EGL14.java
    // This is similar to how GlSurfaceView does:
    // http://grepcode.com/file/repository.grepcode.com/java/ext/com.google.android/android/5.1.1_r1/android/opengl/GLSurfaceView.java#760
    public static final int EGL_OPENGL_ES2_BIT = 4;
    // Android-specific extension.
    public static final int EGL_RECORDABLE_ANDROID = 0x3142;

    // clang-format off
    public static final int[] CONFIG_PLAIN = {
            EGL10.EGL_RED_SIZE, 8,
            EGL10.EGL_GREEN_SIZE, 8,
            EGL10.EGL_BLUE_SIZE, 8,
            EGL10.EGL_RENDERABLE_TYPE, EGL_OPENGL_ES2_BIT,
            EGL10.EGL_NONE
    };
    public static final int[] CONFIG_RGBA = {
            EGL10.EGL_RED_SIZE, 8,
            EGL10.EGL_GREEN_SIZE, 8,
            EGL10.EGL_BLUE_SIZE, 8,
            EGL10.EGL_ALPHA_SIZE, 8,
            EGL10.EGL_RENDERABLE_TYPE, EGL_OPENGL_ES2_BIT,
            EGL10.EGL_NONE
    };
    public static final int[] CONFIG_PIXEL_BUFFER = {
            EGL10.EGL_RED_SIZE, 8,
            EGL10.EGL_GREEN_SIZE, 8,
            EGL10.EGL_BLUE_SIZE, 8,
            EGL10.EGL_RENDERABLE_TYPE, EGL_OPENGL_ES2_BIT,
            EGL10.EGL_SURFACE_TYPE, EGL10.EGL_PBUFFER_BIT,
            EGL10.EGL_NONE
    };
    public static final int[] CONFIG_PIXEL_RGBA_BUFFER = {
            EGL10.EGL_RED_SIZE, 8,
            EGL10.EGL_GREEN_SIZE, 8,
            EGL10.EGL_BLUE_SIZE, 8,
            EGL10.EGL_ALPHA_SIZE, 8,
            EGL10.EGL_RENDERABLE_TYPE, EGL_OPENGL_ES2_BIT,
            EGL10.EGL_SURFACE_TYPE, EGL10.EGL_PBUFFER_BIT,
            EGL10.EGL_NONE
    };
    public static final int[] CONFIG_RECORDABLE = {
            EGL10.EGL_RED_SIZE, 8,
            EGL10.EGL_GREEN_SIZE, 8,
            EGL10.EGL_BLUE_SIZE, 8,
            EGL10.EGL_RENDERABLE_TYPE, EGL_OPENGL_ES2_BIT,
            EGL_RECORDABLE_ANDROID, 1,
            EGL10.EGL_NONE
    };
    // clang-format on

    /**
     * Create a new context with the specified config attributes, sharing data with |sharedContext|.
     * If |sharedContext| is null, a root context is created. This function will try to create an EGL
     * 1.4 context if possible, and an EGL 1.0 context otherwise.
     */
    public static EglBase create(Context sharedContext, int[] configAttributes) {
        return (EglBase14.isEGL14Supported()
                && (sharedContext == null || sharedContext instanceof EglBase14.Context))
                ? new EglBase14((EglBase14.Context) sharedContext, configAttributes)
                : new EglBase10((EglBase10.Context) sharedContext, configAttributes);
    }

    public static EglBase createAdaptive(Context sharedContext, int[] configAttributes) {
        boolean egl14 = EglBase14.isEGL14Supported()
                && (sharedContext == null || sharedContext instanceof EglBase14.Context);
        if (egl14) {
            EglBase14 eglBase14;
            try {
                eglBase14 = new EglBase14((EglBase14.Context) sharedContext, configAttributes, 3);
            } catch (Exception ex) {
                eglBase14 = new EglBase14((EglBase14.Context) sharedContext, configAttributes, 2);
            }

            return eglBase14;
        } else {
            return new EglBase10((EglBase10.Context) sharedContext, configAttributes);
        }
    }

    public static EglBase createAdaptiveAndMakeCurrent(Context sharedContext, int[] configAttributes) {
        boolean egl14 = EglBase14.isEGL14Supported()
                && (sharedContext == null || sharedContext instanceof EglBase14.Context);
        if (egl14) {
            EglBase14 eglBase14;
            try {
                eglBase14 = new EglBase14((EglBase14.Context) sharedContext, configAttributes, 3);
                eglBase14.createDummyPbufferSurface();
                eglBase14.makeCurrent();
            } catch (Exception ex) {
                eglBase14 = new EglBase14((EglBase14.Context) sharedContext, configAttributes, 2);
                eglBase14.createDummyPbufferSurface();
                eglBase14.makeCurrent();
            }

            return eglBase14;
        } else {
            return new EglBase10((EglBase10.Context) sharedContext, configAttributes);
        }
    }


    /**
     * Helper function for creating a plain root context. This function will try to create an EGL 1.4
     * context if possible, and an EGL 1.0 context otherwise.
     */
    public static EglBase create() {
        return create(null /* shaderContext */, CONFIG_PLAIN);
    }

    /**
     * Helper function for creating a plain context, sharing data with |sharedContext|. This function
     * will try to create an EGL 1.4 context if possible, and an EGL 1.0 context otherwise.
     */
    public static EglBase create(Context sharedContext) {
        return create(sharedContext, CONFIG_PLAIN);
    }

    /**
     * Explicitly create a root EGl 1.0 context with the specified config attributes.
     */
    public static EglBase createEgl10(int[] configAttributes) {
        return new EglBase10(null /* shaderContext */, configAttributes);
    }

    /**
     * Explicitly create a root EGl 1.0 context with the specified config attributes
     * and shared context.
     */
    public static EglBase createEgl10(
            javax.microedition.khronos.egl.EGLContext sharedContext, int[] configAttributes) {
        return new EglBase10(new EglBase10.Context(sharedContext), configAttributes);
    }

    /**
     * Explicitly create a root EGl 1.4 context with the specified config attributes.
     */
    public static EglBase createEgl14(int[] configAttributes) {
        return new EglBase14(null /* shaderContext */, configAttributes);
    }

    /**
     * Explicitly create a root EGl 1.4 context with the specified config attributes
     * and shared context.
     */
    public static EglBase createEgl14(
            android.opengl.EGLContext sharedContext, int[] configAttributes) {
        return new EglBase14(new EglBase14.Context(sharedContext), configAttributes);
    }

    public abstract void createSurface(Surface surface);

    // Create EGLSurface from the Android SurfaceTexture.
    public abstract void createSurface(SurfaceTexture surfaceTexture);

    // Create dummy 1x1 pixel buffer surface so the context can be made current.
    public abstract void createDummyPbufferSurface();

    public abstract void createPbufferSurface(int width, int height);

    public abstract Context getEglBaseContext();

    public abstract boolean hasSurface();

    public abstract int surfaceWidth();

    public abstract int surfaceHeight();

    public abstract void releaseSurface();

    public abstract void release();

    public abstract void makeCurrent();

    // Detach the current EGL context, so that it can be made current on another thread.
    public abstract void detachCurrent();

    public abstract void swapBuffers();

    public abstract void swapBuffers(long presentationTimeStampNs);
}
