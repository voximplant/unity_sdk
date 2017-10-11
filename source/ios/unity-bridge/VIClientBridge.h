//
//  Created by Aleksey Zinchenko on 08/03/2017.
//
//

#ifndef UNITY_BRIDGE_VOXIMPLANTIOSSDK_H
#define UNITY_BRIDGE_VOXIMPLANTIOSSDK_H

#import <VoxImplant/VoxImplant.h>

@class VIBaseVideoStreamSource;

@interface VIClientBridge : NSObject

// TODO: Provide consistent factory methods
@property(nonatomic, strong, readonly) VIClient *client;
@property(nonatomic, strong, readonly, nullable) NSString *preferrredVideoCodec;

- (instancetype)initWithPreferH264:(BOOL)h264 NS_DESIGNATED_INITIALIZER;

- (instancetype)init NS_UNAVAILABLE;

@end

extern VIClientBridge *s_bridge;

#endif
