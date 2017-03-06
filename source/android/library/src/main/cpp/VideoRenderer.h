//
// Created by Aleksey Zinchenko on 03/03/2017.
//

#ifndef VOXDEMO_VIDEOSTREAMRENDERER_H
#define VOXDEMO_VIDEOSTREAMRENDERER_H

#include <EGL/egl.h>
#include <GLES2/gl2.h>

#include "LockGuard.hpp"

#define avprintf(...) __android_log_print(ANDROID_LOG_VERBOSE, "VOXIMPLANT", __VA_ARGS__);
#define aeprintf(...) __android_log_print(ANDROID_LOG_ERROR, "VOXIMPLANT", __VA_ARGS__);

class VideoRenderer {
public:
    VideoRenderer(int width, int height, EGLContext sharedContext);
    ~VideoRenderer();

    void RenderBuffer(uint8_t *yPlane, int yStride,
                      uint8_t *uPlane, int uStride,
                      uint8_t *vPlane, int vStride,
                      int width, int height,
                      int degrees);
    void Detach();
    void Invalidate();

    bool IsValidForSize(int width, int height);

    GLuint GetTargetTextureId();
    EGLContext GetOGLContext();

private:
    int m_ackWidth, m_ackHeight;

    GLuint m_fbo;
    GLuint m_FBOtexture;
    EGLDisplay m_display;
    EGLSurface m_surface;
    EGLContext m_context;
    uint8_t *m_resultingPixels;
    bool m_isInvalidated;

    GLuint *m_textureIds;
    GLuint m_bufferProgram;

    EGLContext m_sharingContext;

    void EnsureOGL();
    void AssertOGLThread();
    void CleanupOGL();

    bool ChooseConfigAndCreateContext(EGLDisplay display);
    GLuint LoadShader(GLenum shaderType, const char* source);
    GLuint LoadProgram(const char *vertexShader, const char *fragmentShader);

    void SetupBufferProgram();
    void RotateByDegrees(int degrees);

    void UploadTextures(uint8_t *yPlane, int yStride,
                        uint8_t *uPlane, int uStride,
                        uint8_t *vPlane, int vStride);
};

#endif //VOXDEMO_VIDEOSTREAMRENDERER_H
