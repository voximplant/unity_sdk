using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

namespace Voximplant
{
    public enum LoginFailureReason
    {
        INVALID_PASSWORD,
        INVALID_USERNAME,
        ACCOUNT_FROZEN,
        INTERNAL_ERROR
    }

    public enum Camera
    {
        CAMERA_FACING_FRONT,
        CAMERA_FACING_BACK
    }

	[System.Serializable]
	public class SizeView
	{
		[JSONItem("x_pos",typeof(int))]
		public int x_pos;
		[JSONItem("y_pos",typeof(int))]
		public int y_pos;
		[JSONItem("width",typeof(int))]
		public int width;
		[JSONItem("height",typeof(int))]
		public int height;
		public SizeView(int pXpos, int pYpos, int pWidth, int pHeight)
		{
			x_pos = pXpos;
			y_pos = pYpos;
			width = pWidth;
			height = pHeight;
		}
	}

	[System.Serializable]
    public class Call
    {
		[JSONItem("id",typeof(string))]
        public String id;
		[JSONItem("incoming",typeof(bool))]
        public bool incoming;
		[JSONItem("video",typeof(bool))]
        public bool video;
		public Call(String id, bool incoming, bool video)
		{
			this.id = id;
			this.incoming = incoming;
			this.video = video;
		}
    }
	[System.Serializable]
    public class LoginClassParam
    {
		[JSONItem("login", typeof(string))]
        public string login;
		[JSONItem("pass", typeof(string))]
        public string pass;
        public LoginClassParam(string pName, string pPass)
        {
            login = pName;
            pass = pPass;
        }
    }
	[System.Serializable]
    public class LoginOneTimeKeyClassParam
    {
		[JSONItem("name", typeof(string))]
        public string name;
		[JSONItem("hash", typeof(string))]
        public string hash;
        public LoginOneTimeKeyClassParam(string pName, string pHash)
        {
            name = pName;
            hash = pHash;
        }
    }
	[System.Serializable]
	public class CallClassParam
	{
		[JSONItem("userCall", typeof(string))]
		public string userCall;
		[JSONItem("videoCall", typeof(bool))]
		public bool videoCall;
		[JSONItem("customData", typeof(string))]
		public string customData;
		[JSONArray("headers",typeof(PairKeyValue))]
		public PairKeyValue[] headers;

		public CallClassParam(string pCallUser, bool pVideoCall, string pCustomData, Dictionary<string, string> pHeader = null)
		{
			userCall = pCallUser;
			videoCall = pVideoCall;
			customData = pCustomData;

			if (pHeader != null)
			{
				headers = Utils.GetDictionaryToArray(pHeader);
			}
		}
	}
	[System.Serializable]
    public class DTFMClassParam
    {
		[JSONItem("callId", typeof(string))]
        public string callId;
		[JSONItem("digit", typeof(string))]
        public string digit;
        public DTFMClassParam(string pCallId, string pDigit)
        {
            callId = pCallId;
            digit = pDigit;
        }
    }
	[System.Serializable]
    public class InfoClassParam
    {
		[JSONItem("callId",typeof(string))]
        public string callId;
		[JSONItem("mimeType",typeof(string))]
        public string mimeType;
		[JSONItem("content",typeof(string))]
        public string content;
		[JSONArray("headers",typeof(PairKeyValue))]
        public PairKeyValue[] headers;
        public InfoClassParam(string pCallId, string pMimeType, string pContent, Dictionary<string, string> pHeader = null)
        {
            callId = pCallId;
            mimeType = pMimeType;
            content = pContent;
            if (pHeader != null)
            {
                headers = Utils.GetDictionaryToArray(pHeader);
            }
        }
    }
	[System.Serializable]
    public class SendMessageClassParam
    {
		[JSONItem("callId",typeof(string))]
        public string callId;
		[JSONItem("text",typeof(string))]
        public string text;
		[JSONArray("headers",typeof(PairKeyValue))]
        public PairKeyValue[] headers;
        public SendMessageClassParam(string pCallId, string pMsg, Dictionary<string, string> pHeader = null)
        {
            callId = pCallId;
            text = pMsg;
            if (pHeader != null)
            {
                headers = Utils.GetDictionaryToArray(pHeader);
            }
        }
    }
	[System.Serializable]
    public class CameraResolutionClassParam
    {
		[JSONItem("width",typeof(int))]
        public int width;
		[JSONItem("height",typeof(int))]
        public int height;
        public CameraResolutionClassParam(int pWidth, int pHeight)
        {
            width = pWidth;
            height = pHeight;
        }
    }
	[System.Serializable]
    public class BoolClassParam
    {
		[JSONItem("value", typeof(bool))]
        public bool value;
        public BoolClassParam(Boolean pValue)
        {
            value = pValue;
        }
    }
	[System.Serializable]
    public class StringClassParam
    {
		[JSONItem("value", typeof(String))]
        public String value;
        public StringClassParam(String pValue)
        {
            value = pValue;
        }
    }
    [Serializable]
    public class PairKeyValue
    {
		[JSONItem("key",typeof(string))]
        public string key;
		[JSONItem("value",typeof(string))]
        public string value;
        public PairKeyValue(string pKey, string pValue)
        {
            key = pKey;
            value = pValue;
        }
    }

	[Serializable]
	public class PairKeyValueArray
	{
		[JSONArray("list",typeof(PairKeyValue))]
		public PairKeyValue[] list;
		public PairKeyValueArray(PairKeyValue[] pList)
		{
			list = pList;
		}
	}

  [System.Serializable]
  public class AnswerClassParam {
    [JSONItem("callId",typeof(string))]
    public string callId;
    [JSONArray("headers",typeof(PairKeyValue))]
    public PairKeyValue[] headers;
    public AnswerClassParam(string pCallId, Dictionary<string, string> pHeader = null) {
      callId = pCallId;
      if (pHeader != null) {
        headers = Utils.GetDictionaryToArray(pHeader);
      }
    }
  }

  [System.Serializable]
  public class DeclineCallClassParam {
    [JSONItem("callId",typeof(string))]
    public string callId;
    [JSONArray("headers",typeof(PairKeyValue))]
    public PairKeyValue[] headers;
    public DeclineCallClassParam(string pCallId, Dictionary<string, string> pHeader = null) {
      callId = pCallId;
      if (pHeader != null) {
        headers = Utils.GetDictionaryToArray(pHeader);
      }
    }
  }

  [System.Serializable]
  public class HangupClassParam {
    [JSONItem("callId",typeof(string))]
    public string callId;
    [JSONArray("headers",typeof(PairKeyValue))]
    public PairKeyValue[] headers;
    public HangupClassParam(string pCallId, Dictionary<string, string> pHeader = null) {
      callId = pCallId;
      if (pHeader != null) {
        headers = Utils.GetDictionaryToArray(pHeader);
      }
    }
  }

  [System.Serializable]
  public class DisconnectCallClassParam {
    [JSONItem("callId",typeof(string))]
    public string callId;
    [JSONArray("headers",typeof(PairKeyValue))]
    public PairKeyValue[] headers;
    public DisconnectCallClassParam(string pCallId, Dictionary<string, string> pHeader = null) {
      callId = pCallId;
      if (pHeader != null) {
        headers = Utils.GetDictionaryToArray(pHeader);
      }
    }
  }
}

