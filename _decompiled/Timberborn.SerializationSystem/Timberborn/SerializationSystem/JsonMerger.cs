using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Timberborn.SerializationSystem;

public class JsonMerger
{
	private class ArrayModification
	{
		public JProperty JProperty { get; }

		public string Path { get; }

		public string ParentPath { get; }

		public ArrayModification(JProperty jProperty, string path, string parentPath)
		{
			JProperty = jProperty;
			Path = path;
			ParentPath = parentPath;
		}
	}

	private static readonly JsonMergeSettings ReplaceSettings = new JsonMergeSettings
	{
		MergeArrayHandling = MergeArrayHandling.Replace
	};

	private readonly List<ArrayModification> _arrayAppenders = new List<ArrayModification>();

	private readonly List<ArrayModification> _arrayRemovals = new List<ArrayModification>();

	private readonly List<string> _propertyDeletions = new List<string>();

	public JObject Merge(IEnumerable<JObject> jsons)
	{
		JObject jObject = null;
		foreach (JObject json in jsons)
		{
			jObject = ProcessAndMergeJsons(jObject, json);
		}
		return jObject;
	}

	private JObject ProcessAndMergeJsons(JObject baseJson, JObject json)
	{
		if (baseJson != null)
		{
			ProcessTokens(baseJson);
		}
		ProcessTokens(json);
		JObject jObject = MergeJsons(baseJson, json);
		ProcessArrayAppenders(jObject);
		ProcessArrayRemovals(jObject);
		ProcessPropertyDeletions(jObject);
		_arrayAppenders.Clear();
		_arrayRemovals.Clear();
		_propertyDeletions.Clear();
		return jObject;
	}

	private void ProcessTokens(JObject json)
	{
		ProcessArrayTokens(json, JsonKeywords.Append, _arrayAppenders);
		ProcessArrayTokens(json, JsonKeywords.Remove, _arrayRemovals);
		ProcessDeleteTokens(json, JsonKeywords.Delete);
	}

	private static void ProcessArrayTokens(JToken parentToken, string keyword, ICollection<ArrayModification> arrayModifications)
	{
		foreach (JProperty item in parentToken.Children<JProperty>().ToList())
		{
			if (item.Name.EndsWith(JsonKeywords.Nested, StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}
			if (item.Name.EndsWith(keyword, StringComparison.OrdinalIgnoreCase))
			{
				if (item.Value.Type != JTokenType.Array)
				{
					throw new ArgumentException("Cannot modify non-array property: " + item.Path + " of " + parentToken.Path);
				}
				CreateArrayModification(item, keyword, arrayModifications);
			}
			else if (item.Value.Type == JTokenType.Object)
			{
				ProcessArrayTokens(item.Value, keyword, arrayModifications);
			}
		}
	}

	private void ProcessDeleteTokens(JToken parentToken, string keyword)
	{
		foreach (JProperty item2 in parentToken.Children<JProperty>().ToList())
		{
			if (!item2.Name.EndsWith(JsonKeywords.Nested, StringComparison.OrdinalIgnoreCase))
			{
				if (item2.Name.EndsWith(keyword, StringComparison.OrdinalIgnoreCase))
				{
					string item = item2.Path.Replace(keyword, string.Empty, StringComparison.OrdinalIgnoreCase);
					_propertyDeletions.Add(item);
					item2.Remove();
				}
				else if (item2.Value.Type == JTokenType.Object)
				{
					ProcessDeleteTokens(item2.Value, keyword);
				}
			}
		}
	}

	private static void CreateArrayModification(JProperty modifyingProperty, string keyword, ICollection<ArrayModification> arrayModifications)
	{
		string path = modifyingProperty.Path.Replace(keyword, string.Empty, StringComparison.OrdinalIgnoreCase);
		string parentPath = ((modifyingProperty.Parent == null) ? string.Empty : modifyingProperty.Parent.Path);
		modifyingProperty.Remove();
		JToken value = modifyingProperty.Value;
		modifyingProperty.Value = JValue.CreateNull();
		JProperty jProperty = new JProperty(modifyingProperty.Name.Replace(keyword, string.Empty, StringComparison.OrdinalIgnoreCase), value);
		arrayModifications.Add(new ArrayModification(jProperty, path, parentPath));
	}

	private static JObject MergeJsons(JObject mergedJson, JObject json)
	{
		if (mergedJson == null)
		{
			return json;
		}
		mergedJson.Merge(json, ReplaceSettings);
		return mergedJson;
	}

	private void ProcessArrayAppenders(JContainer mergedJson)
	{
		foreach (ArrayModification arrayAppender in _arrayAppenders)
		{
			JToken jToken = mergedJson.SelectToken(arrayAppender.Path);
			if (jToken != null)
			{
				AddToExistingArrayProperty(jToken, arrayAppender.JProperty);
			}
			else
			{
				AddNewArrayProperty(mergedJson, arrayAppender);
			}
		}
		_arrayAppenders.Clear();
	}

	private static void AddToExistingArrayProperty(JToken existingArray, JProperty arrayAppender)
	{
		if (existingArray.Type != JTokenType.Array)
		{
			throw new ArgumentException("Cannot append to non-array path: " + existingArray.Path);
		}
		JArray jArray = (JArray)existingArray;
		foreach (JToken jTokenToAppend in arrayAppender.Value.Children())
		{
			if (!jArray.Any((JToken t) => JToken.DeepEquals(t, jTokenToAppend)))
			{
				jArray.Add(jTokenToAppend);
			}
		}
	}

	private static void AddNewArrayProperty(JContainer mergedJson, ArrayModification arrayModification)
	{
		if (string.IsNullOrEmpty(arrayModification.ParentPath))
		{
			mergedJson.Add(arrayModification.JProperty);
		}
		else if (mergedJson.SelectToken(arrayModification.ParentPath) is JObject jObject)
		{
			jObject.Add(arrayModification.JProperty);
		}
		else
		{
			Debug.LogWarning("Parent path not found: " + arrayModification.ParentPath + ". Was probably removed by other json.");
		}
	}

	private void ProcessArrayRemovals(JToken mergedJson)
	{
		foreach (ArrayModification arrayRemoval in _arrayRemovals)
		{
			JToken jToken = mergedJson.SelectToken(arrayRemoval.Path);
			if (jToken != null)
			{
				if (jToken.Type != JTokenType.Array)
				{
					throw new ArgumentException("Cannot remove from non-array path: " + jToken.Path);
				}
				RemoveFromExistingArrayProperty((JArray)jToken, arrayRemoval.JProperty);
			}
		}
		_arrayRemovals.Clear();
	}

	private static void RemoveFromExistingArrayProperty(JArray existingArray, JProperty arrayRemoval)
	{
		foreach (JToken item in arrayRemoval.Value.Children())
		{
			for (int i = 0; i < existingArray.Count; i++)
			{
				if (JToken.DeepEquals(existingArray[i], item))
				{
					existingArray.RemoveAt(i);
					break;
				}
			}
		}
	}

	private void ProcessPropertyDeletions(JToken mergedJson)
	{
		foreach (string propertyDeletion in _propertyDeletions)
		{
			JToken jToken = mergedJson.SelectToken(propertyDeletion);
			if (jToken != null)
			{
				if (jToken.Parent is JProperty jProperty)
				{
					jProperty.Remove();
				}
				else if (jToken is JProperty jProperty2)
				{
					jProperty2.Remove();
				}
				else
				{
					Debug.LogWarning("Cannot delete path (not a property): " + propertyDeletion);
				}
			}
		}
		_propertyDeletions.Clear();
	}
}
