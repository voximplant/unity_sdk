using System.Collections.Generic;
using SimpleJSON;

namespace Voximplant
{
    internal class Utils
    {
        public static JSONNode GetParamList(string p)
        {
            JSONNode rootNode = JSON.Parse(p);
            return rootNode;
        }

        public static PairKeyValue[] GetDictionaryToArray(Dictionary<string, string> pDic)
        {
            if (pDic == null)
                return new PairKeyValue[0];

            PairKeyValue[] list = new PairKeyValue[pDic.Count];
            int i = 0;
            foreach (KeyValuePair<string, string> pair in pDic)
            {
                list[i] = new PairKeyValue(pair.Key, pair.Value);
                i += 1;
            }
            return list;
        }

        public static string ToJson(object obj)
        {
            if (obj == null)
                return null;

            JSONObject jsonObject = new JSONObject();
            JSONSerialize.Serialize(obj, jsonObject);
            return jsonObject.ToString();
        }
    }

}