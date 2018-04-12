//
//  Created by Aleksey Zinchenko on 08/03/2017.
//
//

#ifndef UNITY_BRIDGE_VOXIMPLANTIOSSDK_H
#define UNITY_BRIDGE_VOXIMPLANTIOSSDK_H

#import <VoxImplant/VoxImplant.h>

NS_ASSUME_NONNULL_BEGIN

@class VIBaseVideoStreamSource;

@interface VIClientBridge : NSObject

// TODO: Provide consistent factory methods
@property(nonatomic, strong, readonly) VIClient *client;
@property(nonatomic, strong, readonly, nullable) NSString *preferredVideoCodec;

- (instancetype)initWithPreferH264:(BOOL)h264 NS_DESIGNATED_INITIALIZER;

- (instancetype)init NS_UNAVAILABLE;

@end

extern VIClientBridge *s_bridge;

NS_ASSUME_NONNULL_END

#endif
