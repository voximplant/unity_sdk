//
//  VIVideoRendererView.h
//  VoxImplant
//
//  Created by Andrey Syvrachev (asyvrachev@zingaya.com) on 26.12.16.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <WebRTC/WebRTC.h>
#import "VIVideoRenderer.h"

/**
Enum of supported video resize modes
*/
typedef NS_ENUM(NSUInteger, VIVideoResizeMode) {
    /** Video frame is scaled to be fit the size of the view by maintaining the aspect ratio (black borders may be displayed). */
    VIVideoResizeModeFit,
    /** Video frame is scaled to fill the size of the view by maintaining the aspect ratio. Some portion of the video frame may be clipped. */
    VIVideoResizeModeFill
};

/**
iOS view that renders remote video or local camera preview video
*/
@interface VIVideoRendererView : UIView<VIVideoRenderer>

/**
Resize mode for video renderer.
*/
@property(nonatomic,assign) VIVideoResizeMode resizeMode;

- (instancetype)init NS_UNAVAILABLE;

/**
Create VIVideoRendererView instance
 
@param containerView UIView to which video renderer will be added as a subview.
@return VIVideoRendererView instance
*/
- (instancetype)initWithContainerView:(UIView*)containerView;

@end
