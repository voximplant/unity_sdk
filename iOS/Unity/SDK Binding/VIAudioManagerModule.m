/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

#import "VIAudioManagerModule.h"
#import "VIEmitter.h"

@interface VIAudioManagerModule () <VIAudioManagerDelegate>

@end

@implementation VIAudioManagerModule

+ (instancetype)sharedInstance {
    static VIAudioManagerModule *instance;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        instance = [[VIAudioManagerModule alloc] init];
    });
    return instance;
}

- (instancetype)init {
    self = [super init];

    if (self) {
        [VIAudioManager sharedAudioManager].delegate = self;
    }

    return self;
}

- (void)selectAudioDevice:(VIAudioDeviceType)device {
    [[VIAudioManager sharedAudioManager] selectAudioDevice:[VIAudioDevice deviceWithType:device]];
}

- (VIAudioDeviceType)currentAudioDevice {
    return [VIAudioManager sharedAudioManager].currentAudioDevice.type;
}

- (NSString *)availableAudioDevices {
    __block NSMutableArray *devices = [NSMutableArray array];
    [[VIAudioManager sharedAudioManager].availableAudioDevices enumerateObjectsUsingBlock:^(VIAudioDevice *obj, BOOL *stop) {
        [devices addObject:@(obj.type)];
    }];
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:devices options:(NSJSONWritingOptions) 0 error:nil];
    return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
}

- (void)audioDeviceChanged:(VIAudioDevice *)audioDevice {
    [VIEmitter sendAudioManagerMessage:@"AudioDeviceChanged" payload:@{@"device": @(audioDevice.type)}];
}

- (void)audioDeviceUnavailable:(VIAudioDevice *)audioDevice {
    [VIEmitter sendAudioManagerMessage:@"AudioDeviceUnavailable" payload:@{@"device": @(audioDevice.type)}];
}

- (void)audioDevicesListChanged:(NSSet<VIAudioDevice *> *)availableAudioDevices {
    __block NSMutableSet *devices = [NSMutableSet set];
    [availableAudioDevices enumerateObjectsUsingBlock:^(VIAudioDevice *obj, BOOL *stop) {
        [devices addObject:@(obj.type)];
    }];

    [VIEmitter sendAudioManagerMessage:@"AudioDeviceListChanged" payload:@{@"devices": devices}];
}

@end
