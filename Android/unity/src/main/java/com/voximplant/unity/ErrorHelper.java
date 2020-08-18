/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
package com.voximplant.unity;

import com.voximplant.sdk.call.CallError;
import com.voximplant.sdk.client.LoginError;

import java.util.HashMap;
import java.util.Map;

public class ErrorHelper {
    public static long CodeForCallError(CallError errorCode) {
        switch (errorCode) {
            case REJECTED:
                return 10004;
            case TIMEOUT:
                return 10005;
            case MEDIA_IS_ON_HOLD:
                return 10007;
            case ALREADY_IN_THIS_STATE:
                return 10008;
            case INCORRECT_OPERATION:
                return 10009;
            case INTERNAL_ERROR:
                return 10010;
            case FUNCTIONALITY_IS_DISABLED:
                return 10011; // @todo: code is correct?
            case MISSING_PERMISSION:
                return 10012; // @todo: code is correct?
        }
        return 10010;
    }

    public static Map<String, Object> payloadForError(LoginError loginError) {
        Map<String, Object> errorPayload = new HashMap<>();
        switch (loginError) {
            case INVALID_PASSWORD:
                errorPayload.put("code", 401);
                errorPayload.put("error", "Invalid login or password");
                break;
            case MAU_ACCESS_DENIED:
                errorPayload.put("code", 402);
                errorPayload.put("error", "Monthly Active Users (MAU) limit is reached. Payment is required.");
                break;
            case INVALID_USERNAME:
                errorPayload.put("code", 404);
                errorPayload.put("error", "Invalid username");
                break;
            case ACCOUNT_FROZEN:
                errorPayload.put("code", 403);
                errorPayload.put("error", "Account frozen");
                break;
            case INVALID_STATE:
                errorPayload.put("code", 491);
                errorPayload.put("error", "Invalid state");
                break;
            case NETWORK_ISSUES:
                errorPayload.put("code", 503);
                errorPayload.put("error", "Network issues");
                break;
            case TOKEN_EXPIRED:
                errorPayload.put("code", 701);
                errorPayload.put("error", "Token expired");
                break;
            case TIMEOUT:
                errorPayload.put("code", 408);
                errorPayload.put("error", "Timeout");
                break;
            default:
            case INTERNAL_ERROR:
                errorPayload.put("code", 500);
                errorPayload.put("error", "Internal error");
                break;
        }

        return errorPayload;
    }

    public static Map<String, Object> payloadForError(String error) {
        Map<String, Object> errorPayload = new HashMap<>();
        errorPayload.put("error", error);
        if ("Connectivity check failed".equals(error)) {
            errorPayload.put("code", 10000);
        } else {
            errorPayload.put("code", 10001);
        }
        return errorPayload;
    }
}
