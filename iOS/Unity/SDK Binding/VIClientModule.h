/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */

#import <Foundation/Foundation.h>

@interface VIClientModule : NSObject

+ (instancetype)sharedModule;

- (void)initializeModule:(NSString *)nsstring;

- (void)connect:(BOOL)connectivityCheck gateways:(NSArray<NSString *> *)gateways;

- (void)login:(NSString *)user password:(NSString *)password;

- (void)login:(NSString *)user token:(NSString *)token;

- (void)login:(NSString *)user hash:(NSString *)hash;

- (void)refreshTokenWithUser:(NSString *)user token:(NSString *)token;

- (void)requestOneTimeKey:(NSString *)user;

- (void)disconnect;

- (NSUInteger)state;

- (NSString *)call:(NSString *)username receiveVideo:(BOOL)receiveVideo sendVideo:(BOOL)sendVideo videoCodec:(NSInteger)videoCodec customData:(NSString *)customData headers:(NSString *)headers;

- (NSString *)callConference:(NSString *)conference receiveVideo:(BOOL)receiveVideo sendVideo:(BOOL)sendVideo videoCodec:(NSInteger)videoCodec customData:(NSString *)customData headers:(NSString *)headers;

@end
