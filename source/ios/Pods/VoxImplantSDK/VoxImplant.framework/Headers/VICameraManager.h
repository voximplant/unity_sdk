//
//  VICameraManager.h
//  VoxImplant
//
//  Created by Andrey Syvrachev (asyvrachev@zingaya.com) on 04.04.17.
//  Copyright © 2017 VoxImplant (www.voximplant.com). All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreVideo/CoreVideo.h>
#import "VIVideoSource.h"

/**
@protocol VIVideoPreprocessDelegate
*/
@protocol VIVideoPreprocessDelegate <NSObject>

@optional
/**
Triggered when new video frame is available for preprocessing.
@method preprocessVideoFrame:rotation:
@param {CVPixelBufferRef} pixelBuffer Video frame
@param {VIRotation} rotation Video frame rotation
*/
- (void)preprocessVideoFrame:(CVPixelBufferRef)pixelBuffer rotation:(VIRotation)rotation;

@end

/**
@class VICameraManager
*/
@interface VICameraManager : VIVideoSource

- (instancetype)init NS_UNAVAILABLE;

/**
Obtain VICameraManager instance
@method sharedCameraManager
@static
@return {instancetype} VICameraManager instance
*/
+ (instancetype)sharedCameraManager;

/**
Video prepocessing delegate
@property videoPreprocessDelegate
@type {id<VIVideoPreprocessDelegate>}
*/
@property (nonatomic,weak) id<VIVideoPreprocessDelegate> videoPreprocessDelegate;

/**
A boolean value indicating if back camera should be used
@property useBackCamera
@type {BOOL}
*/
@property (nonatomic,assign) BOOL useBackCamera;

@end
