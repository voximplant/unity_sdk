//
//  VICameraManager.h
//  VoxImplant
//
//  Created by Andrey Syvrachev (asyvrachev@zingaya.com) on 04.04.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreVideo/CoreVideo.h>

/**
 Enum of supported video rotation modes
 */
typedef NS_ENUM(NSInteger,VIRotation) {
    /** No rotation */
    VIRotation_0 = 0,
    /** 90 degrees clockwise rotation */
    VIRotation_90 = 90,
    /** 180 degrees clockwise rotation */
    VIRotation_180 = 180,
    /** 270 degrees clockwise rotation */
    VIRotation_270 = 270
};

/**
 Base class for all video sources. See VICustomVideoSource and VICameraManager for more information.
 */
@interface VIVideoSource: NSObject
@end


/**
 Interface that represents video format
 */
@interface VIVideoFormat: NSObject

/**
 Frame size
 */
@property(nonatomic,assign,readonly) CGSize frame;

/**
 Time interval between frames in milliseconds
 */
@property(nonatomic,assign,readonly) NSUInteger interval;

/**
 @warning NS_UNAVAILABLE
 */
- (instancetype)init NS_UNAVAILABLE;

/**
 Initialize VIVideoFormat with frame size and fps

 @param frame Frame size
 @param fps   FPS
 @return VIVideoFormat instance
 */
- (instancetype)initWithFrame:(CGSize)frame fps:(NSUInteger)fps;

/**
 Initialize VIVideoFormat with frame size and interval

 @param frame     Frame size
 @param interval  Time interval between frames in milliseconds
 @return VIVideoFormat instance
 */
- (instancetype)initWithFrame:(CGSize)frame interval:(NSUInteger)interval;

@end


/**
 Delegate that may be used to handle custom video source events
 */
@protocol VICustomVideoSourceDelegate <NSObject>

/**
 Triggered when custom video source is started

 @param format Video format
 */
- (void)startWithVideoFormat:(VIVideoFormat*)format;

/**
 Triggered when custom video source is stopped
 */
- (void)stop;

@end


/**
 Interface that represents custom video source
 */
@interface VICustomVideoSource: VIVideoSource

/**
 <VICustomVideoSourceDelegate> delegate to handle custom video source events
 */
@property(nonatomic,weak) id<VICustomVideoSourceDelegate> delegate;

/**
 @warning NS_UNAVAILABLE
 */
- (instancetype)init NS_UNAVAILABLE;

/**
 Initialize custom video source

 @param formats Video format for the custom video source
 @return VICustomVideoSource instance
 */
- (instancetype)initWithVideoFormats:(NSArray<VIVideoFormat*>*)formats;

/**
 Send video frame

 @param buffer   Pixel buffer. Should be _kCVPixelFormatType_420YpCbCr8BiPlanarFullRange_ or _kCVPixelFormatType_420YpCbCr8BiPlanarVideoRange_
 @param rotation Video rotation
 */
- (void)sendVideoFrame:(CVPixelBufferRef)buffer rotation:(VIRotation)rotation;


@end
