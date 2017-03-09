//
//  VoxImplant+Renderers.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 01.03.17.
//  Copyright © 2017 Andrey Syvrachev. All rights reserved.
//

#import "VoxImplant.h"

@protocol VXRendererHolder;
@interface VoxImplant (Renderers)

- (void)addLocalRendererHolder:(id<VXRendererHolder>)holder;
- (void)removeLocalRendererHolder:(id<VXRendererHolder>)holder;

- (void)addRemoteRendererHolder:(id<VXRendererHolder>)holder;
- (void)removeRemoteRendererHolder:(id<VXRendererHolder>)holder;

@end
