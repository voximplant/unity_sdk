//
// Created by Aleksey Zinchenko on 08/03/2017.
// Copyright (c) 2017 voximplant. All rights reserved.
//

#import <Foundation/Foundation.h>

#import <VoxImplant/Voximplant.h>
#import <VoxImplant/VoxImplant+Renderers.h>
#import <VoxImplant/VXRendererHolder.h>

NS_ASSUME_NONNULL_BEGIN

typedef void(^NativeTextureReportBlock)(GLuint, void *, int, int);

@interface EAGLRendererHolder : NSObject <VXRendererHolder>

- (instancetype)init NS_UNAVAILABLE;
- (instancetype)initWithStream:(int)stream nativeTextureReport:(NativeTextureReportBlock) block NS_DESIGNATED_INITIALIZER;

@end

NS_ASSUME_NONNULL_END