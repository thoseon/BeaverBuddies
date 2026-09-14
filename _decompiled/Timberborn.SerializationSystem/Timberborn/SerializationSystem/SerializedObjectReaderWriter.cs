using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Timberborn.SerializationSystem;

public class SerializedObjectReaderWriter
{
	private readonly JsonMerger _jsonMerger;

	public SerializedObjectReaderWriter(JsonMerger jsonMerger)
	{
		_jsonMerger = jsonMerger;
	}

	public void WriteJson(SerializedObject serializedObject, Stream stream)
	{
		JObject jObject = SerializeObject(serializedObject);
		try
		{
			using StreamWriter textWriter = new StreamWriter(stream);
			using JsonTextWriter writer = new JsonTextWriter(textWriter);
			jObject.WriteTo(writer);
		}
		catch (Exception ex)
		{
			throw new IOException(ex.Message, ex);
		}
	}

	public string WriteJson(SerializedObject serializedObject)
	{
		return SerializeObject(serializedObject).ToString();
	}

	public SerializedObject ReadJson(Stream stream)
	{
		using StreamReader streamReader = new StreamReader(stream);
		string text = streamReader.ReadToEnd().Replace("\":-.,", "\":0.0,");
		return ReadJson(text);
	}

	public SerializedObject ReadJson(string text)
	{
		return DeserializeObject(JObject.Parse(text));
	}

	public SerializedObject ReadJsons(IEnumerable<string> texts)
	{
		return DeserializeObject(_jsonMerger.Merge(texts.Select(JObject.Parse)));
	}

	private JObject SerializeObject(SerializedObject serializedObject)
	{
		JObject jObject = new JObject();
		foreach (string item in serializedObject.Properties())
		{
			object serialized = serializedObject.GetSerialized(item);
			jObject.Add(item, SerializeAnything(serialized));
		}
		return jObject;
	}

	private SerializedObject DeserializeObject(JToken jToken)
	{
		if (jToken.Type != JTokenType.Object)
		{
			throw new ArgumentException($"{jToken} is not a JProperty, can't deserialize it.");
		}
		SerializedObject serializedObject = new SerializedObject();
		foreach (JProperty item in jToken.Children<JProperty>())
		{
			object obj = DeserializeAnything(item.Value);
			if (obj is Array values)
			{
				serializedObject.SetArray(item.Name, values);
			}
			else
			{
				serializedObject.Set(item.Name, obj);
			}
		}
		return serializedObject;
	}

	private JToken SerializeAnything(object value)
	{
		if (!(value is SerializedObject serializedObject))
		{
			if (value is Array array)
			{
				return SerializeArray(array);
			}
			return SerializaBasicType(value);
		}
		return SerializeObject(serializedObject);
	}

	private object DeserializeAnything(JToken jToken)
	{
		return jToken.Type switch
		{
			JTokenType.Array => DeserializeArray(jToken), 
			JTokenType.Object => DeserializeObject(jToken), 
			_ => DeserializeBasicType(jToken), 
		};
	}

	private JArray SerializeArray(Array array)
	{
		JArray jArray = new JArray();
		foreach (object item in array)
		{
			jArray.Add(SerializeAnything(item));
		}
		return jArray;
	}

	private object[] DeserializeArray(JToken jToken)
	{
		if (jToken.Type != JTokenType.Array)
		{
			throw new ArgumentException($"Argument is not a {JTokenType.Array}.");
		}
		object[] array = new object[jToken.Children().Count()];
		int num = 0;
		foreach (JToken item in jToken.Children())
		{
			array[num] = DeserializeAnything(item);
			num++;
		}
		return array;
	}

	private static JToken SerializaBasicType(object value)
	{
		if (value != null)
		{
			if (!(value is int num))
			{
				if (!(value is float value2))
				{
					if (!(value is bool value3))
					{
						if (value is string value4)
						{
							return new JValue(value4);
						}
						throw new ArgumentException($"Can't create JToken from {value} of type {value.GetType()}");
					}
					return new JValue(value3);
				}
				return new JValue(value2);
			}
			return new JValue(num);
		}
		return null;
	}

	private static object DeserializeBasicType(JToken jToken)
	{
		return jToken.Type switch
		{
			JTokenType.Null => null, 
			JTokenType.Integer => jToken.Value<int>(), 
			JTokenType.Float => jToken.Value<float>(), 
			JTokenType.Boolean => jToken.Value<bool>(), 
			JTokenType.String => jToken.Value<string>(), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
