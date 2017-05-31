//
// Created by Aleksey Zinchenko on 14/05/2017.
// Copyright (c) 2017 voximplant. All rights reserved.
//

#import <VoxImplant/VIVideoSource.h>
#import "VIMetalVideoStreamSource.h"

#import "libyuv.h"
#import "iOSNativeRenderer.h"
#import "IUnityGraphicsMetal.h"

@interface VIMetalVideoStreamSource ()

@property(nonatomic, strong, readonly) NSMutableData *textureBuffer;

@property(nonatomic, assign, readonly) CVPixelBufferRef pixelBuffer;

@end

@implementation VIMetalVideoStreamSource

- (instancetype)initWidth:(NSUInteger)width height:(NSUInteger)height {
    self = [super initWithWidth:width height:height];
    if (self == nil) {
        return self;
    }

    return self;
}

- (void)ensureBuffersWithWidth:(NSUInteger)width height:(NSUInteger)height {
    _textureBuffer = [NSMutableData dataWithLength:4 * width * height];

    if (self.pixelBuffer != nil) {
        CVPixelBufferRelease(self.pixelBuffer);
    }

    if (CVPixelBufferCreate(NULL, width, height, kCVPixelFormatType_420YpCbCr8BiPlanarFullRange, NULL, &_pixelBuffer) != kCVReturnSuccess) {
        @throw [NSException exceptionWithName:@"Internal inconsistency"
                                       reason:nil
                                     userInfo:nil];
    }
}

- (void)dealloc {
    CVPixelBufferRelease(_pixelBuffer);
}

#pragma mark - VIBaseVideoStreamSource

- (void)sendVideoFrameFromTexture:(intptr_t)texturePtr width:(NSUInteger)width height:(NSUInteger)height {
    if (!self.isRunning) {
        return;
    }

    id <MTLTexture> texture = (__bridge id) ((void *)texturePtr);
    NSParameterAssert(texture.pixelFormat == MTLPixelFormatRGBA8Unorm);

    IUnityGraphicsMetal *metalGraphics = s_unityInterfaces->Get<IUnityGraphicsMetal>();

    [metalGraphics->CurrentCommandBuffer() addCompletedHandler:^(id <MTLCommandBuffer> o) {
        [self ensureBuffersWithWidth:width height:height];

        [texture getBytes:self.textureBuffer.mutableBytes
              bytesPerRow:4 * width
               fromRegion:MTLRegionMake2D(0, 0, width, height)
              mipmapLevel:0];

        CVPixelBufferRef buffer = self.pixelBuffer;

        CVPixelBufferLockBaseAddress(buffer, NULL);
        libyuv::ARGBToNV21((const uint8 *) self.textureBuffer.mutableBytes, 4 * width,
                (uint8 *) CVPixelBufferGetBaseAddressOfPlane(buffer, 0), (int) CVPixelBufferGetBytesPerRowOfPlane(buffer, 0),
                (uint8 *) CVPixelBufferGetBaseAddressOfPlane(buffer, 1), (int) CVPixelBufferGetBytesPerRowOfPlane(buffer, 1),
                width, height);
        CVPixelBufferUnlockBaseAddress(buffer, NULL);

        [self.videoSource sendVideoFrame:buffer rotation:VIRotation_180];
    }];
}

@end
