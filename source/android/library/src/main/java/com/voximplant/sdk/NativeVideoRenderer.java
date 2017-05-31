package com.voximplant.sdk;

import android.os.Looper;
import android.util.Log;

import org.webrtc.VideoRenderer;

import java.nio.ByteBuffer;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;

/**
 * Created by zintus on 03/03/2017.
 */

public class NativeVideoRenderer implements VideoRenderer.Callbacks {
    public interface NativeVideoRendererCallbacks {
        void onBufferFrameRender(ByteBuffer[] planes, int[] strides, int width, int height, int degrees);
    }

    private NativeVideoRendererCallbacks callbacks;
    private ExecutorService executor;

    NativeVideoRenderer(NativeVideoRendererCallbacks callbacks) {
        this.callbacks = callbacks;

        executor = Executors.newSingleThreadExecutor();
    }

    @Override
    public void renderFrame(final VideoRenderer.I420Frame i420Frame) {
        Runnable runnable = new Runnable() {
            @Override
            public void run() {
                if (i420Frame.yuvFrame) {
                    callbacks.onBufferFrameRender(i420Frame.yuvPlanes,
                            i420Frame.yuvStrides,
                            i420Frame.width,
                            i420Frame.height,
                            i420Frame.rotationDegree);
                } else {
                    Log.e("VOXIMPLANT", "not implemented texture based rendering");
                }

                VideoRenderer.renderFrameDone(i420Frame);
            }
        };

        executor.submit(runnable);
    }
}
