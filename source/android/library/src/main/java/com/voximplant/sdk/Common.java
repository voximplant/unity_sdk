package com.voximplant.sdk;

import java.util.ArrayList;

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

}
