//
//  VICameraManager.h
//  VoxImplant
//
//  Created by Andrey Syvrachev (asyvrachev@zingaya.com) on 04.04.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreVideo/CoreVideo.h>
#import "VIVideoSource.h"

@protocol VIVideoPreprocessDelegate <NSObject>

@optional
/**
 Triggered when new video frame is available for preprocessing.
 
 @param pixelBuffer Video frame
 @param rotation    <VIRotation> video frame rotation
 */
- (void)preprocessVideoFrame:(CVPixelBufferRef)pixelBuffer rotation:(VIRotation)rotation;

@end

/** VICameraManager */
@interface VICameraManager : VIVideoSource

/**
 @warning NS_UNAVAILABLE
 */
- (instancetype)init NS_UNAVAILABLE;

/**
 Obtain VICameraManager instance
 
 @return VICameraManager instance
 */
+ (instancetype)sharedCameraManager;

/**
 Video prepocessing delegate
 */
@property (nonatomic,weak) id<VIVideoPreprocessDelegate> videoPreprocessDelegate;

/**
 A boolean value indicating if back camera should be used
 */
@property (nonatomic,assign) BOOL useBackCamera;

@end
