//
//  VICallStat.h
//  VoxImplant
//
//  Created by Andrey Syvrachev (asyvrachev@zingaya.com) on 18.04.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreGraphics/CoreGraphics.h>

/**
 Enum of supported audio/video stream directions
 */
typedef NS_ENUM(NSUInteger, VIStreamDirection) {
    /** Stream is directed from this SDK toward the Voximplant cloud or toward another SDK in Peer-to-Peer mode */
    VIStreamDirectionSend,
    /** Stream is directed to this SDK from the Voximplant cloud or from another SDK in Peer-to-Peer mode */
    VIStreamDirectionRecv
};

/**
 Enum of supported stream types
 */
typedef NS_ENUM(NSUInteger,VIStreamType){
    /** Stream is not video or audio, defined for future compatibility */
    VIStreamTypeUnknown,
    /** Audio stream */
    VIStreamTypeAudio,
    /** Video stream */
    VIStreamTypeVideo
};

@class RTCLegacyStatsReport;

/** Stream statistics */
@interface VIStreamStat : NSObject

/**
 Unique stream id as a string, same as "streamId" for <VIVideoStream>
 */
@property(nonatomic,copy,readonly)   NSString* streamId;

/**
 Audio or video codec name, ex "VP8" or "H264"
 */
@property(nonatomic,copy,readonly)   NSString* codec;

/**
 Stream direction: VIStreamDirectionSend for streams directed from this device,
 VIStreamDirectionRecv for streams directed to this device
 */
@property(nonatomic,assign,readonly) VIStreamDirection dir;

/**
 Stream type: VIStreamTypeAudio or VIStreamTypeVideo. VIStreamTypeUnknown for
 future SDK versions compatibility
 */
@property(nonatomic,assign,readonly) VIStreamType type;

/**
 Number of bytes transferred through stream
 */
@property(nonatomic,assign,readonly) NSInteger bytes;

/**
 Number of packets transferred through stream
 */
@property(nonatomic,assign,readonly) NSInteger packets;

/**
 Total number of packets lost
 */
@property(nonatomic,assign,readonly) NSInteger packetsLost;

/**
 Total packet loss percentage from 0 to 100
 */
@property(nonatomic,assign,readonly) NSInteger packetLossPercents;

/**
 Packet loss percentage from 0 to 100 for last 5 seconds.
 */
@property(nonatomic,assign,readonly) NSInteger packetLossCurrentPercents;

/**
 Jitter delay, in milliseconds, for receive streams. For send streams always 0.
 */
@property(nonatomic,assign,readonly) NSInteger jitterMs;

/**
 Jitter delay, in milliseconds, for single buffer, for receive streams. For send streams always 0
 */
@property(nonatomic,assign,readonly) NSInteger jitterBufferMs;

/**
 Video frame size, in pixels, for video streams. For audio streams always 0.
 */
@property(nonatomic,assign,readonly) CGSize    videoFrame;

/**
 Current video FPS, for video streams. For audio streams always 0.
 */
@property(nonatomic,assign,readonly) NSInteger videoFps;

@end

/** Video statistics */
@interface VIVideoStat: NSObject

/**
 Current video retransmit bitrate, in bits per second, high in case of poor network connection.
 */
@property(nonatomic,assign,readonly) NSInteger retransmitBitrate;

/**
 Current video transmit bitrate, in bits per second
 */
@property(nonatomic,assign,readonly) NSInteger transmitBitrate;

/**
 Current send bandwidth that is assumed by the engine to be available for sending video, in bits per second.
 */
@property(nonatomic,assign,readonly) NSInteger availableSendBandwidth;

@end

/** Endpoint statistics */
@interface VIEndpointStat : NSObject

/**
 Collection of statistics for remote receive streams.
 */
@property(nonatomic,strong,readonly) NSArray<VIStreamStat*>* remoteStreams; // only remote recv streams

@end

/** Call staticstics */
@interface VICallStat : NSObject

/**
 References video statistics information
 */
@property(nonatomic,strong,readonly) VIVideoStat* video;

/**
 References remote endpoints statistics information
 */
@property(nonatomic,strong,readonly) NSArray<VIEndpointStat*>* endpoints;

/**
 References local send streams statistics information
 */
@property(nonatomic,strong,readonly) NSArray<VIStreamStat*>* localStreams;

/**
 References statistics information for all streams in one place: 'audio-send',
 'audio-recv', 'video-send', 'video-recv'
 */
@property(nonatomic,strong,readonly) NSArray<VIStreamStat*>* streams;

/**
 References original WebRTC statistics information
 'audio-recv', 'video-send', 'video-recv'
 */
@property(nonatomic,strong,readonly) NSArray<RTCLegacyStatsReport*>* webrtcOriginalReports;

@end

