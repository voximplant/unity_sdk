//
// Created by Aleksey Zinchenko on 01/03/2017.
// Copyright (c) 2017 voximplant. All rights reserved.
//

#import "VIBlocksThread.h"

@interface VIBlocksThread ()

@property(nonatomic, strong, readonly) NSCondition *threadStarted;
@property(nonatomic, strong, readonly) NSCondition *submit;

@property(nonatomic, strong, readonly) NSLock *queue;
@property(nonatomic, assign, readwrite) NSUInteger queueLength;
@property(nonatomic, assign, readonly) NSUInteger queueLimit;

@end

@implementation VIBlocksThread

- (instancetype)init {
    return [self initWithQueueLimit:NSUIntegerMax];
}

- (instancetype)initWithQueueLimit:(NSUInteger)queueLimit {
    NSParameterAssert(queueLimit > 0);

    self = [super init];
    if (self == nil)
        return self;

    _queueLimit = queueLimit;
    _threadStarted = [NSCondition new];
    _submit = [NSCondition new];
    _queue = [NSLock new];

    [self.threadStarted lock];
    [self start];
    [self.threadStarted wait];
    [self.threadStarted unlock];

    return self;
}


- (void)main {
    _runLoop = [NSRunLoop currentRunLoop];
    [self.threadStarted lock];
    [self.threadStarted broadcast];
    [self.threadStarted unlock];

    {
        // Add an empty run loop source to prevent runloop from spinning.
        CFRunLoopSourceContext sourceCtx = {
                .version = 0,
                .info = NULL,
                .retain = NULL,
                .release = NULL,
                .copyDescription = NULL,
                .equal = NULL,
                .hash = NULL,
                .schedule = NULL,
                .cancel = NULL,
                .perform = NULL
        };
        CFRunLoopSourceRef source = CFRunLoopSourceCreate(NULL, 0, &sourceCtx);
        CFRunLoopAddSource(CFRunLoopGetCurrent(), source, kCFRunLoopDefaultMode);
        CFRelease(source);

        while (![NSThread currentThread].isCancelled) {
            @autoreleasepool {
                if (![self.runLoop runMode:NSDefaultRunLoopMode beforeDate:[NSDate distantFuture]]) {
                    break;
                }
            }
        }
    }
};

- (void)ensureQueueLength {
    if (self.queueLimit != NSUIntegerMax) {
        [self.queue lock];

        while (self.queueLength >= self.queueLimit) {
            [self.queue unlock];
            CFRunLoopWakeUp(self.runLoop.getCFRunLoop);
            [NSThread sleepForTimeInterval:0.005];
            [self.queue lock];
        }

        self.queueLength++;
        [self.queue unlock];
    }
}

- (void)enqueueBlock:(void (^)())block {
    CFRunLoopRef const cfRunLoop = self.runLoop.getCFRunLoop;

    [self ensureQueueLength];

    CFRunLoopPerformBlock(cfRunLoop, kCFRunLoopDefaultMode, ^{
        if (block) {
            block();
        }

        if (self.queueLimit != NSUIntegerMax) {
            [self.queue lock];
            NSParameterAssert(self.queueLength > 0);
            self.queueLength--;
            [self.queue unlock];
        }
    });

    CFRunLoopWakeUp(cfRunLoop);
}

- (void)enqueueBlockAndWait:(void (^)())block {
    CFRunLoopRef const cfRunLoop = self.runLoop.getCFRunLoop;

    [self.submit lock];

    [self ensureQueueLength];

    CFRunLoopPerformBlock(cfRunLoop, kCFRunLoopDefaultMode, ^{
        if (block) {
            block();
        }

        if (self.queueLimit != NSUIntegerMax) {
            [self.queue lock];
            NSParameterAssert(self.queueLength > 0);
            self.queueLength--;
            [self.queue unlock];
        }

        [self.submit lock];
        [self.submit broadcast];
        [self.submit unlock];
    });

    CFRunLoopWakeUp(cfRunLoop);

    [self.submit wait];
    [self.submit unlock];
}


@end