//
//  VICameraManager.h
//  VoxImplant
//
//  Created by Andrey Syvrachev (asyvrachev@zingaya.com) on 04.04.17.
//  Copyright © 2017 VoxImplant (www.voximplant.com). All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreVideo/CoreVideo.h>

typedef NS_ENUM(NSInteger,VIRotation) {
    VIRotation_0 = 0,
    VIRotation_90 = 90,
    VIRotation_180 = 180,
    VIRotation_270 = 270
};

@protocol VIVideoPreprocessDelegate <NSObject>

@optional

- (void)preprocessVideoFrame:(CVPixelBufferRef)pixelBuffer rotation:(VIRotation)rotation;

@end


@interface VICameraManager : NSObject

- (instancetype)init NS_UNAVAILABLE;

+ (instancetype)sharedCameraManager;

@property (nonatomic,weak) id<VIVideoPreprocessDelegate> videoPreprocessDelegate;

@property (nonatomic,assign) BOOL useBackCamera;

@end
