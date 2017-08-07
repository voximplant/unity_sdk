package com.voximplant.sdk;

import android.annotation.SuppressLint;
import android.opengl.EGL14;
import android.util.Log;

import com.voximplant.sdk.hardware.ICustomVideoSource;
import com.voximplant.sdk.hardware.ICustomVideoSourceListener;

import org.webrtc.RendererCommon;

import java.nio.ByteBuffer;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;

import javax.microedition.khronos.egl.EGL10;
import javax.microedition.khronos.egl.EGLContext;

class RGBATextureVideoSource {

    private String TAG = "RGBATextureVideoSource";

    private int _width;
    private int _height;
    private ICustomVideoSource _videoSource;
    private boolean _isStarted;

    private ByteBuffer _convertBuffer;
    private byte[] _outputBuffer;
    private YuvConverter _converter;

    private ExecutorService _executor;
    private EglBase _eglBase;

    private List<Future<?>> _futuresQueue = new ArrayList<>();
    private int _queueLength = 2;

    @Override
    protected void finalize() throws Throwable {
        super.finalize();

        _converter.release();
    }

    private void EnsureConvertUtils() {
        int yuvSize = 3 * _width * _height / 2;
        if (_convertBuffer == null
                || _convertBuffer.capacity() != yuvSize) {
            _convertBuffer = ByteBuffer.allocateDirect(yuvSize);
        }
        if (_converter == null) {
            _converter = new YuvConverter();
        }
        if (_outputBuffer == null
                || _outputBuffer.length != yuvSize) {
            _outputBuffer = new byte[yuvSize];
        }
    }

    void SendFrame(final int textureId) throws Exception {
        if (!_isStarted) {
            return;
        }

        if (_executor == null) {
            throw new Exception("Failed to create ogl context");
        }

        while (_futuresQueue.size() >= _queueLength) {
            Future<?> future = _futuresQueue.get(0);
            future.get();
            _futuresQueue.remove(0);
        }

        Future<?> sendFrame = _executor.submit(new Runnable() {
            @Override
            public void run() {
                EnsureConvertUtils();

                _convertBuffer.position(0);
                _converter.convert(_convertBuffer, _width, _height, _width, textureId, RendererCommon.identityMatrix());

                _convertBuffer.position(0);
                { // Y
                    _convertBuffer.get(_outputBuffer, 0, _width * _height);
                }

                { // UV plane
                    int base = _width * _height;
                    int uvWidth = _width / 2;

                    for (int row = 0; row < _height / 2; row++) { // U
                        _convertBuffer.position(_width * _height + row * _width);
                        _convertBuffer.get(_outputBuffer, base, uvWidth);
                        base += uvWidth;
                    }

                    for (int row = 0; row < _height / 2; row++) { // V
                        _convertBuffer.position(_width * _height + row * _width + uvWidth);
                        _convertBuffer.get(_outputBuffer, base, uvWidth);
                        base += uvWidth;
                    }
                }

                long start = System.currentTimeMillis();
                _videoSource.sendFrame(_outputBuffer, _width, _height);
                long time = System.currentTimeMillis() - start;
                Log.v(TAG, "Spent " + time + "ms in SendFrame");
            }
        });

        _futuresQueue.add(sendFrame);
    }

    @SuppressLint("NewApi")
    RGBATextureVideoSource(int width, int height, ICustomVideoSource videoSource) {
        _width = width;
        _height = height;
        _videoSource = videoSource;
        _videoSource.setCustomVideoSourceListener(new ICustomVideoSourceListener() {
            @Override
            public void onStarted() {
                _isStarted = true;
            }

            @Override
            public void onStopped() {
                _isStarted = false;
            }
        });

        _executor = Executors.newSingleThreadExecutor();

        final EglBase.Context currentContext;
        if (EglBase14.isEGL14Supported()) {
            currentContext = new EglBase14.Context(EGL14.eglGetCurrentContext());
        } else {
            EGL10 egl = (EGL10) EGLContext.getEGL();
            currentContext = new EglBase10.Context(egl.eglGetCurrentContext());
        }

        _executor.submit(new Runnable() {
            @Override
            public void run() {
                try {
                    _eglBase = EglBase.createAndMakeCurrent(currentContext, EglBase.CONFIG_PIXEL_BUFFER);

                    EnsureConvertUtils();
                } catch (RuntimeException exception) {
                    _executor = null;

                    Log.e(TAG, _eglBase.toString() + " " + exception.getLocalizedMessage());
                    throw exception;
                }
            }
        });
    }

    @Override
    public String toString() {
        return "RGBATextureVideoSource{" +
                ", _width=" + _width +
                ", _height=" + _height +
                ", _videoSource=" + _videoSource +
                '}';
    }
}
