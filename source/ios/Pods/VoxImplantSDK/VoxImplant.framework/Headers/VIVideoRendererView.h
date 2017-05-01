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

typedef NS_ENUM(NSUInteger, VIVideoResizeMode) {
    VIVideoResizeModeFit,
    VIVideoResizeModeFill
};


@interface VIVideoRendererView : UIView<VIVideoRenderer>

@property(nonatomic,assign) VIVideoResizeMode resizeMode;

- (instancetype)init NS_UNAVAILABLE;

- (instancetype)initWithContainerView:(UIView*)containerView;

@end
