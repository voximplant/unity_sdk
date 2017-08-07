//
// Created by Aleksey Zinchenko on 10/05/2017.
// Copyright (c) 2017 voximplant. All rights reserved.
//

#import <VoxImplant/VIVideoSource.h>

#import "VIEAGLVideoStreamSource.h"

#import "libyuv.h"
#import "iOSNativeRenderer.h"
#import "VIBlocksThread.h"

@interface VIEAGLVideoStreamSource ()

@property(nonatomic, assign, readonly) GLuint fbo;
@property(nonatomic, assign, readwrite) BOOL initialzedFBO;
@property(nonatomic, strong, readonly) NSMutableData *textureBuffer;
@property(nonatomic, strong, readonly) NSMutableData *mirrorBuffer;

@property(nonatomic, strong, readonly) VIBlocksThread *thread;
@property(nonatomic, strong, readwrite) EAGLContext *glContext;

@property(nonatomic, assign, readwrite) CVPixelBufferRef buffer;

@end

@implementation VIEAGLVideoStreamSource

- (instancetype)initWithWidth:(NSUInteger)width height:(NSUInteger)height {
    self = [super initWithWidth:width height:height];
    if (self == nil) {
        return self;
    }

    _thread = [[VIBlocksThread alloc] initWithQueueLimit:2];

    return self;
}

#pragma mark - VIBaseUnityVideoStreamSource

- (void)sendVideoFrameFromTexture:(intptr_t)texturePtr width:(NSUInteger)width height:(NSUInteger)height {
    if (!self.isRunning) {
        return;
    }

    if (self.buffer == nil
            || CVPixelBufferGetWidth(self.buffer) != width
            || CVPixelBufferGetHeight(self.buffer) != height) {
        [self.thread enqueueBlock:^{
            _textureBuffer = [NSMutableData dataWithLength:4 * width * height];
            _mirrorBuffer = [NSMutableData dataWithLength:4 * width * height];

            if (self.buffer != nil) {
                CVPixelBufferRelease(self.buffer);
            }

            if (CVPixelBufferCreate(NULL, width, height, kCVPixelFormatType_420YpCbCr8BiPlanarFullRange, NULL, &_buffer) != kCVReturnSuccess) {
                @throw [NSException exceptionWithName:@"Internal inconsistency"
                                               reason:nil
                                             userInfo:nil];
            };
        }];
    }

    [self.thread enqueueBlock:^{
        [self ensureContext];

        GLuint currentFBO;
        glGetIntegerv(GL_FRAMEBUFFER_BINDING, (GLint *) &currentFBO);

        GLenum status = glCheckFramebufferStatus(GL_FRAMEBUFFER);
        if(status != GL_FRAMEBUFFER_COMPLETE) {
            if (self.initialzedFBO) {
                glDeleteFramebuffers(1, &_fbo);
                self.initialzedFBO = NO;
            }
        }

        if (!self.initialzedFBO) {
            glGenFramebuffers(1, &_fbo);

            self.initialzedFBO = YES;
        }

        glBindFramebuffer(GL_FRAMEBUFFER, self.fbo);

        glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, (GLuint) texturePtr, 0);

        status = glCheckFramebufferStatus(GL_FRAMEBUFFER);
        if(status != GL_FRAMEBUFFER_COMPLETE) {
            NSLog(@"failed to make complete framebuffer for local video stream %x", status);
        }

        glReadPixels(0, 0, width, height, GL_RGBA, GL_UNSIGNED_BYTE, self.textureBuffer.mutableBytes);

        CVPixelBufferLockBaseAddress(self.buffer, NULL);
        libyuv::ARGBMirror(
                (const uint8 *) self.textureBuffer.mutableBytes, 4 * width,
                (uint8 *) self.mirrorBuffer.mutableBytes, 4 * width,
                width, height);
        libyuv::ARGBRotate(
                (const uint8 *) self.mirrorBuffer.mutableBytes, 4 * width,
                (uint8 *) self.mirrorBuffer.mutableBytes, 4 * width,
                width, height, libyuv::kRotate180);
        libyuv::ARGBToNV21((const uint8 *) self.mirrorBuffer.mutableBytes, 4 * width,
                (uint8 *) CVPixelBufferGetBaseAddressOfPlane(self.buffer, 0), (int) CVPixelBufferGetBytesPerRowOfPlane(self.buffer, 0),
                (uint8 *) CVPixelBufferGetBaseAddressOfPlane(self.buffer, 1), (int) CVPixelBufferGetBytesPerRowOfPlane(self.buffer, 1),
                width, height);
        CVPixelBufferUnlockBaseAddress(self.buffer, NULL);

        [self.videoSource sendVideoFrame:self.buffer
                                rotation:VIRotation_0];

        glBindFramebuffer(GL_FRAMEBUFFER, currentFBO);
    }];
}

- (void)ensureContext {
    if (self.glContext != nil) {
        return;
    }

    self.glContext = [[EAGLContext alloc] initWithAPI:kEAGLRenderingAPIOpenGLES3
                                           sharegroup:s_unityContext.sharegroup];
    if (self.glContext == nil) {
        self.glContext = [[EAGLContext alloc] initWithAPI:kEAGLRenderingAPIOpenGLES2
                                               sharegroup:s_unityContext.sharegroup];
    }

    [EAGLContext setCurrentContext:self.glContext];

    if (self.glContext == nil) {
        @throw [NSException exceptionWithName:@"Internal inconsistency"
                                       reason:nil
                                     userInfo:nil];
    }
}

#pragma mark - dealloc

- (void)dealloc {
    if (self.initialzedFBO) {
        glDeleteFramebuffers(1, &_fbo);
    }
}

@end
