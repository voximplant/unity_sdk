//
// Created by Aleksey Zinchenko on 07/03/2017.
//

#include "BaseImports.h"

#include "BaseVideoRenderer.h"

bool testGLErrors(const char *checkpoint) {
    GLenum error;
    bool anyError = false;
    while ((error = glGetError()) != GL_NO_ERROR) {
        vxeprintf("Error at %s with %d", checkpoint, error);
        anyError = true;
    }
    return anyError;
}

BaseVideoRenderer::BaseVideoRenderer(int width, int height):
        m_ackWidth(width), m_ackHeight(height),

        m_isInvalidated(false),
        m_textureIds(new GLuint[3]),

        m_fbo(0),
        m_FBOtexture(0)
{ }

BaseVideoRenderer::~BaseVideoRenderer() {
    delete [] m_textureIds;
}

void BaseVideoRenderer::Invalidate() {
    m_isInvalidated = true;
}

bool BaseVideoRenderer::IsValidForSize(int width, int height) {
    return !m_isInvalidated
           && IsActiveContextMatch()
           && m_ackWidth == width
           && m_ackHeight == height;
}

static const char s_vertexShader[] =
                "attribute vec4 aPosition;\n"
                "attribute vec2 aTextureCoord;\n"
                "varying vec2 vTextureCoord;\n"
                "void main() {\n"
                "  gl_Position = aPosition;\n"
                "  vTextureCoord = aTextureCoord;\n"
                "}\n";

static const char s_bufferFragmentShader[] =
                "precision mediump float;\n"
                "uniform sampler2D Ytex;\n"
                "uniform sampler2D Utex,Vtex;\n"
                "varying vec2 vTextureCoord;\n"
                "void main(void) {\n"
                "  float nx,ny,r,g,b,y,u,v;\n"
                "  mediump vec4 txl,ux,vx;"
                "  nx=vTextureCoord[0];\n"
                "  ny=vTextureCoord[1];\n"
                "  y=texture2D(Ytex,vec2(nx,ny)).r;\n"
                "  u=texture2D(Utex,vec2(nx,ny)).r;\n"
                "  v=texture2D(Vtex,vec2(nx,ny)).r;\n"

                "  y=1.1643*(y-0.0625);\n"
                "  u=u-0.5;\n"
                "  v=v-0.5;\n"

                "  r=y+1.5958*v;\n"
                "  g=y-0.39173*u-0.81290*v;\n"
                "  b=y+2.017*u;\n"
                "  gl_FragColor=vec4(r,g,b,1.0);\n"
                "}\n";

static const char s_indices[] = { 0, 3, 2, 0, 2, 1 };

static const GLfloat s_vertices[] = {
    // X, Y, Z
    -1, -1, 0, // Bottom Left
    1,  -1, 0, //Bottom Right
    1, 1, 0, //Top Right
    -1, 1, 0 //Top Left
};

static const GLfloat s_uvCoordinates[] = {
    1, 1,
    0, 1,
    0, 0,
    1, 0,
};

GLuint BaseVideoRenderer::LoadShader(GLenum shaderType, const char* source) {
    GLuint shader = glCreateShader(shaderType);
    if (shader) {
        glShaderSource(shader, 1, &source, NULL);
        glCompileShader(shader);
        GLint compiled = 0;
        glGetShaderiv(shader, GL_COMPILE_STATUS, &compiled);
        if (!compiled) {
            GLint infoLen = 0;
            glGetShaderiv(shader, GL_INFO_LOG_LENGTH, &infoLen);
            if (infoLen) {
                char* buf = (char*) malloc((size_t) infoLen);
                if (buf) {
                    glGetShaderInfoLog(shader, infoLen, NULL, buf);
                    vxeprintf("Could not compile shader %d: %s", shaderType, buf);
                    free(buf);
                }
                glDeleteShader(shader);
                shader = 0;
            }
        }
    }
    return shader;
}

void BaseVideoRenderer::LoadProgram() {
    GLuint vertexShader = LoadShader(GL_VERTEX_SHADER, s_vertexShader);
    if (!vertexShader) {
        return;
    }

    GLuint pixelShader = LoadShader(GL_FRAGMENT_SHADER, s_bufferFragmentShader);
    if (!pixelShader) {
        return;
    }

    GLuint program = glCreateProgram();
    if (program) {
        glAttachShader(program, vertexShader);
        testGLErrors("glAttachShader");
        glAttachShader(program, pixelShader);
        testGLErrors("glAttachShader");
        glLinkProgram(program);
        GLint linkStatus = GL_FALSE;
        glGetProgramiv(program, GL_LINK_STATUS, &linkStatus);
        if (linkStatus != GL_TRUE) {
            GLint bufLength = 0;
            glGetProgramiv(program, GL_INFO_LOG_LENGTH, &bufLength);
            if (bufLength) {
                char* buf = (char*) malloc((size_t) bufLength);
                if (buf) {
                    glGetProgramInfoLog(program, bufLength, NULL, buf);
                    vxeprintf("Could not link program: %s", buf);
                    free(buf);
                }
            }
            glDeleteProgram(program);
            program = 0;
        }
    }

    m_bufferProgram = program;
}

static void initializePlaneTexture(GLenum name, GLuint id, int width, int height) {
    glActiveTexture(name);
    glBindTexture(GL_TEXTURE_2D, id);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
    glTexImage2D(GL_TEXTURE_2D, 0, GL_LUMINANCE, width, height, 0,
                 GL_LUMINANCE, GL_UNSIGNED_BYTE, NULL);
}

void BaseVideoRenderer::SetupRender() {
    glGenFramebuffers(1, &m_fbo);
    testGLErrors("glGenFramebuffers");

    glGenTextures(1, &m_FBOtexture);
    glBindTexture(GL_TEXTURE_2D, m_FBOtexture);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, m_ackWidth, m_ackHeight, 0, GL_RGBA, GL_UNSIGNED_BYTE, 0);
    testGLErrors("glTexImage2D");

    glBindFramebuffer(GL_FRAMEBUFFER, m_fbo);
    glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, m_FBOtexture, 0);
    testGLErrors("glFramebufferTexture2D");

    GLenum status = glCheckFramebufferStatus(GL_FRAMEBUFFER);
    if (status != GL_FRAMEBUFFER_COMPLETE) {
        vxeprintf("Error: Failed to build complete FBO: \n%x\n", status);
        CleanupRender();
        return;
    }

    LoadProgram();

    if (m_bufferProgram == 0) {
        vxeprintf("Failed to load buffer program");
        return;
    }

    glGenTextures(3, m_textureIds); //Generate  the Y, U and V texture
    initializePlaneTexture(GL_TEXTURE0, m_textureIds[0], m_ackWidth, m_ackHeight);
    initializePlaneTexture(GL_TEXTURE1, m_textureIds[1], m_ackWidth / 2, m_ackHeight / 2);
    initializePlaneTexture(GL_TEXTURE2, m_textureIds[2], m_ackWidth / 2, m_ackHeight / 2);
    testGLErrors("InitializeTextures");

    GLint positionHandle = glGetAttribLocation(m_bufferProgram, "aPosition");
    testGLErrors("glGetAttribLocation aPosition");
    if (positionHandle == -1) {
        vxeprintf("Could not get aPosition handle");
        return;
    }

    glVertexAttribPointer((GLuint) positionHandle, 3, GL_FLOAT, GL_FALSE, 0, s_vertices);
    testGLErrors("glVertexAttribPointer positionHandle");
    glEnableVertexAttribArray((GLuint) positionHandle);
    testGLErrors("glEnableVertexAttribArray positionHandle");

    RotateByDegrees(0);

    glUseProgram(m_bufferProgram);
    int i = glGetUniformLocation(m_bufferProgram, "Ytex");
    testGLErrors("glGetUniformLocation");
    glUniform1i(i, 0); /* Bind Ytex to texture unit 0 */
    testGLErrors("glUniform1i Ytex");

    i = glGetUniformLocation(m_bufferProgram, "Utex");
    testGLErrors("glGetUniformLocation Utex");
    glUniform1i(i, 1); /* Bind Utex to texture unit 1 */
    testGLErrors("glUniform1i Utex");

    i = glGetUniformLocation(m_bufferProgram, "Vtex");
    testGLErrors("glGetUniformLocation");
    glUniform1i(i, 2); /* Bind Vtex to texture unit 2 */
    testGLErrors("glUniform1i");
    
    glViewport(0, 0, m_ackWidth, m_ackHeight);
    
    glClearColor(1, 0, 0, 1);
    glClear(GL_COLOR_BUFFER_BIT);
    
    testGLErrors("glViewport");
}

void shr(GLfloat source[], int length, int distance, GLfloat destination[]) {
    for (int i = 0; i < length; i++) {
        destination[(distance+i) % length] = source[i];
    }
}

static GLfloat rotatedUV[8];

void BaseVideoRenderer::RotateByDegrees(int degrees) {
    if (degrees % 90 != 0) {
        vxeprintf("Only multiples of 90 are supported for rotations");
        degrees = 0;
    }

    GLint textureHandle = glGetAttribLocation(m_bufferProgram, "aTextureCoord");
    testGLErrors("glGetAttribLocation aTextureCoord");
    if (textureHandle == -1) {
        vxeprintf("Could not get aTextureCoord handle");
        return;
    }

    shr((GLfloat *) s_uvCoordinates, 8, 2 * (degrees / 90), rotatedUV);

    glVertexAttribPointer((GLuint) textureHandle, 2, GL_FLOAT, GL_FALSE, 0, rotatedUV);
    testGLErrors("glVertexAttribPointer aTextureCoord");
    glEnableVertexAttribArray((GLuint) textureHandle);
    testGLErrors("glEnableVertexAttribArray aTextureCoord");
}

void BaseVideoRenderer::RenderBuffer(const uint8_t *yPlane, int yStride,
                                    const uint8_t *uPlane, int uStride,
                                    const uint8_t *vPlane, int vStride,
                                    int width, int height,
                                    int degrees) {
    if (width != m_ackWidth || height != m_ackHeight) {
        return;
    }

    glUseProgram(m_bufferProgram);
    testGLErrors("glUseProgram");

    RotateByDegrees(degrees);
    testGLErrors("RotateByDegrees");

    glBindFramebuffer(GL_FRAMEBUFFER, m_fbo);
    GLenum status = glCheckFramebufferStatus(GL_FRAMEBUFFER);
    if (status != GL_FRAMEBUFFER_COMPLETE) {
        testGLErrors("glCheckFramebufferStatus");
        vxvprintf("Incomplete FBO!");
        return;
    }

    UploadTextures(yPlane, yStride, uPlane, uStride, vPlane, vStride);

    glClearColor(1, 0, 0, 1);
    glClear(GL_COLOR_BUFFER_BIT);
    
    glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_BYTE, s_indices);
    testGLErrors("glDrawArrays");

    glFinish();
    testGLErrors("Rendering finished");
}

static void uploadPlane(GLsizei width, GLsizei height, int stride, const uint8_t *plane) {
    if (stride == width) {
        // Yay!  We can upload the entire plane in a single GL call.
        glTexSubImage2D(GL_TEXTURE_2D, 0, 0, 0, width, height, GL_LUMINANCE,
                        GL_UNSIGNED_BYTE,
                        static_cast<const GLvoid*>(plane));
    } else {
        // Boo!  Since GLES2 doesn't have GL_UNPACK_ROW_LENGTH and Android doesn't
        // have GL_EXT_unpack_subimage we have to upload a row at a time.  Ick.
        for (int row = 0; row < height; ++row) {
            glTexSubImage2D(GL_TEXTURE_2D, 0, 0, row, width, 1, GL_LUMINANCE,
                            GL_UNSIGNED_BYTE,
                            static_cast<const GLvoid*>(plane + (row * stride)));
        }
    }
}

void BaseVideoRenderer::UploadTextures(const uint8_t *yPlane, int yStride,
                                      const uint8_t *uPlane, int uStride,
                                      const uint8_t *vPlane, int vStride
) {
    glActiveTexture(GL_TEXTURE0);
    glBindTexture(GL_TEXTURE_2D, m_textureIds[0]);
    uploadPlane(m_ackWidth, m_ackHeight, yStride, yPlane);

    glActiveTexture(GL_TEXTURE1);
    glBindTexture(GL_TEXTURE_2D, m_textureIds[1]);
    uploadPlane(m_ackWidth / 2, m_ackHeight / 2, uStride, uPlane);

    glActiveTexture(GL_TEXTURE2);
    glBindTexture(GL_TEXTURE_2D, m_textureIds[2]);
    uploadPlane(m_ackWidth / 2, m_ackHeight / 2, vStride, vPlane);

    testGLErrors("UploadTextures");
}

GLuint BaseVideoRenderer::GetTargetTextureId() {
    return m_FBOtexture;
}
