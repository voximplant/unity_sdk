//
//  VIUser.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 07.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface VIUser : NSObject

- (instancetype)init NS_UNAVAILABLE;

@property(nonatomic,strong,readonly) NSArray<NSString*>* conversationList;
@property(nonatomic,strong,readonly) NSDictionary* customData;
@property(nonatomic,strong,readonly) NSDictionary* privateCustomData;
@property(nonatomic,strong,readonly) NSString* userId;
@property(nonatomic,strong,readonly) NSArray<NSNumber*>* notifications;

@end
