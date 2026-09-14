using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.SerializationSystem;
using UnityEngine;

namespace Timberborn.BlueprintSystem;

public class BlueprintDeserializer
{
	private static readonly string ChildrenProperty = "Children";

	private readonly BasicDeserializer _basicDeserializer;

	private readonly AdvancedDeserializer _advancedDeserializer;

	private readonly SerializedObjectReaderWriter _serializedObjectReaderWriter;

	private readonly BlueprintFileBundleLoader _blueprintFileBundleLoader;

	private readonly SpecTypeCache _specTypeCache = SpecTypeCache.Create();

	public BlueprintDeserializer(BasicDeserializer basicDeserializer, AdvancedDeserializer advancedDeserializer, SerializedObjectReaderWriter serializedObjectReaderWriter, BlueprintFileBundleLoader blueprintFileBundleLoader)
	{
		_basicDeserializer = basicDeserializer;
		_advancedDeserializer = advancedDeserializer;
		_serializedObjectReaderWriter = serializedObjectReaderWriter;
		_blueprintFileBundleLoader = blueprintFileBundleLoader;
	}

	public IEnumerable<Type> GetSpecTypes(SerializedObject serializedObject)
	{
		foreach (string item in serializedObject.Properties())
		{
			if (_specTypeCache.TryGetType(item, out var type))
			{
				yield return type;
			}
		}
	}

	public Blueprint DeserializeSafe(BlueprintFileBundle blueprintFileBundle)
	{
		return DeserializeRoot(blueprintFileBundle, safe: true);
	}

	public Blueprint DeserializeUnsafe(BlueprintFileBundle blueprintFileBundle)
	{
		try
		{
			return DeserializeRoot(blueprintFileBundle, safe: false);
		}
		catch (Exception)
		{
			Debug.LogError("Failed to deserialize blueprint " + blueprintFileBundle.Path);
			throw;
		}
	}

	private Blueprint DeserializeRoot(BlueprintFileBundle blueprintFileBundle, bool safe)
	{
		SerializedObject serializedObject = FileBundleToSerializedObject(blueprintFileBundle);
		ImmutableArray<string> nestedBlueprints = ImmutableArray.Create(blueprintFileBundle.Path);
		return DeserializeBlueprint(serializedObject, safe, nestedBlueprints, blueprintFileBundle.Name);
	}

	private SerializedObject FileBundleToSerializedObject(BlueprintFileBundle blueprintFileBundle)
	{
		return _serializedObjectReaderWriter.ReadJsons(blueprintFileBundle.Jsons);
	}

	private Blueprint DeserializeBlueprint(SerializedObject serializedObject, bool safe, ImmutableArray<string> nestedBlueprints, string name)
	{
		return new Blueprint(name, DeserializeSpecs(serializedObject, safe), DeserializeChildren(serializedObject, safe, nestedBlueprints));
	}

	private IEnumerable<ComponentSpec> DeserializeSpecs(SerializedObject serializedObject, bool safe)
	{
		foreach (string item in serializedObject.Properties())
		{
			if (!(item != ChildrenProperty))
			{
				continue;
			}
			SerializedObject serializedObject2 = serializedObject.Get<SerializedObject>(item);
			if (_specTypeCache.TryGetType(item, out var type))
			{
				yield return DeserializeSpec(serializedObject2, type);
				continue;
			}
			if (safe)
			{
				string content = _serializedObjectReaderWriter.WriteJson(serializedObject2);
				yield return new NonExistingSpec
				{
					SpecName = item,
					Content = content
				};
				continue;
			}
			throw new ArgumentException("No type found for key " + item);
		}
	}

	private ComponentSpec DeserializeSpec(SerializedObject serializedObject, Type type)
	{
		object instance = _basicDeserializer.Deserialize(serializedObject, type);
		return (ComponentSpec)_advancedDeserializer.Deserialize(instance, type);
	}

	private ImmutableArray<Blueprint> DeserializeChildren(SerializedObject serializedObject, bool safe, ImmutableArray<string> nestedBlueprints)
	{
		List<Blueprint> list = new List<Blueprint>();
		if (serializedObject.Has(ChildrenProperty))
		{
			SerializedObject serializedObject2 = serializedObject.Get<SerializedObject>(ChildrenProperty);
			foreach (string item in serializedObject2.Properties())
			{
				SerializedObject serializedObject3 = serializedObject2.Get<SerializedObject>(item);
				list.Add(item.Contains(JsonKeywords.Nested) ? DeserializeNestedBlueprint(serializedObject3, safe, nestedBlueprints, item) : DeserializeBlueprint(serializedObject3, safe, nestedBlueprints, item));
			}
		}
		return list.ToImmutableArray();
	}

	private Blueprint DeserializeNestedBlueprint(SerializedObject serializedObject, bool safe, ImmutableArray<string> nestedBlueprints, string name)
	{
		string text = serializedObject.Get<string>(NestedBlueprintSpec.BlueprintPathKey);
		if (nestedBlueprints.Contains(text))
		{
			throw new InvalidOperationException("Circular reference detected in nested Blueprints: " + string.Join("->", nestedBlueprints) + "->" + text);
		}
		Blueprint originalBlueprint = DeserializeNestedBlueprint(serializedObject, safe, nestedBlueprints, text, name);
		return AddNestedBlueprintSpec(originalBlueprint, serializedObject, text);
	}

	private Blueprint DeserializeNestedBlueprint(SerializedObject serializedObject, bool safe, ImmutableArray<string> nestedBlueprints, string blueprintPath, string name)
	{
		BlueprintFileBundle bundle = _blueprintFileBundleLoader.GetBundle(blueprintPath);
		if (bundle == null)
		{
			if (safe)
			{
				return new Blueprint(name, ImmutableArray.Create<ComponentSpec>(), ImmutableArray<Blueprint>.Empty);
			}
			throw new InvalidOperationException("Blueprint bundle not found for path: " + blueprintPath);
		}
		return DeserializeNestedBlueprintUnwrapped(serializedObject, bundle, safe, nestedBlueprints.Add(blueprintPath), name);
	}

	private Blueprint DeserializeNestedBlueprintUnwrapped(SerializedObject serializedObject, BlueprintFileBundle blueprintFileBundle, bool safe, ImmutableArray<string> nestedBlueprints, string name)
	{
		BlueprintFileBundle blueprintFileBundle2 = (serializedObject.Has(NestedBlueprintSpec.ModificationKey) ? blueprintFileBundle.AddJson(GetModificationJson(serializedObject), "Modifying") : blueprintFileBundle);
		SerializedObject serializedObject2 = FileBundleToSerializedObject(blueprintFileBundle2);
		return DeserializeBlueprint(serializedObject2, safe, nestedBlueprints, name);
	}

	private Blueprint AddNestedBlueprintSpec(Blueprint originalBlueprint, SerializedObject serializedObject, string blueprintPath)
	{
		string modification = (serializedObject.Has(NestedBlueprintSpec.ModificationKey) ? GetModificationJson(serializedObject) : string.Empty);
		NestedBlueprintSpec newSpec = new NestedBlueprintSpec
		{
			BlueprintPath = blueprintPath,
			Modification = modification
		};
		return new Blueprint(originalBlueprint, null, newSpec);
	}

	private string GetModificationJson(SerializedObject serializedObject)
	{
		return _serializedObjectReaderWriter.WriteJson(serializedObject.Get<SerializedObject>(NestedBlueprintSpec.ModificationKey));
	}
}
