//
//  RTCCustomVideoSource.h
//  products
//
//  Created by Andrey Syvrachev on 10.05.17.
//
//

#import <UIKit/UIKit.h>
#import <WebRTC/RTCMacros.h>
#import <WebRTC/RTCVideoSource.h>

@class RTCPeerConnectionFactory;
@class RTCMediaConstraints;

// right now only NV12 supported

RTC_EXTERN uint32_t kFOURCC_I420;

RTC_EXPORT
@interface RTCVideoFormat: NSObject

- (instancetype)init NS_UNAVAILABLE;

- (instancetype)initWithWidth:(NSUInteger)width height:(NSUInteger)height interval:(NSUInteger)interval fourcc:(uint32_t)fourcc;
- (instancetype)initWithWidth:(NSUInteger)width height:(NSUInteger)height fps:(NSUInteger)fps  fourcc:(uint32_t)fourcc;

@property(nonatomic, assign, readonly) NSUInteger width;
@property(nonatomic, assign, readonly) NSUInteger height;
@property(nonatomic, assign, readonly) NSUInteger interval; // Nanoseconds = 1/FPS
@property(nonatomic, assign, readonly) uint32_t fourcc; // Nanoseconds = 1/FPS

@end



@protocol RTCCustomVideoSourceDelegate <NSObject>

- (void)startWithVideoFormat:(RTCVideoFormat*)videoFormat;
- (void)stop;

@end

@class RTCVideoFrame;
RTC_EXPORT
@interface RTCCustomVideoSource : RTCVideoSource

- (instancetype)init NS_UNAVAILABLE;

- (instancetype)initWithFactory:(RTCPeerConnectionFactory *)factory
                    constraints:(RTCMediaConstraints *)constraints
               supportedFormats:(NSArray<RTCVideoFormat*>*)supportedFormats
                       delegate:(id<RTCCustomVideoSourceDelegate>)delegate;

- (void)sendVideoFrame:(RTCVideoFrame*)videoFrame;

@end
