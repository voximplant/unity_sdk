//
//  VIMessage.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 14.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.  
//

#import <Foundation/Foundation.h>

@class VIPayload;
@interface VIMessage : NSObject

@property (nonatomic,strong,readonly) NSString* conversation;
@property (nonatomic,strong,readonly) NSString* sender;
@property (nonatomic,strong,readonly) NSString* uuid;
@property (nonatomic,strong,readonly) NSNumber* seq;

@property (nonatomic,strong) NSString* text;
@property (nonatomic,strong) NSArray<VIPayload*>* payload;

- (void)update;

- (void)remove;


@end
