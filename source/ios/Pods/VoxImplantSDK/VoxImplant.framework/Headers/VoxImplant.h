//
//  VoxImplant.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 20.02.17.
//  Copyright © 2017 VoxImplant (www.voximplant.com). All rights reserved.
//

#import <UIKit/UIKit.h>

//! Project version number for VoxImplant.
FOUNDATION_EXPORT double VoxImplantVersionNumber;

//! Project version string for VoxImplant.
FOUNDATION_EXPORT const unsigned char VoxImplantVersionString[];

// In this header, you should import all the public headers of your framework using statements like #import <VoxImplant/PublicHeader.h>

#import <Foundation/Foundation.h>

#import <VoxImplant/VIClient.h>
#import <VoxImplant/VICall.h>
#import <VoxImplant/VIEndpoint.h>
#import <VoxImplant/VIAudioManager.h>
#import <VoxImplant/VIVideoSource.h>
#import <VoxImplant/VICameraManager.h>
#import <VoxImplant/VIVideoRenderer.h>
#import <VoxImplant/VIVideoRendererView.h>
#import <VoxImplant/VIVideoStream.h>
#import <VoxImplant/VICallStat.h>

// Messenger
#import <VoxImplant/VIConversation.h>
#import <VoxImplant/VIConversationParticipant.h>
#import <VoxImplant/VIConversationEvent.h>
#import <VoxImplant/VIConversationServiceEvent.h>
#import <VoxImplant/VIErrorEvent.h>
#import <VoxImplant/VIMessage.h>
#import <VoxImplant/VIMessageEvent.h>
#import <VoxImplant/VIMessenger.h>
#import <VoxImplant/VIMessengerEvent.h>
#import <VoxImplant/VIPayload.h>
#import <VoxImplant/VIRetransmitEvent.h>
#import <VoxImplant/VISubscribeEvent.h>
#import <VoxImplant/VIUser.h>
#import <VoxImplant/VIUserEvent.h>
#import <VoxImplant/VIUserStatusEvent.h>
#import <VoxImplant/VIMessengerPushNotificationProcessing.h>



// legacy API
#import <VoxImplant/LegacyAPI.h>
#import <VoxImplant/VoxImplantDelegate.h>

