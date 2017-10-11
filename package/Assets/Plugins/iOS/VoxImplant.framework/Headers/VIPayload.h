//
//  VIPayload.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 14.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import <Foundation/Foundation.h>

/**
 Interface that represent message payload format.
 */
@interface VIPayload : NSObject

/**
 Payload data
 */
@property(nonatomic,strong) NSObject* data;

/**
 Payload fragment unique title. Used to identifier fragment in the list of fragments associated with the message
 */
@property(nonatomic,strong) NSString* title;

/**
 Arbitrary payload type
 */
@property(nonatomic,strong) NSString* type;


/**
 @warning NS_UNAVAILABLE
 */
-(instancetype)init NS_UNAVAILABLE;


/**
 Initialize VIPayload with NSString

 @param title      Payload fragment unique title.
 @param type       Arbitrary payload type
 @param dataString Payload data string
 @return VIPayload instance
 */
-(instancetype)initWithTitle:(NSString*)title type:(NSString*)type dataString:(NSString*)dataString;

/**
 Initialize VIPayload with NSDictionary

 @param title          Payload fragment unique title.
 @param type           Arbitrary payload type
 @param dataDictionary Payload data disctionary
 @return VIPayload instance
 */
-(instancetype)initWithTitle:(NSString*)title type:(NSString*)type dataDictionary:(NSDictionary*)dataDictionary;

/**
 Initialize VIPayload with NSArray

 @param title     Payload fragment unique title.
 @param type      Arbitrary payload type
 @param dataArray Payload data array
 @return VIPayload instance
 */
-(instancetype)initWithTitle:(NSString*)title type:(NSString*)type dataArray:(NSArray*)dataArray;

@end
