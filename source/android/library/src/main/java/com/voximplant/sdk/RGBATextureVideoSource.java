package com.voximplant.sdk;

import com.voximplant.sdk.call.ICustomVideoSource;

/**
 * Created by zintus on 15/05/2017.
 */

class RGBATextureVideoSource {

    private int _textureId;
    private int _width;
    private int _height;
    private ICustomVideoSource _videoSource;

    void SendFrame() {
        _videoSource.sendFrame(_textureId, _width, _height);
    }

    RGBATextureVideoSource(int textureId, int width, int height, ICustomVideoSource videoSource) {
        _textureId = textureId;
        _width = width;
        _height = height;
        _videoSource = videoSource;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;

        RGBATextureVideoSource that = (RGBATextureVideoSource) o;

        return _textureId == that._textureId;
    }

    @Override
    public int hashCode() {
        int result = (int) (_textureId ^ (_textureId >>> 32));
        return result;
    }

    @Override
    public String toString() {
        return "RGBATextureVideoSource{" +
                "_textureId=" + _textureId +
                ", _width=" + _width +
                ", _height=" + _height +
                ", _videoSource=" + _videoSource +
                '}';
    }
}
