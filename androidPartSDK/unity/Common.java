package com.sdk.unity;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

import static java.lang.System.out;

/**
 * Created by omen on 12.09.2016.
 */

public class Common {

    public static ArrayList<String> GetParamList(String p){
        ArrayList<String> list = new ArrayList<String>();
        String temp = p;

        while(!temp.trim().equals("")){
            list.add(getParam(temp));
            temp = temp.subSequence(temp.indexOf("}")+1,temp.length()).toString();
        }
        return list;
    }

    public static String getParam(String p){
        return p.substring(p.indexOf("{",0) + 1,p.indexOf("}",0));
    }

    public static String getParamString(ArrayList<Object> p){
        String str = "";
        for (Object item: p) {
            if (item instanceof Map) {
                str += "{" + item.toString() + "}";
            }
            else if (item instanceof Boolean){
                str += "{" + (((Boolean)item)?"1":"0") + "}";
            }
            else
                str += "{" + item.toString() + "}";
        }
        return str;
    }
}
