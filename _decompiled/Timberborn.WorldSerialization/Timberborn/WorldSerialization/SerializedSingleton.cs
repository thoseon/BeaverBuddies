using Timberborn.SerializationSystem;

namespace Timberborn.WorldSerialization;

public class SerializedSingleton
{
	public string Name { get; }

	public SerializedObject Value { get; }

	public SerializedSingleton(string name)
		: this(name, new SerializedObject())
	{
	}

	public SerializedSingleton(string name, SerializedObject value)
	{
		Name = name;
		Value = value;
	}
}
