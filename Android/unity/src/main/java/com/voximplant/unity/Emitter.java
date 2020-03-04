/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
package com.voximplant.unity;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.unity3d.player.UnityPlayer;

import java.security.KeyPair;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

// @todo: optimize
public class Emitter {
    private static final String UNITY_LISTENER = "Voximplant";
    private static Gson mGson;

    public static void sendClientMessage(String event) {
        sendClientMessage(event, null);
    }

    public static void sendClientMessage(String event, Object payload) {
        sendMessage(event, "OnClientEvent", payload);
    }

    public static void sendCallMessage(String callId, String event) {
        sendCallMessage(callId, event, null);
    }

    public static void sendCallMessage(String callId, String event, Object payload) {
        UnityPlayer.UnitySendMessage(UNITY_LISTENER, "OnCallEvent", createPayload(event, payload == null ? null : getGson().toJson(payload), callId));
    }

    private static void sendMessage(String event, String node, Object payload) {
        UnityPlayer.UnitySendMessage(UNITY_LISTENER, node, createPayload(event, payload == null ? null : getGson().toJson(payload)));
    }

    public static Gson getGson() {
        if (mGson == null) {
            mGson = new GsonBuilder()
                    .create();
        }

        return mGson;
    }

    private static String createPayload(String event, String payload) {
        return createPayload(event, payload, null);
    }

    private static String createPayload(String event, String payload, String senderId) {
        Map<String, String> message = new HashMap<>();
        message.put("event", event);
        if (senderId != null) {
            message.put("senderId", senderId);
        }
        if (payload != null) {
            message.put("payload", payload);
        }
        return getGson().toJson(message);
    }

    public static List<String> flatten(Map<String, String> headers) {
        List<String> result = new ArrayList<>();
        for (Map.Entry<String, String> pair : headers.entrySet()) {
            result.add(pair.getKey());
            result.add(pair.getValue());
        }
        return result;
    }

    public static void sendEndpointMessage(String endpointId, String event) {
        sendEndpointMessage(endpointId, event, null);
    }

    public static void sendEndpointMessage(String endpointId, String event, Object payload) {
        UnityPlayer.UnitySendMessage(UNITY_LISTENER, "OnEndpointEvent", createPayload(event, payload == null ? null : getGson().toJson(payload), endpointId));
    }

    public static void sendAudioManagerMessage(String event, Object payload) {
        UnityPlayer.UnitySendMessage(UNITY_LISTENER, "OnAudioManagerEvent", createPayload(event, getGson().toJson(payload)));
    }
}
