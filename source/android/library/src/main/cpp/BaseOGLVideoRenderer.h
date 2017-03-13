//
// Created by Aleksey Zinchenko on 07/03/2017.
//

#ifndef VOX_BASEOGLVIDEORENDERER_H
#define VOX_BASEOGLVIDEORENDERER_H

#include "BaseImports.h"
#include "BaseVideoRenderer.h"

bool testGLErrors(const char *checkpoint);

class BaseOGLVideoRenderer: public BaseVideoRenderer {
public:
    BaseOGLVideoRenderer(int width, int height);
    virtual ~BaseOGLVideoRenderer();

    void RenderBuffer(const uint8_t *yPlane, int yStride,
                      const uint8_t *uPlane, int uStride,
                      const uint8_t *vPlane, int vStride,
                      int width, int height,
                      int degrees);

    virtual void Detach() = 0;

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

private:
    GLuint *m_textureIds;
    GLuint m_bufferProgram;
};


#endif
