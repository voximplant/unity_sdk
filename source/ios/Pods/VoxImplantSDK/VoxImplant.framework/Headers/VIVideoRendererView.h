//
//  VIVideoRendererView.h
//  Pods
//
//  Created by Andrey Syvrachev (asyvrachev@zingaya.com) on 26.12.16.
//  Copyright © 2017 VoxImplant (www.voximplant.com). All rights reserved.
//

#import <UIKit/UIKit.h>
#import <WebRTC/WebRTC.h>
#import "VIVideoRenderer.h"

/**
@enum VIVideoResizeMode
@constant VIVideoResizeModeFit Video frame is scaled to be fit the size of the view by maintaining the aspect ratio (black borders may be displayed).
@constant VIVideoResizeModeFill Video frame is scaled to fill the size of the view by maintaining the aspect ratio. Some portion of the video frame may be clipped.
*/
typedef NS_ENUM(NSUInteger, VIVideoResizeMode) {
    VIVideoResizeModeFit,
    VIVideoResizeModeFill
};

/**
@class VIVideoRendererView
@abstract iOS view that renders remote video or local camera preview video
*/
@interface VIVideoRendererView : UIView<VIVideoRenderer>

/**
Resize mode for video renderer.
@property resizeMode
@type {VIVideoResizeMode}
*/
@property(nonatomic,assign) VIVideoResizeMode resizeMode;

- (instancetype)init NS_UNAVAILABLE;

/**
Create VIVideoRendererView instance
@method initWithContainerView:
@param {UIView*} containerView UIView to which video renderer will be added as a subview.
@return {instancetype} VIVideoRendererView instance
*/
- (instancetype)initWithContainerView:(UIView*)containerView;

@end
