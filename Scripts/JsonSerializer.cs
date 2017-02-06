#define PARSE_ESCAPED_UNICODE

#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_WII || UNITY_PS3 || UNITY_XBOX360 || UNITY_FLASH
#define USE_UNITY_DEBUGGING
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

#if PARSE_ESCAPED_UNICODE
using System.Text.RegularExpressions;
#endif

#if!USE_UNITY_DEBUGGING
using System.Diagnostics;
#endif

namespace Voximplant
{

	public static class Extensions
	{
		public static T Pop<T>(this List<T> list)
		{
			var result = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
			return result;
		}
	}

	static class JSONLogger
	{

		#if USE_UNITY_DEBUGGING
		public static void Log(string str)
		{
		#if USE_LOGGER
		Debug.Log(str);
		#endif
		}
		public static void Error(string str)
		{
		#if USE_LOGGER
		Debug.LogError(str);
		#endif
		}
		#else
		public static void Log(string str) {
		Debug.WriteLine(str);
		}
		public static void Error(string str) {
		Debug.WriteLine(str);
		}
		#endif

	}

	public enum JSONValueType
	{
		String,
		Number,
		Object,
		Array,
		Boolean,
		Null
	}

	public class JSONValue : System.IDisposable
	{
		#region IDisposable implementation

		public void Dispose () { }

		#endregion

		public JSONValue(JSONValueType type)
		{
			Type = type;
		}

		public JSONValue(string str)
		{
			Type = JSONValueType.String;
			Str = str;
		}

		public JSONValue(double number)
		{
			Type = JSONValueType.Number;
			Number = number;
		}

		public JSONValue(JSONObject obj)
		{
			if (obj == null)
			{
				Type = JSONValueType.Null;
			}
			else
			{
				Type = JSONValueType.Object;
				Obj = obj;
			}
		}

		public JSONValue(JSONArray array)
		{
			Type = JSONValueType.Array;
			Array = array;
		}

		public JSONValue(bool boolean)
		{
			Type = JSONValueType.Boolean;
			Boolean = boolean;
		}

		/// <summary>
		/// Construct a copy of the JSONValue given as a parameter
		/// </summary>
		/// <param name="value"></param>
		public JSONValue(JSONValue value)
		{
			Type = value.Type;
			switch (Type)
			{
			case JSONValueType.String:
				Str = value.Str;
				break;

			case JSONValueType.Boolean:
				Boolean = value.Boolean;
				break;

			case JSONValueType.Number:
				Number = value.Number;
				break;

			case JSONValueType.Object:
				if (value.Obj != null)
				{
					Obj = new JSONObject(value.Obj);
				}
				break;

			case JSONValueType.Array:
				Array = new JSONArray(value.Array);
				break;
			}
		}

		public JSONValueType Type { get; private set; }
		public string Str { get; set; }
		public double Number { get; set; }
		public JSONObject Obj { get; set; }
		public JSONArray Array { get; set; }
		public bool Boolean { get; set; }
		public JSONValue Parent { get; set; }

		public static implicit operator JSONValue(string str)
		{
			return new JSONValue(str);
		}

		public static implicit operator JSONValue(double number)
		{
			return new JSONValue(number);
		}

		public static implicit operator JSONValue(JSONObject obj)
		{
			return new JSONValue(obj);
		}

		public static implicit operator JSONValue(JSONArray array)
		{
			return new JSONValue(array);
		}

		public static implicit operator JSONValue(bool boolean)
		{
			return new JSONValue(boolean);
		}

		/// <returns>String representation of this JSONValue</returns>
		public override string ToString()
		{
			switch (Type)
			{
			case JSONValueType.Object:
				return Obj.ToString();

			case JSONValueType.Array:
				return Array.ToString();

			case JSONValueType.Boolean:
				return Boolean ? "true" : "false";

			case JSONValueType.Number:
				return Number.ToString();

			case JSONValueType.String:
				return "\"" + Str + "\"";

			case JSONValueType.Null:
				return "null";
			}
			return "null";
		}

	}

	public class JSONArray : IEnumerable<JSONValue>, System.IDisposable
	{
		#region IDisposable implementation

		public void Dispose () { }

		#endregion


		private readonly List<JSONValue> values = new List<JSONValue>();

		public JSONArray()
		{
		}

		/// <summary>
		/// Construct a new array and copy each value from the given array into the new one
		/// </summary>
		/// <param name="array"></param>
		public JSONArray(JSONArray array)
		{
			values = new List<JSONValue>();
			foreach (var v in array.values)
			{
				values.Add(new JSONValue(v));
			}
		}

		/// <summary>
		/// Add a JSONValue to this array
		/// </summary>
		/// <param name="value"></param>
		public void Add(JSONValue value)
		{
			values.Add(value);
		}

		public JSONValue this[int index]
		{
			get { return values[index]; }
			set { values[index] = value; }
		}

		/// <returns>
		/// Return the length of the array
		/// </returns>
		public int Length
		{
			get { return values.Count; }
		}

		/// <returns>String representation of this JSONArray</returns>
		public override string ToString()
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.Append('[');
			foreach (var value in values)
			{
				stringBuilder.Append(value.ToString());
				stringBuilder.Append(',');
			}
			if (values.Count > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}

		public IEnumerator<JSONValue> GetEnumerator()
		{
			return values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return values.GetEnumerator();
		}

		/// <summary>
		/// Attempt to parse a string as a JSON array.
		/// </summary>
		/// <param name="jsonString"></param>
		/// <returns>A new JSONArray object if successful, null otherwise.</returns>
		public static JSONArray Parse(string jsonString)
		{
			var tempObject = JSONObject.Parse("{ \"array\" :" + jsonString + '}');
			return tempObject == null ? null : tempObject.GetValue("array").Array;
		}

		/// <summary>
		/// Empty the array of all values.
		/// </summary>
		public void Clear()
		{
			values.Clear();
		}

		/// <summary>
		/// Remove the value at the given index, if it exists.
		/// </summary>
		/// <param name="index"></param>
		public void Remove(int index)
		{
			if (index >= 0 && index < values.Count)
			{
				values.RemoveAt(index);
			}
			else
			{
				JSONLogger.Error("index out of range: " + index + " (Expected 0 <= index < " + values.Count + ")");
			}
		}

		/// <summary>
		/// Concatenate two JSONArrays
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <returns>A new JSONArray that is the result of adding all of the right-hand side array's values to the left-hand side array.</returns>
		public static JSONArray operator +(JSONArray lhs, JSONArray rhs)
		{
			var result = new JSONArray(lhs);
			foreach (var value in rhs.values)
			{
				result.Add(value);
			}
			return result;
		}

	}

	public class JSONObject : IEnumerable<KeyValuePair<string, JSONValue>>, System.IDisposable
	{
		#region IDisposable implementation

		public void Dispose (){ }

		#endregion


		private enum JSONParsingState
		{
			Object,
			Array,
			EndObject,
			EndArray,
			Key,
			Value,
			KeyValueSeparator,
			ValueSeparator,
			String,
			Number,
			Boolean,
			Null
		}

		private readonly IDictionary<string, JSONValue> values = new Dictionary<string, JSONValue>();

		#if PARSE_ESCAPED_UNICODE
		private static readonly Regex unicodeRegex = new Regex(@"\\u([0-9a-fA-F]{4})");
		private static readonly byte[] unicodeBytes = new byte[2];
		#endif

		public JSONObject()
		{
		}

		/// <summary>
		/// Construct a copy of the given JSONObject.
		/// </summary>
		/// <param name="other"></param>
		public JSONObject(JSONObject other)
		{
			values = new Dictionary<string, JSONValue>();

			if (other != null)
			{
				foreach (var keyValuePair in other.values)
				{
					values[keyValuePair.Key] = new JSONValue(keyValuePair.Value);
				}
			}
		}

		/// <param name="key"></param>
		/// <returns>Does 'key' exist in this object.</returns>
		public bool ContainsKey(string key)
		{
			return values.ContainsKey(key);
		}

		public JSONValue GetValue(string key)
		{
			JSONValue value;
			values.TryGetValue(key, out value);
			return value;
		}

		public string GetString(string key)
		{
			var value = GetValue(key);
			if (value == null)
			{
				JSONLogger.Error(key + "(string) == null");
				return null;
			}
			return value.Str;
		}

		public double GetNumber(string key)
		{
			var value = GetValue(key);
			if (value == null)
			{
				JSONLogger.Error(key + " == null");
				return double.NaN;
			}
			return value.Number;
		}

		public JSONObject GetObject(string key)
		{
			var value = GetValue(key);
			if (value == null)
			{
				JSONLogger.Error(key + " == null");
				return null;
			}
			return value.Obj;
		}

		public bool GetBoolean(string key)
		{
			var value = GetValue(key);
			if (value == null)
			{
				JSONLogger.Error(key + " == null");
				return false;
			}
			if (value.Str == "true" || value.Str == "false")
			{
				return (value.Str == "true") ? true : false;
			}
			return value.Boolean;
		}

		public JSONArray GetArray(string key)
		{
			var value = GetValue(key);
			if (value == null)
			{
				JSONLogger.Error(key + " == null");
				return null;
			}
			return value.Array;
		}

		public JSONValue this[string key]
		{
			get { return GetValue(key); }
			set { values[key] = value; }
		}

		public void Add(string key, JSONValue value)
		{
			values[key] = value;
		}

		public void Add(KeyValuePair<string, JSONValue> pair)
		{
			values[pair.Key] = pair.Value;
		}

		/// <summary>
		/// Attempt to parse a string into a JSONObject.
		/// </summary>
		/// <param name="jsonString"></param>
		/// <returns>A new JSONObject or null if parsing fails.</returns>
		public static JSONObject Parse(string jsonString)
		{
			if (string.IsNullOrEmpty(jsonString))
			{
				return null;
			}

			JSONValue currentValue = null;

			var keyList = new List<string>();

			var state = JSONParsingState.Object;

			for (var startPosition = 0; startPosition < jsonString.Length; ++startPosition)
			{

				startPosition = SkipWhitespace(jsonString, startPosition);

				switch (state)
				{
				case JSONParsingState.Object:
					if (jsonString[startPosition] != '{')
					{
						return Fail('{', startPosition);
					}

					JSONValue newObj = new JSONObject();
					if (currentValue != null)
					{
						newObj.Parent = currentValue;
					}
					currentValue = newObj;

					state = JSONParsingState.Key;
					break;

				case JSONParsingState.EndObject:
					if (jsonString[startPosition] != '}')
					{
						return Fail('}', startPosition);
					}

					if (currentValue.Parent == null)
					{
						return currentValue.Obj;
					}

					switch (currentValue.Parent.Type)
					{

					case JSONValueType.Object:
						currentValue.Parent.Obj.values[keyList.Pop()] = new JSONValue(currentValue.Obj);
						break;

					case JSONValueType.Array:
						currentValue.Parent.Array.Add(new JSONValue(currentValue.Obj));
						break;

					default:
						return Fail("valid object", startPosition);

					}
					currentValue = currentValue.Parent;

					state = JSONParsingState.ValueSeparator;
					break;

				case JSONParsingState.Key:
					if (jsonString[startPosition] == '}')
					{
						--startPosition;
						state = JSONParsingState.EndObject;
						break;
					}

					var key = ParseString(jsonString, ref startPosition);
					if (key == null)
					{
						return Fail("key string", startPosition);
					}
					keyList.Add(key);
					state = JSONParsingState.KeyValueSeparator;
					break;

				case JSONParsingState.KeyValueSeparator:
					if (jsonString[startPosition] != ':')
					{
						return Fail(':', startPosition);
					}
					state = JSONParsingState.Value;
					break;

				case JSONParsingState.ValueSeparator:
					switch (jsonString[startPosition])
					{

					case ',':
						state = currentValue.Type == JSONValueType.Object ? JSONParsingState.Key : JSONParsingState.Value;
						break;

					case '}':
						state = JSONParsingState.EndObject;
						--startPosition;
						break;

					case ']':
						state = JSONParsingState.EndArray;
						--startPosition;
						break;

					default:
						return Fail(", } ]", startPosition);
					}
					break;

				case JSONParsingState.Value:
					{
						var c = jsonString[startPosition];
						if (c == '"')
						{
							state = JSONParsingState.String;
						}
						else if (char.IsDigit(c) || c == '-')
						{
							state = JSONParsingState.Number;
						}
						else
							switch (c)
						{

						case '{':
							state = JSONParsingState.Object;
							break;

						case '[':
							state = JSONParsingState.Array;
							break;

						case ']':
							if (currentValue.Type == JSONValueType.Array)
							{
								state = JSONParsingState.EndArray;
							}
							else
							{
								return Fail("valid array", startPosition);
							}
							break;

						case 'f':
						case 't':
							state = JSONParsingState.Boolean;
							break;


						case 'n':
							state = JSONParsingState.Null;
							break;

						default:
							return Fail("beginning of value", startPosition);
						}

						--startPosition; //To re-evaluate this char in the newly selected state
						break;
					}

				case JSONParsingState.String:
					var str = ParseString(jsonString, ref startPosition);
					if (str == null)
					{
						return Fail("string value", startPosition);
					}

					switch (currentValue.Type)
					{

					case JSONValueType.Object:
						currentValue.Obj.values[keyList.Pop()] = new JSONValue(str);
						break;

					case JSONValueType.Array:
						currentValue.Array.Add(str);
						break;

					default:
						JSONLogger.Error("Fatal error, current JSON value not valid");
						return null;
					}

					state = JSONParsingState.ValueSeparator;
					break;

				case JSONParsingState.Number:
					var number = ParseNumber(jsonString, ref startPosition);
					if (double.IsNaN(number))
					{
						return Fail("valid number", startPosition);
					}

					switch (currentValue.Type)
					{

					case JSONValueType.Object:
						currentValue.Obj.values[keyList.Pop()] = new JSONValue(number);
						break;

					case JSONValueType.Array:
						currentValue.Array.Add(number);
						break;

					default:
						JSONLogger.Error("Fatal error, current JSON value not valid");
						return null;
					}

					state = JSONParsingState.ValueSeparator;

					break;

				case JSONParsingState.Boolean:
					if (jsonString[startPosition] == 't')
					{
						if (jsonString.Length < startPosition + 4 ||
							jsonString[startPosition + 1] != 'r' ||
							jsonString[startPosition + 2] != 'u' ||
							jsonString[startPosition + 3] != 'e')
						{
							return Fail("true", startPosition);
						}

						switch (currentValue.Type)
						{

						case JSONValueType.Object:
							currentValue.Obj.values[keyList.Pop()] = new JSONValue(true);
							break;

						case JSONValueType.Array:
							currentValue.Array.Add(new JSONValue(true));
							break;

						default:
							JSONLogger.Error("Fatal error, current JSON value not valid");
							return null;
						}

						startPosition += 3;
					}
					else
					{
						if (jsonString.Length < startPosition + 5 ||
							jsonString[startPosition + 1] != 'a' ||
							jsonString[startPosition + 2] != 'l' ||
							jsonString[startPosition + 3] != 's' ||
							jsonString[startPosition + 4] != 'e')
						{
							return Fail("false", startPosition);
						}

						switch (currentValue.Type)
						{

						case JSONValueType.Object:
							currentValue.Obj.values[keyList.Pop()] = new JSONValue(false);
							break;

						case JSONValueType.Array:
							currentValue.Array.Add(new JSONValue(false));
							break;

						default:
							JSONLogger.Error("Fatal error, current JSON value not valid");
							return null;
						}

						startPosition += 4;
					}

					state = JSONParsingState.ValueSeparator;
					break;

				case JSONParsingState.Array:
					if (jsonString[startPosition] != '[')
					{
						return Fail('[', startPosition);
					}

					JSONValue newArray = new JSONArray();
					if (currentValue != null)
					{
						newArray.Parent = currentValue;
					}
					currentValue = newArray;

					state = JSONParsingState.Value;
					break;

				case JSONParsingState.EndArray:
					if (jsonString[startPosition] != ']')
					{
						return Fail(']', startPosition);
					}

					if (currentValue.Parent == null)
					{
						return currentValue.Obj;
					}

					switch (currentValue.Parent.Type)
					{

					case JSONValueType.Object:
						currentValue.Parent.Obj.values[keyList.Pop()] = new JSONValue(currentValue.Array);
						break;

					case JSONValueType.Array:
						currentValue.Parent.Array.Add(new JSONValue(currentValue.Array));
						break;

					default:
						return Fail("valid object", startPosition);
					}
					currentValue = currentValue.Parent;

					state = JSONParsingState.ValueSeparator;
					break;

				case JSONParsingState.Null:
					if (jsonString[startPosition] == 'n')
					{
						if (jsonString.Length < startPosition + 4 ||
							jsonString[startPosition + 1] != 'u' ||
							jsonString[startPosition + 2] != 'l' ||
							jsonString[startPosition + 3] != 'l')
						{
							return Fail("null", startPosition);
						}

						switch (currentValue.Type)
						{

						case JSONValueType.Object:
							currentValue.Obj.values[keyList.Pop()] = new JSONValue(JSONValueType.Null);
							break;

						case JSONValueType.Array:
							currentValue.Array.Add(new JSONValue(JSONValueType.Null));
							break;

						default:
							JSONLogger.Error("Fatal error, current JSON value not valid");
							return null;
						}

						startPosition += 3;
					}
					state = JSONParsingState.ValueSeparator;
					break;

				}
			}
			JSONLogger.Error("Unexpected end of string");
			return null;
		}

		private static int SkipWhitespace(string str, int pos)
		{
			for (; pos < str.Length && char.IsWhiteSpace(str[pos]); ++pos) ;
			return pos;
		}

		private static string ParseString(string str, ref int startPosition)
		{
			if (str[startPosition] != '"' || startPosition + 1 >= str.Length)
			{
				Fail('"', startPosition);
				return null;
			}

			var endPosition = str.IndexOf('"', startPosition + 1);
			if (endPosition <= startPosition)
			{
				Fail('"', startPosition + 1);
				return null;
			}

			while (str[endPosition - 1] == '\\')
			{
				endPosition = str.IndexOf('"', endPosition + 1);
				if (endPosition <= startPosition)
				{
					Fail('"', startPosition + 1);
					return null;
				}
			}

			var result = string.Empty;

			if (endPosition > startPosition + 1)
			{
				result = str.Substring(startPosition + 1, endPosition - startPosition - 1);
			}

			startPosition = endPosition;

			#if PARSE_ESCAPED_UNICODE
			// Parse Unicode characters that are escaped as \uXXXX
			do
			{
				Match m = unicodeRegex.Match(result);
				if (!m.Success)
				{
					break;
				}

				string s = m.Groups[1].Captures[0].Value;
				unicodeBytes[1] = byte.Parse(s.Substring(0, 2), NumberStyles.HexNumber);
				unicodeBytes[0] = byte.Parse(s.Substring(2, 2), NumberStyles.HexNumber);
				s = Encoding.Unicode.GetString(unicodeBytes);

				result = result.Replace(m.Value, s);
			} while (true);
			#endif

			return result;
		}

		private static double ParseNumber(string str, ref int startPosition)
		{
			if (startPosition >= str.Length || (!char.IsDigit(str[startPosition]) && str[startPosition] != '-'))
			{
				return double.NaN;
			}

			var endPosition = startPosition + 1;

			for (;
				endPosition < str.Length && str[endPosition] != ',' && str[endPosition] != ']' && str[endPosition] != '}';
				++endPosition) ;

			double result;
			if (
				!double.TryParse(str.Substring(startPosition, endPosition - startPosition), System.Globalization.NumberStyles.Float,
					System.Globalization.CultureInfo.InvariantCulture, out result))
			{
				return double.NaN;
			}
			startPosition = endPosition - 1;
			return result;
		}

		private static JSONObject Fail(char expected, int position)
		{
			return Fail(new string(expected, 1), position);
		}

		private static JSONObject Fail(string expected, int position)
		{
			JSONLogger.Error("Invalid json string, expecting " + expected + " at " + position);
			return null;
		}

		/// <returns>String representation of this JSONObject</returns>
		public override string ToString()
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.Append('{');

			foreach (var pair in values)
			{
				stringBuilder.Append("\"" + pair.Key + "\"");
				stringBuilder.Append(':');
				stringBuilder.Append(pair.Value.ToString());
				stringBuilder.Append(',');
			}
			if (values.Count > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}
		public IEnumerator<KeyValuePair<string, JSONValue>> GetEnumerator()
		{
			return values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return values.GetEnumerator();
		}

		/// <summary>
		/// Empty this JSONObject of all values.
		/// </summary>
		public void Clear()
		{
			values.Clear();
		}

		/// <summary>
		/// Remove the JSONValue attached to the given key.
		/// </summary>
		/// <param name="key"></param>
		public void Remove(string key)
		{
			if (values.ContainsKey(key))
			{
				values.Remove(key);
			}
		}
	}

	[System.Serializable]
	public class JSONClass : IDisposable
	{
		public void Dispose () { }
		public JSONClass() { }
	}

	internal enum AttrEnum{ None, AttrObject = 0, AttrItem = 1, AttrArray = 2 }
	[System.Serializable]
	[AttributeUsage(AttributeTargets.All)]
	public abstract class JSONAttribute : System.Attribute
	{
		internal readonly string data;
		internal readonly Type type;
		internal readonly AttrEnum attrType = AttrEnum.None;
		internal JSONAttribute(string data, Type type, AttrEnum attrType)
		{
			this.type = type;
			this.data = data;
			this.attrType = attrType;
		}
	}
	public class JSONObjectAttribute : JSONAttribute
	{
		public JSONObjectAttribute(string data, Type type) : base(data, type, AttrEnum.AttrObject) { }	
	}

	public class JSONItemAttribute : JSONAttribute
	{
		public JSONItemAttribute(string data, Type type) : base(data, type, AttrEnum.AttrItem) { }
	}

	public class JSONArrayAttribute : JSONAttribute
	{
		public JSONArrayAttribute(string data, Type type) : base(data, type, AttrEnum.AttrArray) { }
	}

	public class JSONSerialize
	{
		public static object Deserialize(Type rootType, string json)
		{
			object rootObject = Activator.CreateInstance(rootType);
			JSONObject jsonObject = JSONObject.Parse(json); // parse JSON file into JSONObject
			if (json == null)
			{
				return null;
			}
			// Iterate all members of root object
			foreach (var member in rootType.GetMembers())
			{
				// Iterate through all fields with JSONAttribute from root object
				JSONAttribute[] attrFields = (JSONAttribute[])member.GetCustomAttributes(typeof(JSONAttribute), true);

				foreach (JSONAttribute attrField in attrFields)
				{
					Type attrFieldType = attrField.type;    // Get the type of the field
					string data = attrField.data;           // Get the data name of the field
					FieldInfo info = rootType.GetField(data); // Create a connection with the field
					if (info == null)                   // If no field next (probably wrong design of the class)
					{
						continue;
					}
					AttrEnum value = attrField.attrType;
					// Type is either object, item or array.
					switch(value)
					{
					case AttrEnum.AttrObject: 
						JSONObject jsonChildObject = jsonObject.GetObject(data);
						if(jsonChildObject == null)
						{
							continue;
						}
						info.SetValue(rootObject, Deserialize(attrFieldType, jsonObject.GetObject(data).ToString()));
						break;
					case AttrEnum.AttrItem:
						SetJSONItem(attrFieldType, jsonObject, data, info, rootObject);
						break;
					case AttrEnum.AttrArray:
						JSONArray jsonArray = jsonObject.GetArray(data);
						if (jsonArray == null) 
						{
							continue;
						}
						object o = Array.CreateInstance(attrFieldType, jsonArray.Length);
						SetJSONArray(attrFieldType, jsonArray, o);
						info.SetValue(rootObject, o);
						break;
					}
				}
			}
			return rootObject;
		}
		public static T Deserialize<T>(string json) where T : class, new()
		{
			return (T)Deserialize(typeof(T), json);
		}

		public static string GetJsonFile(string jsonFile, string key)
		{
			if(CheckParameters(jsonFile, key) == false)
			{
				return null;
			}
			return null;

		}

		private static bool CheckParameters(string jsonFile, string key)
		{
			if(jsonFile == null || key == null)
			{
				return false;
			}
			JSONObject jsonObject = JSONObject.Parse (jsonFile);
			if(jsonObject == null)
			{
				return false;
			}
			return true;
		}
		public static void Serialize(string path, object obj)
		{
			using (TextWriter text = new StreamWriter(path))
			{
				JSONObject jsonObject = new JSONObject();
				Serialize( obj, jsonObject);
				text.Write(jsonObject.ToString());
			}
		}

		public static void Serialize(TextWriter text, object obj) 
		{
			JSONObject jsonObject = new JSONObject();
			Serialize( obj, jsonObject);
			text.Write(jsonObject.ToString());
		}

		public static void Serialize(object obj, JSONObject jsonObject)
		{
			Type rootType = obj.GetType();
			foreach (var member in rootType.GetMembers())
			{
				// Iterate through all fields with JSONAttribute from root object
				JSONAttribute[] attrFields = (JSONAttribute[])member.GetCustomAttributes(typeof(JSONAttribute), true);
				foreach (JSONAttribute attrField in attrFields)
				{

					string data = attrField.data;           // Get the data name of the field
					FieldInfo info = rootType.GetField(data); // Create a connection with the field
					if (info == null)                   // If no field next (probably wrong design of the class)
					{
						continue;
					}  

					AttrEnum attrType = attrField.attrType;    // Get the type of the attribute
					// Type is either object, item or array.
					switch(attrType)
					{
					case AttrEnum.AttrObject:
						{
							JSONObject jsonChild = new JSONObject();
							jsonObject.Add(data, jsonChild);
							Serialize( info.GetValue(obj), jsonChild);
							break;
						}

					case AttrEnum.AttrItem:
						{
							object val = info.GetValue(obj);
							string result = val.ToString();
							Type attrFieldType = attrField.type; 
							if (attrFieldType == typeof(Color))
							{
								Color col = (Color)val;
								result = ((int)(col.r * 255f)).ToString() +","
									+((int)(col.g * 255f)).ToString() +","
									+((int)(col.b * 255f)).ToString() +","
									+((int)(col.a * 255f)).ToString();
							}
							else if(attrFieldType == typeof(Vector2))
							{
								Vector2 v = (Vector2)val;
								result = v.x.ToString()+","+ v.y.ToString();
							}
							else if(attrFieldType == typeof(Vector3))
							{
								Vector3 v = (Vector3)val;
								result = v.x.ToString()+","+ v.y.ToString()+","+v.z.ToString();
							}
							else if(attrFieldType == typeof(Vector4))
							{
								Vector4 v = (Vector4)val;
								result = v.x.ToString()+","+ v.y.ToString()+","+v.z.ToString() + v.z.ToString();
							}
							JSONValue value = new JSONValue(result);
							jsonObject.Add(data, value);
							break;
						}
					case AttrEnum.AttrArray:
						{
							object[] val = (object[])info.GetValue(obj);
							JSONArray jsonArray = new JSONArray();
							jsonObject.Add(data, jsonArray);
							if (val != null)
							{
								foreach (object v in val) 
								{
									JSONObject jsonChild = new JSONObject();
									Serialize( v, jsonChild);
									jsonArray.Add(jsonChild);
								}
							}
							break;
						}
					}
				}
			}
		}

		// If type is item, it has to be int, float, string, boolean
		private static void SetJSONItem(Type attrFieldType, JSONObject jsonObject, string data, FieldInfo info, object rootObject)
		{
			if(attrFieldType.IsEnum)
			{
				string valueStr = jsonObject.GetString(data);
				if (valueStr != null)
				{
					try
					{
						object result = Enum.Parse (attrFieldType, valueStr);
						info.SetValue(rootObject, result);
					}catch(Exception) { }
					return;
				}
			}
			else if (attrFieldType == typeof(int))
			{
				string valueStr = jsonObject.GetString(data);
				Debug.Log (valueStr);
				if (valueStr != null)
				{
					int intResult = 0;
					if (int.TryParse(valueStr, out intResult) == true)
					{
						info.SetValue(rootObject, intResult);
						return;
					}
				}
				JSONValue value = jsonObject.GetValue(data);
				if (value != null) { info.SetValue(rootObject, (int)value.Number); }

			}
			else if (attrFieldType == typeof(string))
			{
				string value = jsonObject.GetString(data);

				if (value != null)
				{
					info.SetValue(rootObject, value);
				}
			}
			else if (attrFieldType == typeof(float))
			{
				string valueStr = jsonObject.GetString(data);
				if (valueStr != null)
				{
					float floatResult = 0.0f;

					if (float.TryParse(valueStr, out floatResult) == true)
					{
						info.SetValue(rootObject, floatResult);
						return;
					}
				}
				JSONValue value = jsonObject.GetValue(data);
				if (value != null) 
				{ 
					info.SetValue(rootObject, (float)value.Number); return; 
				}
			}
			else if (attrFieldType == typeof(bool))
			{
				bool value = jsonObject.GetBoolean(data);
				info.SetValue(rootObject, value);
			}
			else if (attrFieldType == typeof(Color))
			{
				string value = jsonObject.GetString(data);
				if(value == null)
				{
					return;
				}
				Color color = Color.white;
				if(OnlyHexInString(value) == true)
				{
					color = HexToColor(value);
					color = color / 255f;
				}
				else
				{
					float [] f = SetVector(value, 4);
					color = new Color(f[0] / 255f, f[1] / 255f, f[2] / 255f, f[3] / 255f);
				}
				info.SetValue(rootObject, color);
			}
			else if(attrFieldType == typeof(Vector2))
			{
				string value = jsonObject.GetString(data);
				if(value == null)
				{
					return;
				}
				float [] f = SetVector(value, 2);
				Vector2 v = new Vector2(f[0], f[1]);
				info.SetValue(rootObject, v);
			}
			else if(attrFieldType == typeof(Vector3))
			{
				string value = jsonObject.GetString(data);
				if(value == null)
				{
					return;
				}
				float [] f = SetVector(value, 3);
				Vector3 v = new Vector3(f[0], f[1], f[2]);
				info.SetValue(rootObject, v);
			}
			else if(attrFieldType == typeof(Vector4))
			{
				string value = jsonObject.GetString(data);
				if(value == null)
				{
					return;
				}
				float [] f = SetVector(value, 4);
				Vector4 v = new Vector4(f[0], f[1], f[2], f[3]);
				info.SetValue(rootObject, v);
			}
		}
		private static float [] SetVector(string value, int count)
		{
			string [] splits = value.Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries);
			float [] results = new float[count]; 
			for(int i = 0; i < count; i++)
			{
				try
				{
					results[i] = float.Parse(splits[i]);
				}
				catch(Exception)
				{
					continue;
				}
			}
			return results;
		}
		private static bool OnlyHexInString(string test)
		{
			// For C-style hex notation (0xFF) you can use @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z"
			return System.Text.RegularExpressions.Regex.IsMatch(test, @"\A\b[0-9a-fA-F]+\b\Z");
		}
		private static void SetJSONArray(Type attrFieldType, JSONArray jsonArray, object o)
		{
			int length = jsonArray.Length;
			object[] objs = (object[])o;
			for (int i = 0; i < length; i++)
			{
				JSONObject obj = jsonArray[i].Obj;
				objs[i] = Deserialize(attrFieldType, obj.ToString());
			}
		}

		private static Color HexToColor(string hex)
		{
			byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
			try
			{
				byte a = byte.Parse(hex.Substring(6,2), System.Globalization.NumberStyles.HexNumber);
				return new Color(r,g,b,a);
			}
			catch(Exception)
			{
				return new Color(r,g,b, 255);
			}
		}
		private static string ColorToHex(Color color)
		{
			string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
			return hex;
		}
	}



}

