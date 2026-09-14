using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Timberborn.SerializationSystem;

public class JsonDiffer
{
	public string GenerateDiffJson(string originalJson, string modifiedJson)
	{
		JToken? jToken = JsonConvert.DeserializeObject<JToken>(originalJson);
		JToken jToken2 = JsonConvert.DeserializeObject<JToken>(modifiedJson);
		JObject jObject = DiffObject(jToken as JObject, jToken2 as JObject);
		if (jObject != null)
		{
			return JsonConvert.SerializeObject(jObject, Formatting.Indented);
		}
		return string.Empty;
	}

	private static JObject DiffObject(JObject original, JObject modified)
	{
		JObject jObject = new JObject();
		foreach (JProperty item in modified.Properties())
		{
			JProperty jProperty = original.Property(item.Name);
			if (jProperty == null)
			{
				jObject[item.Name] = item.Value;
				continue;
			}
			JToken jToken = DiffToken(jProperty.Value, item.Value);
			if (jToken == null)
			{
				continue;
			}
			if (jProperty.Value.Type == JTokenType.Array)
			{
				foreach (JProperty item2 in ((JObject)jToken).Properties())
				{
					jObject[item.Name + item2.Name] = item2.Value;
				}
			}
			else
			{
				jObject[item.Name] = jToken;
			}
		}
		foreach (JProperty item3 in original.Properties())
		{
			if (modified.Property(item3.Name) == null)
			{
				jObject[item3.Name + JsonKeywords.Delete] = new JObject();
			}
		}
		if (!jObject.HasValues)
		{
			return null;
		}
		return jObject;
	}

	private static JToken DiffToken(JToken original, JToken modified)
	{
		if (original.Type != modified.Type)
		{
			return modified;
		}
		switch (original.Type)
		{
		case JTokenType.Object:
			return DiffObject(original as JObject, modified as JObject);
		case JTokenType.Array:
			return DiffArray(original as JArray, modified as JArray);
		default:
			if (!JToken.DeepEquals(original, modified))
			{
				return modified;
			}
			return null;
		}
	}

	private static JObject DiffArray(JArray original, JArray modified)
	{
		JObject jObject = new JObject();
		AddRemovedItems(original, modified, jObject);
		AddAppendedItems(original, modified, jObject);
		if (!jObject.HasValues)
		{
			return null;
		}
		return jObject;
	}

	private static void AddRemovedItems(JArray original, JArray modified, JObject diff)
	{
		HashSet<string> first = new HashSet<string>(original.Select((JToken x) => x.ToString(Formatting.None)));
		HashSet<string> second = new HashSet<string>(modified.Select((JToken x) => x.ToString(Formatting.None)));
		List<string> source = first.Except(second).ToList();
		if (source.Any())
		{
			diff[JsonKeywords.Remove] = new JArray(source.Select(JToken.Parse));
		}
	}

	private static void AddAppendedItems(JArray original, JArray modified, JObject diff)
	{
		List<JToken> list = modified.ToList();
		foreach (JToken item in original)
		{
			for (int i = 0; i < list.Count; i++)
			{
				JToken jToken = list[i];
				if (item.Type == jToken.Type && JToken.DeepEquals(item, jToken))
				{
					list.RemoveAt(i);
					break;
				}
			}
		}
		if (list.Any())
		{
			diff[JsonKeywords.Append] = new JArray(list);
		}
	}
}
