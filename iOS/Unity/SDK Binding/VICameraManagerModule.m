/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

#import "VICameraManagerModule.h"
#import <VoxImplant/VoxImplant.h>

@interface VICameraManagerModule ()

@property(nonatomic, assign) AVCaptureDevicePosition currentCamera;
@property(nonatomic, assign) CGSize currentResolution;

@end

@implementation VICameraManagerModule

+ (instancetype)sharedInstance {
    static VICameraManagerModule *instance;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        instance = [[VICameraManagerModule alloc] init];
    });
    return instance;
}

+ (AVCaptureDevicePosition)devicePositionForCamera:(VICameraType)cameraType {
    switch (cameraType) {
        case VICameraTypeBack:
            return AVCaptureDevicePositionBack;
        case VICameraTypeFront:
            return AVCaptureDevicePositionFront;
    }
}

- (instancetype)init {
    self = [super init];
    if (self) {
        _currentCamera = [VICameraManagerModule devicePositionForCamera:VICameraTypeFront];
        _currentResolution = CGSizeMake(640, 480);
    }
    return self;
}

- (void)switchCamera:(VICameraType)type {
    AVCaptureDevicePosition newCamera = [VICameraManagerModule devicePositionForCamera:type];
    if (_currentCamera == newCamera) return;
    _currentCamera = newCamera;

    switch (_currentCamera) {
        case AVCaptureDevicePositionBack:
            [VICameraManager sharedCameraManager].useBackCamera = true;
            break;
        case AVCaptureDevicePositionFront:
            [VICameraManager sharedCameraManager].useBackCamera = false;
            break;
        case AVCaptureDevicePositionUnspecified:
            break;
    }
}

- (void)setCameraResolution:(CGSize)size {
    if (CGSizeEqualToSize(_currentResolution, size)) return;
    _currentResolution = size;

    __block AVCaptureDevice *captureDevice = nil;
    [[VICameraManager sharedCameraManager].captureDevices enumerateObjectsUsingBlock:^(AVCaptureDevice *obj, NSUInteger idx, BOOL *stop) {
        if (obj.position == self.currentCamera) {
            captureDevice = obj;
            *stop = YES;
        }
    }];

    if (!captureDevice) return;

    int targetWidth = (int) _currentResolution.width;
    int targetHeight = (int) _currentResolution.height;
    AVCaptureDeviceFormat *selectedFormat = nil;
    int currentDiff = INT_MAX;

    for (AVCaptureDeviceFormat *format in [[VICameraManager sharedCameraManager] supportedFormatsForDevice:captureDevice]) {
        CMVideoDimensions dimension = CMVideoFormatDescriptionGetDimensions(format.formatDescription);
        int diff = abs(targetWidth - dimension.width) + abs(targetHeight - dimension.height);
        if (diff < currentDiff) {
            selectedFormat = format;
            currentDiff = diff;
        }
    }

    if (selectedFormat) {
        [[VICameraManager sharedCameraManager] changeCaptureFormat:selectedFormat];
    }
}
@end
