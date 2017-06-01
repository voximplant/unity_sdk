//
// Created by Andrey Syvrachev on 29.03.17.
//

#import <Foundation/Foundation.h>
#import "RTCPeerConnectionFactory.h"
#import <CoreVideo/CoreVideo.h>

NS_ASSUME_NONNULL_BEGIN
namespace webrtc {
    namespace video_pre_process {

        enum VideoRotation {
            kVideoRotation_0 = 0,
            kVideoRotation_90 = 90,
            kVideoRotation_180 = 180,
            kVideoRotation_270 = 270
        };
        
        typedef void (^VideoPreProcessBlock)(CVPixelBufferRef pixel_buffer,VideoRotation rotation);
    }
}


@interface RTCPeerConnectionFactory (VideoPreProcess)


- (RTCAVFoundationVideoSource *)avFoundationVideoSourceWithConstraints:
        (nullable RTCMediaConstraints *)constraints
        videoPreProcessBlock:(nonnull webrtc::video_pre_process::VideoPreProcessBlock)videoPreProcessBlock;

@end
NS_ASSUME_NONNULL_END
