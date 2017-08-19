//
//  VIMessengerEvent.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 16.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import <Foundation/Foundation.h>


typedef NS_ENUM(NSInteger,VIMessengerEventType) {
    VIMessengerEventTypeError,
    VIMessengerEventTypeGetUser,
    VIMessengerEventTypeEditUser,
    VIMessengerEventTypeCreateConversation,
    VIMessengerEventTypeRemoveConversation,
    VIMessengerEventTypeEditConversation,
    VIMessengerEventTypeGetConversation,
    VIMessengerEventTypeSubscribe,
    VIMessengerEventTypeUnsubscribe,
    VIMessengerEventTypeSendMessage,
    VIMessengerEventTypeEditMessage,
    VIMessengerEventTypeRemoveMessage,
    VIMessengerEventTypeTyping,
    VIMessengerEventTypeRead,
    VIMessengerEventTypeDelivered,
    VIMessengerEventTypeUserStatus,
    VIMessengerEventTypeRetransmit,
    VIMessengerEventTypeUnknown
};

typedef NS_ENUM(NSInteger,VIMessengerActionType) {
    VIMessengerActionTypeCreateConversation,
    VIMessengerActionTypeRemoveConversation,
    VIMessengerActionTypeJoinConversation,
    VIMessengerActionTypeLeaveConversation,
    VIMessengerActionTypeEditConversation,
    VIMessengerActionTypeGetUser,
    VIMessengerActionTypeGetUsers,
    VIMessengerActionTypeEditUser,
    VIMessengerActionTypeGetConversation,
    VIMessengerActionTypeGetConversations,
    VIMessengerActionTypeAddParticipants,
    VIMessengerActionTypeEditParticipants,
    VIMessengerActionTypeRemoveParticipants,
    VIMessengerActionTypeAddModerators,
    VIMessengerActionTypeRemoveModerators,
    VIMessengerActionTypeMarkAsDelivered,
    VIMessengerActionTypeMarkAsRead,
    VIMessengerActionTypeTyping,
    VIMessengerActionTypeRetransmitEvents,
    VIMessengerActionTypeSubscribe,
    VIMessengerActionTypeUnsubscribe,
    VIMessengerActionTypeSetStatus,
    VIMessengerActionTypeSendMessage,
    VIMessengerActionTypeEditMessage,
    VIMessengerActionTypeRemoveMessage,
    VIMessengerActionTypeManageNotifications,
    VIMessengerActionTypeUnknown
};

@interface VIMessengerEvent : NSObject

@property(nonatomic,assign,readonly) VIMessengerEventType eventType;
@property(nonatomic,assign,readonly) VIMessengerActionType incomingAction;
@property(nonatomic,strong,readonly) NSString* userId;

@end


@interface VIMessengerEventSeq : VIMessengerEvent
@property(nonatomic,strong) NSNumber* seq;
@end
