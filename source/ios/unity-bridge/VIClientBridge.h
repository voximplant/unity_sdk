//
//  Created by Aleksey Zinchenko on 08/03/2017.
//
//

#ifndef UNITY_BRIDGE_VOXIMPLANTIOSSDK_H
#define UNITY_BRIDGE_VOXIMPLANTIOSSDK_H

#import <VoxImplant/VoxImplant.h>

@class VIBaseVideoStreamSource;

@interface VIClientBridge : NSObject

@property(nonatomic, strong, readonly) VIClient *client;

@end

extern VIClientBridge *s_bridge;

#endif
