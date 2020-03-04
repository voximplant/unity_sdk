/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

#import "VIOpenGLRenderer.h"
#include <OpenGLES/ES2/glext.h>

@interface VIRenderer (OpenGL)

+ (EAGLContext *)unityContext;

@end

@interface VIOpenGLRenderer ()

@property(assign, nonatomic) CGSize frameSize;

@end

@implementation VIOpenGLRenderer {
    EAGLSharegroup *_sharegroup;
    EAGLContext *_context;

    GLuint *_textureIds;
    GLuint _bufferProgram;
    GLuint _fbo;
    GLuint _fboTexture;
}

- (instancetype)init {
    self = [super init];

    if (self) {
        _textureIds = new GLuint[3];
        _fbo = 0;
        _fboTexture = 0;
        _bufferProgram = 0;

        _sharegroup = [VIRenderer unityContext].sharegroup;

        _context = [[EAGLContext alloc] initWithAPI:kEAGLRenderingAPIOpenGLES3 sharegroup:_sharegroup];
        if (!_context) {
            _context = [[EAGLContext alloc] initWithAPI:kEAGLRenderingAPIOpenGLES2 sharegroup:_sharegroup];
        }

        [EAGLContext setCurrentContext:_context];
    }

    return self;
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

static const GLfloat s_vertices[] = {
        // X, Y, Z
        -1, -1, 0, // Bottom Left
        1, -1, 0,  // Bottom Right
        1, 1, 0,   // Top Right
        -1, 1, 0   // Top Left
};

static const GLfloat s_uvCoordinates[] = {
        1, 1,
        0, 1,
        0, 0,
        1, 0,
};

bool testGLErrors(const char *checkpoint) {
    NSLog(@"Passing: %s", checkpoint);
    GLenum error;
    bool anyError = false;
    while ((error = glGetError()) != GL_NO_ERROR) {
        NSLog(@"Error at %s with %d", checkpoint, error);
        anyError = true;
    }

    return anyError;
}


- (GLuint)loadShader:(GLenum)shaderType source:(const char *)source {
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
                char *buf = (char *) malloc((size_t) infoLen);
                if (buf) {
                    glGetShaderInfoLog(shader, infoLen, NULL, buf);
                    NSLog(@"Could not compile shader %d: %s", shaderType, buf);
                    free(buf);
                }
                glDeleteShader(shader);
                shader = 0;
            }
        }
    }
    return shader;
}

- (void)loadProgram {
    GLuint vertexShader = [self loadShader:GL_VERTEX_SHADER source:s_vertexShader];
    if (!vertexShader) {
        return;
    }

    GLuint pixelShader = [self loadShader:GL_FRAGMENT_SHADER source:s_bufferFragmentShader];
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
                char *buf = (char *) malloc((size_t) bufLength);
                if (buf) {
                    glGetProgramInfoLog(program, bufLength, NULL, buf);
                    NSLog(@"Could not link program: %s", buf);
                    free(buf);
                }
            }
            glDeleteProgram(program);
            program = 0;
        }
    }

    _bufferProgram = program;
}

static void initializePlaneTexture(GLenum name, GLuint id, int width, int height) {
    glActiveTexture(name);
    glBindTexture(GL_TEXTURE_2D, id);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
    glTexImage2D(GL_TEXTURE_2D, 0, GL_LUMINANCE, width, height, 0, GL_LUMINANCE, GL_UNSIGNED_BYTE, NULL);
}

- (void)setupRenderer:(CGSize)frameSize {
    if (CGSizeEqualToSize(frameSize, self.frameSize)) return;

    _frameSize = frameSize;

    glGenFramebuffers(1, &_fbo);
    testGLErrors("glGenFramebuffers");

    glGenTextures(1, &_fboTexture);
    glBindTexture(GL_TEXTURE_2D, _fboTexture);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
    glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, static_cast<GLsizei>(_frameSize.width), static_cast<GLsizei>(_frameSize.height), 0, GL_RGBA, GL_UNSIGNED_BYTE, 0);
    testGLErrors("glTexImage2D");

    glBindFramebuffer(GL_FRAMEBUFFER, _fbo);
    glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, _fboTexture, 0);
    testGLErrors("glFramebufferTexture2D");

    GLenum status = glCheckFramebufferStatus(GL_FRAMEBUFFER);
    if (status != GL_FRAMEBUFFER_COMPLETE) {
        NSLog(@"Error: Failed to build complete FBO: \n%x\n", status);
        return;
    }

    [self loadProgram];

    if (_bufferProgram == 0) {
        NSLog(@"Failed to load buffer program");
        return;
    }

    glGenTextures(3, _textureIds); // Generate  the Y, U and V texture
    initializePlaneTexture(GL_TEXTURE0, _textureIds[0], static_cast<GLsizei>(_frameSize.width), static_cast<GLsizei>(_frameSize.height));
    initializePlaneTexture(GL_TEXTURE1, _textureIds[1], static_cast<GLsizei>(_frameSize.width / 2), static_cast<GLsizei>(_frameSize.height / 2));
    initializePlaneTexture(GL_TEXTURE2, _textureIds[2], static_cast<GLsizei>(_frameSize.width / 2), static_cast<GLsizei>(_frameSize.height / 2));
    testGLErrors("InitializeTextures");

    GLint positionHandle = glGetAttribLocation(_bufferProgram, "aPosition");
    testGLErrors("glGetAttribLocation aPosition");
    if (positionHandle == -1) {
        NSLog(@"Could not get aPosition handle");
        return;
    }

    glVertexAttribPointer((GLuint) positionHandle, 3, GL_FLOAT, GL_FALSE, 0, s_vertices);
    testGLErrors("glVertexAttribPointer positionHandle");
    glEnableVertexAttribArray((GLuint) positionHandle);
    testGLErrors("glEnableVertexAttribArray positionHandle");

    [self rotateByDegrees:0];

    glUseProgram(_bufferProgram);
    int i = glGetUniformLocation(_bufferProgram, "Ytex");
    testGLErrors("glGetUniformLocation");
    glUniform1i(i, 0); /* Bind Ytex to texture unit 0 */
    testGLErrors("glUniform1i Ytex");

    i = glGetUniformLocation(_bufferProgram, "Utex");
    testGLErrors("glGetUniformLocation Utex");
    glUniform1i(i, 1); /* Bind Utex to texture unit 1 */
    testGLErrors("glUniform1i Utex");

    i = glGetUniformLocation(_bufferProgram, "Vtex");
    testGLErrors("glGetUniformLocation");
    glUniform1i(i, 2); /* Bind Vtex to texture unit 2 */
    testGLErrors("glUniform1i");

    glViewport(0, 0, static_cast<GLsizei>(_frameSize.width), static_cast<GLsizei>(_frameSize.height));

    glClearColor(1, 0, 0, 1);
    glClear(GL_COLOR_BUFFER_BIT);

    testGLErrors("glViewport");
}

void shrGLfloat(float *source, int length, int distance, float *destination) {
    while (distance < 0) {
        distance += length;
    }
    for (int i = 0; i < length; i++) {
        destination[(distance + i) % length] = source[i];
    }
}

- (void)rotateByDegrees:(int)degrees {
    if (degrees % 90 != 0) {
        NSLog(@"Only multiples of 90 are supported for rotations");
        degrees = 0;
    }

    GLint textureHandle = glGetAttribLocation(_bufferProgram, "aTextureCoord");
    testGLErrors("glGetAttribLocation aTextureCoord");
    if (textureHandle == -1) {
        NSLog(@"Could not get aTextureCoord handle");
        return;
    }

    GLfloat rotatedUV[8];
    shrGLfloat((GLfloat *) s_uvCoordinates, 8, 2 * (degrees / 90), rotatedUV);

    glVertexAttribPointer((GLuint) textureHandle, 2, GL_FLOAT, GL_FALSE, 0, rotatedUV);
    testGLErrors("glVertexAttribPointer aTextureCoord");
    glEnableVertexAttribArray((GLuint) textureHandle);
    testGLErrors("glEnableVertexAttribArray aTextureCoord");
}

static void uploadPlane(GLsizei width, GLsizei height, int stride, const uint8_t *plane) {
    glPixelStorei(GL_UNPACK_ALIGNMENT, 1);
    NSLog(@"uploadPlane: %dx%d stride %d %p", width, height, stride, plane);
    if (stride == width) {
        // Yay!  We can upload the entire plane in a single GL createCall.
        glTexSubImage2D(GL_TEXTURE_2D, 0, 0, 0, width, height, GL_LUMINANCE,
                GL_UNSIGNED_BYTE,
                static_cast<const GLvoid *>(plane));
    } else {
        // Boo!  Since GLES2 doesn't have GL_UNPACK_ROW_LENGTH and Android doesn't
        // have GL_EXT_unpack_subimage we have to upload a row at a time.  Ick.
        for (int row = 0; row < height; ++row) {
            glTexSubImage2D(GL_TEXTURE_2D, 0, 0, row, width, 1, GL_LUMINANCE,
                    GL_UNSIGNED_BYTE,
                    static_cast<const GLvoid *>(plane + (row * stride)));
        }
    }
}

- (void)uploadTextures:(id<RTCI420Buffer>)buffer {
    glActiveTexture(GL_TEXTURE0);
    glBindTexture(GL_TEXTURE_2D, _textureIds[0]);
    testGLErrors("glBindTexture");
    uploadPlane(buffer.width, buffer.height, buffer.strideY, buffer.dataY);

    glActiveTexture(GL_TEXTURE1);
    glBindTexture(GL_TEXTURE_2D, _textureIds[1]);
    testGLErrors("glBindTexture");
    uploadPlane(buffer.width / 2, buffer.height / 2, buffer.strideU, buffer.dataU);

    glActiveTexture(GL_TEXTURE2);
    glBindTexture(GL_TEXTURE_2D, _textureIds[2]);
    testGLErrors("glBindTexture");
    uploadPlane(buffer.width / 2, buffer.height / 2, buffer.strideV, buffer.dataV);

    testGLErrors("UploadTextures");
}

- (void)renderBuffer:(id <RTCI420Buffer>)buffer rotation:(RTCVideoRotation)rotation {
    if (buffer == nil || buffer.width != _frameSize.width || buffer.height != _frameSize.height) {
        return;
    }

    glUseProgram(_bufferProgram);
    testGLErrors("glUseProgram");

    [self rotateByDegrees:rotation];
    testGLErrors("RotateByDegrees");

    glBindFramebuffer(GL_FRAMEBUFFER, _fbo);
    GLenum status = glCheckFramebufferStatus(GL_FRAMEBUFFER);
    if (status != GL_FRAMEBUFFER_COMPLETE) {
        testGLErrors("glCheckFramebufferStatus");
        NSLog(@"Incomplete FBO!");
        return;
    }

    [self uploadTextures:buffer];

    glClearColor(1, 0, 0, 1);
    glClear(GL_COLOR_BUFFER_BIT);

    const char s_indices[] = {0, 3, 2, 0, 2, 1};
    glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_BYTE, s_indices);
    testGLErrors("glDrawArrays");

    glFlush();
    testGLErrors("Rendering finished");
}

- (long long)texture {
    return _fboTexture;
}

@end
