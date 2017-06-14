//
//  VICallStat.h
//  Pods
//
//  Created by Andrey Syvrachev (asyvrachev@zingaya.com) on 18.04.17.
//  Copyright © 2017 VoxImplant (www.voximplant.com). All rights reserved.
//

#import <Foundation/Foundation.h>
#import <CoreGraphics/CoreGraphics.h>

/*
 warning, this Statistics API is not final right now, it may be changed in future releases
*/

typedef NS_ENUM(NSUInteger,VIStreamDirection){
    VIStreamDirectionSend,
    VIStreamDirectionRecv
};

typedef NS_ENUM(NSUInteger,VIStreamType){
    VIStreamTypeUnknown,
    VIStreamTypeAudio,
    VIStreamTypeVideo
};

@class RTCLegacyStatsReport;
@interface VIStreamStat : NSObject

@property(nonatomic,copy,readonly)   NSString* streamId;
@property(nonatomic,copy,readonly)   NSString* codec;

@property(nonatomic,assign,readonly) VIStreamDirection dir;
@property(nonatomic,assign,readonly) VIStreamType type;

@property(nonatomic,assign,readonly) NSInteger bytes;
@property(nonatomic,assign,readonly) NSInteger packets;
@property(nonatomic,assign,readonly) NSInteger packetsLost;
@property(nonatomic,assign,readonly) NSInteger packetLossPercents;
@property(nonatomic,assign,readonly) NSInteger packetLossCurrentPercents;

// receive only statistics, for transmit streams always zero
@property(nonatomic,assign,readonly) NSInteger jitterMs;
@property(nonatomic,assign,readonly) NSInteger jitterBufferMs;

// only for video streams, for audio streams always zero
@property(nonatomic,assign,readonly) CGSize    videoFrame;
@property(nonatomic,assign,readonly) NSInteger videoFps;

@end


@interface VIVideoStat: NSObject

@property(nonatomic,assign,readonly) NSInteger retransmitBitrate;
@property(nonatomic,assign,readonly) NSInteger transmitBitrate;
@property(nonatomic,assign,readonly) NSInteger availableSendBandwidth;

@end

@interface VIEndPointStat : NSObject

@property(nonatomic,strong,readonly) NSArray<VIStreamStat*> *remoteStreams; // only remote recv streams

@end


@interface VICallStat : NSObject

// common video stat
@property(nonatomic,strong,readonly) VIVideoStat* video;

@property(nonatomic,strong,readonly) NSArray<VIEndPointStat*> *endPoints; // remote EndPoints

@property(nonatomic,strong,readonly) NSArray<VIStreamStat*> *localStreams; // only local sendStreams
@property(nonatomic,strong,readonly) NSArray<VIStreamStat*> *streams; // all streams audio-send,audio-recv,video-send,video-recv in one array for convinience

// original webrtc statistical reports
@property(nonatomic,strong,readonly) NSArray<RTCLegacyStatsReport *> *webrtcOriginalReports;

@end
