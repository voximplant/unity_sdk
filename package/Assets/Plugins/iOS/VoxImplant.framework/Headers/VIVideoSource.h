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
Base class for all video sources
*/
@interface VIVideoSource: NSObject
@end


@interface VIVideoFormat: NSObject

/**
Frame size
*/
@property(nonatomic,assign,readonly) CGSize frame;
@property(nonatomic,assign,readonly) NSUInteger interval;

- (instancetype)init NS_UNAVAILABLE;
- (instancetype)initWithFrame:(CGSize)frame fps:(NSUInteger)fps;
- (instancetype)initWithFrame:(CGSize)frame interval:(NSUInteger)interval;

@end

@protocol VICustomVideoSourceDelegate <NSObject>

- (void)startWithVideoFormat:(VIVideoFormat*)format;
- (void)stop;

@end


@interface VICustomVideoSource: VIVideoSource

@property(nonatomic,weak) id<VICustomVideoSourceDelegate> delegate;

- (instancetype)init NS_UNAVAILABLE;

- (instancetype)initWithVideoFormats:(NSArray<VIVideoFormat*>*)formats;

// kCVPixelFormatType_420YpCbCr8BiPlanarFullRange
// kCVPixelFormatType_420YpCbCr8BiPlanarVideoRange
- (void)sendVideoFrame:(CVPixelBufferRef)buffer rotation:(VIRotation)rotation;


@end
