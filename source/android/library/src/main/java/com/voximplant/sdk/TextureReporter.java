package com.voximplant.sdk;

/**
 * Created by zintus on 06/09/2017.
 */

public class TextureReporter {
    private String callId;
    private int stream;

    private boolean reportedTextureId = true;
    private int textureId;

    private int width;
    private int height;

    public TextureReporter(String callId, int stream) {
        this.callId = callId;
        this.stream = stream;
    }

    public void setTextureId(int textureId) {
        if (textureId != this.textureId) {
            this.textureId = textureId;
            this.reportedTextureId = false;
        }

        tryToReportTexture();
    }

    public void setRect(int width, int height) {
        this.width = width;
        this.height = height;

        tryToReportTexture();
    }

    private void tryToReportTexture() {
        if (width > 0 && height > 0 && !reportedTextureId) {
            listener.onTexture(callId, textureId, width, height, stream);
            reportedTextureId = true;
        }
    }

    public TextureListener listener;

    public interface TextureListener {
        void onTexture(String callId, long textureId, int width, int height, int stream);
    }
}
