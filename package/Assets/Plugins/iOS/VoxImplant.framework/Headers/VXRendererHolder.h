//
//  VXRendererHolder.h
//  Pods
//
//  Created by Andrey Syvrachev on 01.03.17.
//
//

#import <Foundation/Foundation.h>

@protocol RTCVideoRenderer;
@protocol VXRendererHolder <NSObject>

- (id<RTCVideoRenderer>) renderer;

- (void) start;

- (void) stop;

@end
