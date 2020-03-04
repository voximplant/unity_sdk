/*
 *  Copyright (c) 2011-2020, Zingaya, Inc. All rights reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Voximplant.Unity.@internal
{
    internal static class JsonHelper
    {
        public static IReadOnlyList<T> FromJson<T>(string jsonString)
        {
            var json = "{\"array\": " + jsonString + "}";
            var holder = JsonUtility.FromJson<Wrapper<T>>(json);
            return holder.array;
        }

        private static string Sanitize(string input)
        {
            return input.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        public static string ToJson([CanBeNull] IDictionary<string, string> dictionary)
        {
            if (dictionary == null)
            {
                return "{}";
            }

            var pairs = dictionary.Select(pair => $"\"{Sanitize(pair.Key)}\":\"{Sanitize(pair.Value)}\"").ToList();
            return $"{{{string.Join(",", pairs)}}}";
        }

        public static IReadOnlyDictionary<string, string> FromList(IList<string> list)
        {
            var result = new Dictionary<string, string>();
            for (var i = 0; i < list.Count; i += 2)
            {
                result.Add(list[i], list[i + 1]);
            }

            return result;
        }

        [Serializable]
        private class Wrapper<T>
        {
            [SerializeField]
            public T[] array = default;
        }
    }
}
