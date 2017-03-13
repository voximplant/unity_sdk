//
// Created by Aleksey Zinchenko on 01/03/2017.
// Copyright (c) 2017 voximplant. All rights reserved.
//

#import "BlocksThread.h"

@interface BlocksThread ()

@property(nonatomic, strong, readonly) NSCondition *threadStarted;
@property(nonatomic, strong, readonly) NSCondition *submit;

@end

@implementation BlocksThread

- (instancetype)init {
    self = [super init];
    if (self == nil)
        return self;

    _threadStarted = [NSCondition new];
    _submit = [NSCondition new];

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

        while (![NSThread currentThread].isCancelled &&
                [self.runLoop runMode:NSDefaultRunLoopMode beforeDate:[NSDate distantFuture]]) {

        }
    }
};

- (void)enqueueBlock:(void (^)())block {
    CFRunLoopRef const cfRunLoop = self.runLoop.getCFRunLoop;

    CFRunLoopPerformBlock(cfRunLoop, kCFRunLoopDefaultMode, ^{
        if (block) {
            block();
        }
    });

    CFRunLoopWakeUp(cfRunLoop);
}

- (void)enqueueBlockAndWait:(void (^)())block {
    CFRunLoopRef const cfRunLoop = self.runLoop.getCFRunLoop;

    [self.submit lock];

    CFRunLoopPerformBlock(cfRunLoop, kCFRunLoopDefaultMode, ^{
        if (block) {
            block();
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