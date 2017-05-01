//
// Created by Aleksey Zinchenko on 01/03/2017.
// Copyright (c) 2017 voximplant. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface VIJsonDic : NSObject

@property NSArray *list;
@property NSMutableDictionary *dic;

- (instancetype)initWithJSONString:(NSString *)JSONString;

@end

NS_ASSUME_NONNULL_END