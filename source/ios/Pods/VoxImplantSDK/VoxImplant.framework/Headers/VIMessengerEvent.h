//
//  VIMessengerEvent.h
//  VoxImplant
//
//  Created by Andrey Syvrachev on 16.06.17.
//  Copyright © 2017 Zingaya. All rights reserved.
//

#import <Foundation/Foundation.h>


/**
 Types of messenger events
 */
typedef NS_ENUM(NSInteger,VIMessengerEventType) {
    /** Error event */
    VIMessengerEventTypeError,
    /** Get user event */
    VIMessengerEventTypeGetUser,
    /** Edit user event */
    VIMessengerEventTypeEditUser,
    /** Create conversation event */
    VIMessengerEventTypeCreateConversation,
    /** Remove conversation event */
    VIMessengerEventTypeRemoveConversation,
    /** Edit conversation event */
    VIMessengerEventTypeEditConversation,
    /** Get conversation event */
    VIMessengerEventTypeGetConversation,
    /** Subscribe event */
    VIMessengerEventTypeSubscribe,
    /** Unsubscribe event */
    VIMessengerEventTypeUnsubscribe,
    /** Send message event */
    VIMessengerEventTypeSendMessage,
    /** Edit message event */
    VIMessengerEventTypeEditMessage,
    /** Remove message event */
    VIMessengerEventTypeRemoveMessage,
    /** Typing event */
    VIMessengerEventTypeTyping,
    /** Marked as read event */
    VIMessengerEventTypeRead,
    /** Marked as delivered event */
    VIMessengerEventTypeDelivered,
    /** User status event */
    VIMessengerEventTypeUserStatus,
    /** Retransmit event */
    VIMessengerEventTypeRetransmit,
    /** Unknown event */
    VIMessengerEventTypeUnknown
};

/**
 Actions that trigger messenger events.
 */
typedef NS_ENUM(NSInteger,VIMessengerActionType) {
    /** Create conversation action */
    VIMessengerActionTypeCreateConversation,
    /** Remove conversation action */
    VIMessengerActionTypeRemoveConversation,
    /** Join conversation action */
    VIMessengerActionTypeJoinConversation,
    /** Leave conversation action */
    VIMessengerActionTypeLeaveConversation,
    /** Edit conversation action */
    VIMessengerActionTypeEditConversation,
    /** Get user action */
    VIMessengerActionTypeGetUser,
    /** Get users action */
    VIMessengerActionTypeGetUsers,
    /** Edit user action */
    VIMessengerActionTypeEditUser,
    /** Get conversation action */
    VIMessengerActionTypeGetConversation,
    /** Get conversations action */
    VIMessengerActionTypeGetConversations,
    /** Add participants action */
    VIMessengerActionTypeAddParticipants,
    /** Edit participants action */
    VIMessengerActionTypeEditParticipants,
    /** Remove participants action */
    VIMessengerActionTypeRemoveParticipants,
    /** Add moderators action */
    VIMessengerActionTypeAddModerators,
    /** Remove moderators action */
    VIMessengerActionTypeRemoveModerators,
    /** Mark as delivered action */
    VIMessengerActionTypeMarkAsDelivered,
    /** Mark as read action */
    VIMessengerActionTypeMarkAsRead,
    /** Typing action */
    VIMessengerActionTypeTyping,
    /** Retransmit events action */
    VIMessengerActionTypeRetransmitEvents,
    /** Subscribe action */
    VIMessengerActionTypeSubscribe,
    /** Unsubscribe action */
    VIMessengerActionTypeUnsubscribe,
    /** Set status action */
    VIMessengerActionTypeSetStatus,
    /** Send message action */
    VIMessengerActionTypeSendMessage,
    /** Edit message action */
    VIMessengerActionTypeEditMessage,
    /** Remove message action */
    VIMessengerActionTypeRemoveMessage,
    /** Manage notifications action */
    VIMessengerActionTypeManageNotifications,
    /** Unsubscribe action */
    VIMessengerActionTypeUnknown
};


/**
 Interface that represents all messenger events provided via <VIMessengerDelegate>.
 */
@interface VIMessengerEvent : NSObject

/**
 Type of event
 @see <VIMessengerEventType>
 */
@property(nonatomic,assign,readonly) VIMessengerEventType eventType;

/**
 Action that is the reason this event was triggered
 @see <VIMessengerActionType>
 */
@property(nonatomic,assign,readonly) VIMessengerActionType incomingAction;

/**
 Voximplant user identifier, ex _username@appname.accname_, of the user that initiated the event.
 */
@property(nonatomic,strong,readonly) NSString* userId;

@end


/**
 Interface that represents all messenger events provided via <VIMessengerDelegate> that have event sequence number.
 */
@interface VIMessengerEventSeq : VIMessengerEvent

/**
 Event sequence number
 */
@property(nonatomic,strong) NSNumber* seq;
@end
