//
//  VIPayload.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 14.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface VIPayload : NSObject

@property(nonatomic,strong) NSObject* data;
@property(nonatomic,strong) NSString* title;
@property(nonatomic,strong) NSString* type;

-(instancetype)init NS_UNAVAILABLE;
-(instancetype)initWithTitle:(NSString*)title type:(NSString*)type dataString:(NSString*)dataString;
-(instancetype)initWithTitle:(NSString*)title type:(NSString*)type dataDictionary:(NSDictionary*)dataDictionary;
-(instancetype)initWithTitle:(NSString*)title type:(NSString*)type dataArray:(NSArray*)dataArray;

@end
