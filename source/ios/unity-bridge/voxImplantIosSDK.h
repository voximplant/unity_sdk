//
//  voxImplantIosSDK.cpp
//  Unity-iPhone
//
//  Created by Aleksey Zinchenko on 08/03/2017.
//
//

#ifndef UNITY_BRIDGE_VOXIMPLANTIOSSDK_H
#define UNITY_BRIDGE_VOXIMPLANTIOSSDK_H

#import <VoxImplant/VoxImplant.h>

@interface ProxyingVoxImplantDelegate : NSObject <VoxImplantDelegate>

@property (nonatomic, strong) VoxImplant *sdk;

@end

extern ProxyingVoxImplantDelegate *singleton;

#endif
