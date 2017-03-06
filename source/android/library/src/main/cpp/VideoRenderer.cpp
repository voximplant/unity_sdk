//
// Created by Aleksey Zinchenko on 03/03/2017.
//

#include <EGL/egl.h>
#include <stdio.h>
#include <cstdlib>
#include <android/log.h>

#include "VideoRenderer.h"

VideoRenderer::VideoRenderer(int width, int height, EGLContext sharedContext) :
        m_ackWidth(width),
        m_ackHeight(height),

        m_fbo(0),
        m_FBOtexture(0),
        m_display(0),
        m_context(EGL_NO_CONTEXT),
        m_resultingPixels(new uint8_t[width * height * 4]),

        m_textureIds(new GLuint[3]),

        m_sharingContext(sharedContext)
        { }

bool testGLErrors(const char *checkpoint) {
    GLenum error;
    bool anyError = false;
    while ((error = glGetError()) != GL_NO_ERROR) {
        aeprintf("Error at %s with %d", checkpoint, error);
        anyError = true;
    }
    return anyError;
}

bool VideoRenderer::ChooseConfigAndCreateContext(EGLDisplay display) {
    EGLint numConfigs = 0;

    if (!eglGetConfigs(display, NULL, 0, &numConfigs) || numConfigs == 0) {
        return false;
    }

    EGLConfig *configs = new EGLConfig[numConfigs];

    eglGetConfigs(display, configs, numConfigs, &numConfigs);

    for (int i = 0; i < numConfigs; ++i) {
        EGLint surfaceType;
        EGLint renderableType;
        EGLint redSize;
        EGLint greenSize;
        EGLint blueSize;

        eglGetConfigAttrib(display, configs[i], EGL_SURFACE_TYPE, &surfaceType);
        eglGetConfigAttrib(display, configs[i], EGL_RENDERABLE_TYPE, &renderableType);
        eglGetConfigAttrib(display, configs[i], EGL_RED_SIZE, &redSize);
        eglGetConfigAttrib(display, configs[i], EGL_GREEN_SIZE, &greenSize);
        eglGetConfigAttrib(display, configs[i], EGL_BLUE_SIZE, &blueSize);

        int neededSurfType = EGL_PBUFFER_BIT;
        if ((surfaceType & neededSurfType) != neededSurfType)
            continue;

        if ((renderableType & EGL_OPENGL_ES2_BIT) != EGL_OPENGL_ES2_BIT)
            continue;

        if (redSize < 8 || greenSize < 8 || blueSize < 8)
            continue;

        do {
            EGLint ctxAttr[] = {
                    EGL_CONTEXT_CLIENT_VERSION, 3,
                    EGL_NONE
            };

            m_context = eglCreateContext(m_display, configs[i], m_sharingContext, ctxAttr);
            if (!testGLErrors("eglCreateContext") && m_context != EGL_NO_CONTEXT) {
                break;
            }

            // No luck with 3.0, let's try 2.0
            ctxAttr[1] = 2;
            m_context = eglCreateContext(m_display, configs[i], m_sharingContext, ctxAttr);
        } while(false);

        if (testGLErrors("eglCreateContext") || m_context == EGL_NO_CONTEXT) {
            m_context = EGL_NO_CONTEXT;
            continue;
        }

        const EGLint surfaceAttr[] = {
                EGL_WIDTH, m_ackWidth,
                EGL_HEIGHT, m_ackHeight,
                EGL_NONE
        };

        m_surface = eglCreatePbufferSurface(m_display, configs[i], surfaceAttr);
        if (testGLErrors("eglCreatePbufferSurface")) {
            m_context = EGL_NO_CONTEXT;
            m_surface = EGL_NO_SURFACE;
            continue;
        }

        if (eglMakeCurrent(m_display, m_surface, m_surface, m_context) == EGL_FALSE) {
            m_context = EGL_NO_CONTEXT;
            m_surface = EGL_NO_SURFACE;
            continue;
        }
        if (testGLErrors("eglMakeCurrent")) {
            m_context = EGL_NO_CONTEXT;
            m_surface = EGL_NO_SURFACE;
            continue;
        }

        delete[] configs;
        return true;
    }

    delete[] configs;
    return false;
}


void VideoRenderer::AssertOGLThread() {
    if (m_context != EGL_NO_CONTEXT && eglGetCurrentContext() != m_context) {
        aeprintf("Calling thread changed since creation of OGL");
        __builtin_trap();
    }
}

static const char s_indices[] = { 0, 3, 2, 0, 2, 1 };

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

static const GLfloat s_vertices[] = {
        // X, Y, Z
        -1, -1, 0, // Bottom Left
        1, -1, 0, //Bottom Right
        1, 1, 0, //Top Right
        -1, 1, 0}; //Top Left

static const GLfloat s_uvCoordinates[] = {
        0, 1,
        1, 1,
        1, 0,
        0, 0
};

GLuint VideoRenderer::LoadShader(GLenum shaderType, const char* source) {
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
                char* buf = (char*) malloc(infoLen);
                if (buf) {
                    glGetShaderInfoLog(shader, infoLen, NULL, buf);
                    aeprintf("Could not compile shader %d: %s", shaderType, buf);
                    free(buf);
                }
                glDeleteShader(shader);
                shader = 0;
            }
        }
    }
    return shader;
}

GLuint VideoRenderer::LoadProgram(const char* pVertexShader, const char* pFragmentShader) {
    GLuint vertexShader = LoadShader(GL_VERTEX_SHADER, pVertexShader);
    if (!vertexShader) {
        return 0;
    }

    GLuint pixelShader = LoadShader(GL_FRAGMENT_SHADER, pFragmentShader);
    if (!pixelShader) {
        return 0;
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
                char* buf = (char*) malloc(bufLength);
                if (buf) {
                    glGetProgramInfoLog(program, bufLength, NULL, buf);
                    aeprintf("Could not link program: %s", buf);
                    free(buf);
                }
            }
            glDeleteProgram(program);
            program = 0;
        }
    }
    return program;
}

void VideoRenderer::SetupBufferProgram() {
    GLint positionHandle = glGetAttribLocation(m_bufferProgram, "aPosition");
    testGLErrors("glGetAttribLocation aPosition");
    if (positionHandle == -1) {
        aeprintf("Could not get aPosition handle");
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
}

void shr(GLfloat source[], int length, int distance, GLfloat destination[]) {
    for (int i = 0; i < length; i++) {
        destination[distance+i % length] = source[i];
    }
}

static GLfloat rotatedUV[8];

void VideoRenderer::RotateByDegrees(int degrees) {
    if (degrees % 90 != 0) {
        aeprintf("Only multiples of 90 are supported for rotations");
        degrees = 0;
    }

    GLint textureHandle = glGetAttribLocation(m_bufferProgram, "aTextureCoord");
    testGLErrors("glGetAttribLocation aTextureCoord");
    if (textureHandle == -1) {
        aeprintf("Could not get aTextureCoord handle");
        return;
    }

    shr((GLfloat *) s_uvCoordinates, 8, 2 * (degrees / 90), rotatedUV);

    glVertexAttribPointer((GLuint) textureHandle, 2, GL_FLOAT, GL_FALSE, 0, rotatedUV);
    testGLErrors("glVertexAttribPointer aTextureCoord");
    glEnableVertexAttribArray((GLuint) textureHandle);
    testGLErrors("glEnableVertexAttribArray aTextureCoord");
}

static void initializePlaneTexture(int name, int id, int width, int height) {
    glActiveTexture(name);
    glBindTexture(GL_TEXTURE_2D, id);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
    glTexImage2D(GL_TEXTURE_2D, 0, GL_LUMINANCE, width, height, 0,
                 GL_LUMINANCE, GL_UNSIGNED_BYTE, NULL);
}

void VideoRenderer::EnsureOGL() {
    if (m_context != 0) {
        AssertOGLThread();
        return;
    }

    if (eglGetCurrentContext() != EGL_NO_CONTEXT) {
        aeprintf("Error: trying to setup context with already present OGL context");
        return;
    }

    m_display = eglGetDisplay(EGL_DEFAULT_DISPLAY);
    EGLint major, minor;
    eglInitialize(m_display, &major, &minor);
    eglBindAPI(EGL_OPENGL_ES_API);
    testGLErrors("eglBindAPI");

    if (!ChooseConfigAndCreateContext(m_display)) {
        testGLErrors("chooseConfigAndCreateContext");
        CleanupOGL();
        return;
    }

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
        aeprintf("Error: Failed to build complete FBO: \n%x\n", status);
        CleanupOGL();
        return;
    }

    if ((m_bufferProgram = LoadProgram(s_vertexShader, s_bufferFragmentShader)) == 0) {
        aeprintf("Failed to load buffer program");
        CleanupOGL();
        return;
    }

    glGenTextures(3, m_textureIds); //Generate  the Y, U and V texture
    initializePlaneTexture(GL_TEXTURE0, m_textureIds[0], m_ackWidth, m_ackHeight);
    initializePlaneTexture(GL_TEXTURE1, m_textureIds[1], m_ackWidth / 2, m_ackHeight / 2);
    initializePlaneTexture(GL_TEXTURE2, m_textureIds[2], m_ackWidth / 2, m_ackHeight / 2);
    testGLErrors("InitializeTextures");

    SetupBufferProgram();

    glViewport(0, 0, m_ackWidth, m_ackHeight);

    glClearColor(1, 0, 0, 1);
    glClear(GL_COLOR_BUFFER_BIT);

    testGLErrors("glViewport");
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

void VideoRenderer::UploadTextures(uint8_t *yPlane, int yStride,
                                   uint8_t *uPlane, int uStride,
                                   uint8_t *vPlane, int vStride
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

void VideoRenderer::RenderBuffer(uint8_t *yPlane, int yStride,
                                 uint8_t *uPlane, int uStride,
                                 uint8_t *vPlane, int vStride,
                                 int width, int height,
                                 int degrees) {
    if (width != m_ackWidth || height != m_ackHeight) {
        return;
    }

    EnsureOGL();

    glUseProgram(m_bufferProgram);
    testGLErrors("glUseProgram");

    RotateByDegrees(degrees);
    testGLErrors("RotateByDegrees");

    glBindFramebuffer(GL_FRAMEBUFFER, m_fbo);
    GLenum status = glCheckFramebufferStatus(GL_FRAMEBUFFER);
    if (status != GL_FRAMEBUFFER_COMPLETE) {
        testGLErrors("glCheckFramebufferStatus");
        avprintf("Incomplete FBO!");
        return;
    }

    UploadTextures(yPlane, yStride, uPlane, uStride, vPlane, vStride);

    glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_BYTE, s_indices);
    testGLErrors("glDrawArrays");

    glFlush();
    testGLErrors("Rendering finished");
}

VideoRenderer::~VideoRenderer() {
    delete [] m_textureIds;
    delete[] m_resultingPixels;

    CleanupOGL();
}

void VideoRenderer::CleanupOGL() {
    if (m_context != EGL_NO_CONTEXT) {
        eglDestroyContext(m_display, m_context);
    }
    if (m_surface != EGL_NO_SURFACE) {
        eglDestroySurface(m_display, m_surface);
    }
    eglTerminate(m_display);

    m_display = 0;
    m_context = EGL_NO_CONTEXT;
    m_surface = EGL_NO_SURFACE;
}

bool VideoRenderer::IsValidForSize(int width, int height) {
    return !m_isInvalidated && m_ackWidth == width && m_ackHeight == height;
}

GLuint VideoRenderer::GetTargetTextureId() {
    return m_FBOtexture;
}

EGLContext VideoRenderer::GetOGLContext() {
    return m_context;
}

void VideoRenderer::Detach() {
    eglMakeCurrent(m_display, EGL_NO_SURFACE, EGL_NO_SURFACE, EGL_NO_CONTEXT);
}

void VideoRenderer::Invalidate() {
    m_isInvalidated = true;
}