//
//  VICameraManager.h
//  VoxImplant
//
//  Created by Andrey Syvrachev (asyvrachev@zingaya.com) on 04.04.17.
//  Copyright © 2017 VoxImplant (www.voximplant.com). All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreVideo/CoreVideo.h>

/**
 @enum VIRotation
 @contstant VIRotation_0
 @contstant VIRotation_90
 @contstant VIRotation_180
 @contstant VIRotation_270
 */
typedef NS_ENUM(NSInteger,VIRotation) {
    VIRotation_0 = 0,
    VIRotation_90 = 90,
    VIRotation_180 = 180,
    VIRotation_270 = 270
};

/**
 Base class for all video sources
 @class VICameraManager
 */
@interface  VIVideoSource: NSObject
@end


@interface VIVideoFormat: NSObject

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
