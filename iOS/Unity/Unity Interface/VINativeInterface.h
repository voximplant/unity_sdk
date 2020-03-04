/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

#import <Foundation/Foundation.h>

#ifndef UNITY_BRIDGE_NATIVEINTERFACE_H
#define UNITY_BRIDGE_NATIVEINTERFACE_H

#if __cplusplus
extern "C"
{
#endif

void voximplant_send_message(const char *node, NSString *event_payload);

#if __cplusplus
};
#endif

#endif //UNITY_BRIDGE_NATIVEINTERFACE_H
