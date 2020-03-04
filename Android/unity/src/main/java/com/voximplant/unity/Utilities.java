/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
package com.voximplant.unity;

import java.util.HashMap;
import java.util.Map;

public class Utilities {
    public static Map<String, String> ParseHeaders(String headers) {
        Map<String, String> result = new HashMap<>();
        String[] lines = headers.split("\r\n");
        for (String line : lines) {
            String[] pair = line.split(":");
            result.put(pair[0], pair[1]);
        }
        return result;
    }
}
