//
// Created by Aleksey Zinchenko on 01/03/2017.
// Copyright (c) 2017 voximplant. All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface VIBlocksThread : NSThread

- (instancetype)initWithTarget:(id)target selector:(SEL)selector object:(nullable id)argument NS_UNAVAILABLE;

- (instancetype)init;
- (instancetype)initWithQueueLimit:(NSUInteger)queueLimit NS_DESIGNATED_INITIALIZER;

- (void)enqueueBlock:(nullable void (^)())block;

- (void)enqueueBlockAndWait:(void (^)())block;

@property(nonatomic, readonly) NSRunLoop *runLoop;

@end

NS_ASSUME_NONNULL_END