//
// Created by Aleksey Zinchenko on 07/03/2017.
//

#ifndef VOX_BASEVIDEORENDERER_H
#define VOX_BASEVIDEORENDERER_H

#include <cstdint>

#include "BaseImports.h"

bool testGLErrors(const char *checkpoint);

class BaseVideoRenderer {
public:
    BaseVideoRenderer(int width, int height);
    virtual ~BaseVideoRenderer();

    virtual bool IsActiveContextMatch() = 0;
    virtual void Detach() = 0;
    void Invalidate();

    bool IsValidForSize(int width, int height);
    void RenderBuffer(const uint8_t *yPlane, int yStride,
                      const uint8_t *uPlane, int uStride,
                      const uint8_t *vPlane, int vStride,
                      int width, int height,
                      int degrees);

    GLuint GetTargetTextureId();

protected:
    virtual void SetupRender();

    GLuint LoadShader(GLenum shaderType, const char* source);
    void LoadProgram();

    void RotateByDegrees(int degrees);

    void UploadTextures(const uint8_t *yPlane, int yStride,
                        const uint8_t *uPlane, int uStride,
                        const uint8_t *vPlane, int vStride
    );

    virtual void CleanupRender() = 0;

    GLuint m_fbo;
    GLuint m_FBOtexture;
    int m_ackWidth, m_ackHeight;

private:
    bool m_isInvalidated;

    GLuint *m_textureIds;
    GLuint m_bufferProgram;
};


#endif //VOX_BASEVIDEORENDERER_H
