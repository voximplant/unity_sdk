package com.voximplant.sdk;

import android.annotation.SuppressLint;
import android.bluetooth.BluetoothClass;
import android.opengl.GLES20;
import android.opengl.GLES30;
import android.os.Build;
import android.util.Log;

import org.webrtc.GlShader;
import org.webrtc.GlTextureFrameBuffer;
import org.webrtc.GlUtil;
import org.webrtc.RendererCommon;
import org.webrtc.ThreadUtils;

import java.nio.ByteBuffer;
import java.nio.FloatBuffer;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import static android.opengl.GLES10.GL_VERSION;
import static android.opengl.GLES20.GL_ARRAY_BUFFER;

/**
 * Created by zintus on 18/05/2017.
 */

class YuvConverter {
    // Vertex coordinates in Normalized Device Coordinates, i.e.
    // (-1, -1) is bottom-left and (1, 1) is top-right.
    private static final FloatBuffer DEVICE_RECTANGLE = GlUtil.createFloatBuffer(new float[]{
            -1.0f, -1.0f, // Bottom left.
            1.0f, -1.0f, // Bottom right.
            -1.0f, 1.0f, // Top left.
            1.0f, 1.0f, // Top right.
    });

    // Texture coordinates - (0, 0) is bottom-left and (1, 1) is top-right.
    private static final FloatBuffer TEXTURE_RECTANGLE = GlUtil.createFloatBuffer(new float[]{
            0.0f, 0.0f, // Bottom left.
            1.0f, 0.0f, // Bottom right.
            0.0f, 1.0f, // Top left.
            1.0f, 1.0f // Top right.
    });

    // clang-format off
    private static final String VERTEX_SHADER =
            "varying vec2 interp_tc;\n"
                    + "attribute vec4 in_pos;\n"
                    + "attribute vec4 in_tc;\n"
                    + "\n"
                    + "uniform mat4 texMatrix;\n"
                    + "\n"
                    + "void main() {\n"
                    + "    gl_Position = in_pos;\n"
                    + "    interp_tc = (texMatrix * in_tc).xy;\n"
                    + "}\n";

    private static final String FRAGMENT_SHADER =
            "precision mediump float;\n"
                    + "varying vec2 interp_tc;\n"
                    + "\n"
                    + "uniform sampler2D oesTex;\n"
                    // Difference in texture coordinate corresponding to one
                    // sub-pixel in the x direction.
                    + "uniform vec2 xUnit;\n"
                    // Color conversion coefficients, including constant term
                    + "uniform vec4 coeffs;\n"
                    + "\n"
                    + "void main() {\n"
                    // Since the alpha read from the texture is always 1, this could
                    // be written as a mat4 x vec4 multiply. However, that seems to
                    // give a worse framerate, possibly because the additional
                    // multiplies by 1.0 consume resources. TODO(nisse): Could also
                    // try to do it as a vec3 x mat3x4, followed by an add in of a
                    // constant vector.
                    + "  gl_FragColor.r = coeffs.a + dot(coeffs.rgb,\n"
                    + "      texture2D(oesTex, interp_tc - 1.5 * xUnit).rgb);\n"
                    + "  gl_FragColor.g = coeffs.a + dot(coeffs.rgb,\n"
                    + "      texture2D(oesTex, interp_tc - 0.5 * xUnit).rgb);\n"
                    + "  gl_FragColor.b = coeffs.a + dot(coeffs.rgb,\n"
                    + "      texture2D(oesTex, interp_tc + 0.5 * xUnit).rgb);\n"
                    + "  gl_FragColor.a = coeffs.a + dot(coeffs.rgb,\n"
                    + "      texture2D(oesTex, interp_tc + 1.5 * xUnit).rgb);\n"
                    + "}\n";
    // clang-format on

    private final GlTextureFrameBuffer textureFrameBuffer;
    private final GlShader shader;
    private final int texMatrixLoc;
    private final int xUnitLoc;
    private final int coeffsLoc;
    private final ThreadUtils.ThreadChecker threadChecker = new ThreadUtils.ThreadChecker();
    private boolean released = false;

    private final int majorVersion;

    /**
     * This class should be constructed on a thread that has an active EGL context.
     */
    public YuvConverter() {
        threadChecker.checkIsOnValidThread();
        textureFrameBuffer = new GlTextureFrameBuffer(GLES20.GL_RGBA);

        Matcher versionMatcher = Pattern.compile("\\s(\\d)\\.\\d\\s").matcher(GLES20.glGetString(GL_VERSION));
        if (versionMatcher.find()) {
            majorVersion = Integer.parseInt(versionMatcher.group(1));
        } else {
            throw new IllegalStateException("YuvConverter failed to parse GL version");
        }

        shader = new GlShader(VERTEX_SHADER, FRAGMENT_SHADER);
        shader.useProgram();
        texMatrixLoc = shader.getUniformLocation("texMatrix");
        xUnitLoc = shader.getUniformLocation("xUnit");
        coeffsLoc = shader.getUniformLocation("coeffs");
        GlUtil.checkNoGLES2Error("Initialize fragment shader uniform values.");
    }

    public static int BufferCapacityForFrame(int width, int height, int stride) {
        int uv_height = (height + 1) / 2;
        int total_height = height + uv_height;
        return stride * total_height;
    }

    @SuppressLint("NewApi")
    public void convert(ByteBuffer buf, int width, int height, int stride, int srcTextureId, float[] transformMatrix) {
        threadChecker.checkIsOnValidThread();
        if (released) {
            throw new IllegalStateException("YuvConverter.convert called on released object");
        }

        // We draw into a buffer laid out like
        //
        //    +---------+
        //    |         |
        //    |  Y      |
        //    |         |
        //    |         |
        //    +----+----+
        //    | U  | V  |
        //    |    |    |
        //    +----+----+
        //
        // In memory, we use the same stride for all of Y, U and V. The
        // U data starts at offset |height| * |stride| from the Y data,
        // and the V data starts at at offset |stride/2| from the U
        // data, with rows of U and V data alternating.
        //
        // Now, it would have made sense to allocate a pixel buffer with
        // a single byte per pixel (EGL10.EGL_COLOR_BUFFER_TYPE,
        // EGL10.EGL_LUMINANCE_BUFFER,), but that seems to be
        // unsupported by devices. So do the following hack: Allocate an
        // RGBA buffer, of width |stride|/4. To render each of these
        // large pixels, sample the texture at 4 different x coordinates
        // and store the results in the four components.
        //
        // Since the V data needs to start on a boundary of such a
        // larger pixel, it is not sufficient that |stride| is even, it
        // has to be a multiple of 8 pixels.

        if (stride % 8 != 0) {
            throw new IllegalArgumentException("Invalid stride, must be a multiple of 8");
        }
        if (stride < width) {
            throw new IllegalArgumentException("Invalid stride, must >= width");
        }

        int y_width = (width + 3) / 4;
        int uv_width = (width + 7) / 8;
        int uv_height = (height + 1) / 2;
        int total_height = height + uv_height;
        int size = stride * total_height;

        Log.v("YuvConverter", "size is "+size+" capacity is "+buf.capacity());
        if (buf.capacity() < size) {
            throw new IllegalArgumentException("YuvConverter.convert called with too small buffer");
        }

        shader.useProgram();

        if (majorVersion == 3) {
            GLES30.glBindVertexArray(0);
        }

        GLES20.glBindBuffer(GL_ARRAY_BUFFER, 0);
        GlUtil.checkNoGLES2Error("YuvConverter.unbind buffers");

        shader.setVertexAttribArray("in_pos", 2, DEVICE_RECTANGLE);
        shader.setVertexAttribArray("in_tc", 2, TEXTURE_RECTANGLE);

        // Produce a frame buffer starting at top-left corner, not
        // bottom-left.
        transformMatrix = RendererCommon.multiplyMatrices(transformMatrix, RendererCommon.verticalFlipMatrix());

        final int frameBufferWidth = stride / 4;
        final int frameBufferHeight = total_height;
        textureFrameBuffer.setSize(frameBufferWidth, frameBufferHeight);

        // Bind our framebuffer.
        GLES20.glBindFramebuffer(GLES20.GL_FRAMEBUFFER, textureFrameBuffer.getFrameBufferId());
        GlUtil.checkNoGLES2Error("glBindFramebuffer");

        GLES20.glActiveTexture(GLES20.GL_TEXTURE0);
        GLES20.glUniform1i(shader.getUniformLocation("oesTex"), 0);
        GLES20.glBindTexture(GLES20.GL_TEXTURE_2D, srcTextureId);
        GlUtil.checkNoGLES2Error("YuvConverter.active texture 0");

        GLES20.glUniformMatrix4fv(texMatrixLoc, 1, false, transformMatrix, 0);
        GlUtil.checkNoGLES2Error("YuvConverter.uniform matrix4f");

        GLES20.glClearColor(0.1f, 0.1f, 0.1f, 0.1f);
        GLES20.glClear(GLES20.GL_COLOR_BUFFER_BIT | GLES20.GL_DEPTH_BUFFER_BIT);

        // Draw Y
        GLES20.glViewport(0, 0, y_width, height);
        // Matrix * (1;0;0;0) / width. Note that opengl uses column major order.
        GLES20.glUniform2f(xUnitLoc, transformMatrix[0] / width, transformMatrix[1] / width);
        // Y'UV444 to RGB888, see
        // https://en.wikipedia.org/wiki/YUV#Y.27UV444_to_RGB888_conversion.
        // We use the ITU-R coefficients for U and V */
        GLES20.glUniform4f(coeffsLoc, 0.299f, 0.587f, 0.114f, 0.0f);
        GLES20.glDrawArrays(GLES20.GL_TRIANGLE_STRIP, 0, 4);

        GlUtil.checkNoGLES2Error("YuvConverter.draw y");

        // Draw U
        GLES20.glViewport(0, height, uv_width, uv_height);
        // Matrix * (1;0;0;0) / (width / 2). Note that opengl uses column major order.
        GLES20.glUniform2f(xUnitLoc, 2.0f * transformMatrix[0] / width, 2.0f * transformMatrix[1] / width);
        GLES20.glUniform4f(coeffsLoc, -0.169f, -0.331f, 0.499f, 0.5f);
        GLES20.glDrawArrays(GLES20.GL_TRIANGLE_STRIP, 0, 4);

        GlUtil.checkNoGLES2Error("YuvConverter.draw u");

        // Draw V
        GLES20.glViewport(stride / 8, height, uv_width, uv_height);
        GLES20.glUniform4f(coeffsLoc, 0.499f, -0.418f, -0.0813f, 0.5f);
        GLES20.glDrawArrays(GLES20.GL_TRIANGLE_STRIP, 0, 4);

        GlUtil.checkNoGLES2Error("YuvConverter.draw v");

        GLES20.glPixelStorei(GLES20.GL_PACK_ALIGNMENT, 1);
        GLES20.glReadPixels(0, 0, frameBufferWidth, frameBufferHeight, GLES20.GL_RGBA, GLES20.GL_UNSIGNED_BYTE, buf);

        GlUtil.checkNoGLES2Error("YuvConverter.convert");
    }

    public void release() {
        threadChecker.checkIsOnValidThread();
        released = true;
        shader.release();
        textureFrameBuffer.release();
    }
}