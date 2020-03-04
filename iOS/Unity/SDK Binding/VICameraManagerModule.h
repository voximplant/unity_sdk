/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

#import <Foundation/Foundation.h>
#import <CoreGraphics/CoreGraphics.h>

typedef NS_ENUM(NSInteger, VICameraType) {
    VICameraTypeBack,
    VICameraTypeFront
};

@interface VICameraManagerModule : NSObject
+ (instancetype)sharedInstance;

- (void)switchCamera:(VICameraType)type;

- (void)setCameraResolution:(CGSize)size;
@end
