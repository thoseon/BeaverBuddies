using System;
using Timberborn.SerializationSystem;
using UnityEngine;

namespace Timberborn.KeyBindingSystem;

public class CustomInputBindingSerializer
{
	private static readonly string PathKey = "Path";

	private static readonly string InputModifiersKey = "InputModifiers";

	private static readonly string InputModifiersValueKey = "Value";

	private static readonly string DefaultNameKey = "DefaultName";

	private static readonly string InputBindingKey = "InputBindingSpecification";

	private readonly SerializedObjectReaderWriter _serializedObjectReaderWriter;

	public CustomInputBindingSerializer(SerializedObjectReaderWriter serializedObjectReaderWriter)
	{
		_serializedObjectReaderWriter = serializedObjectReaderWriter;
	}

	public string Serialize(CustomInputBinding customInputBinding)
	{
		SerializedObject serializedObject = new SerializedObject();
		serializedObject.Set(PathKey, customInputBinding.Path);
		SerializedObject serializedObject2 = new SerializedObject();
		serializedObject2.Set(InputModifiersValueKey, customInputBinding.InputModifiers.ToString());
		serializedObject.Set(InputModifiersKey, serializedObject2);
		if (!string.IsNullOrEmpty(customInputBinding.DefaultName))
		{
			serializedObject.Set(DefaultNameKey, customInputBinding.DefaultName);
		}
		SerializedObject serializedObject3 = new SerializedObject();
		serializedObject3.Set(InputBindingKey, serializedObject);
		return _serializedObjectReaderWriter.WriteJson(serializedObject3);
	}

	public CustomInputBinding Deserialize(string spec)
	{
		try
		{
			SerializedObject serializedObject = _serializedObjectReaderWriter.ReadJson(spec).Get<SerializedObject>(InputBindingKey);
			return new CustomInputBinding(serializedObject.Get<string>(PathKey), serializedObject.Get<SerializedObject>(InputModifiersKey).Get<InputModifiers>(InputModifiersValueKey), serializedObject.Has(DefaultNameKey) ? serializedObject.Get<string>(DefaultNameKey) : string.Empty);
		}
		catch (Exception arg)
		{
			Debug.LogWarning($"Exception while trying to deserialize:\n{spec}\n\n{arg}");
			return null;
		}
	}
}
