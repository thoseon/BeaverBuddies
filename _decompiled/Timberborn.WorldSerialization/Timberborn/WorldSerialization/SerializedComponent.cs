using System;
using Timberborn.SerializationSystem;

namespace Timberborn.WorldSerialization;

public class SerializedComponent : IEquatable<SerializedComponent>
{
	public string Name { get; }

	public SerializedObject Value { get; }

	public SerializedComponent(string name)
		: this(name, new SerializedObject())
	{
	}

	public SerializedComponent(string name, SerializedObject value)
	{
		Name = name;
		Value = value;
	}

	public bool Equals(SerializedComponent other)
	{
		if (other == null)
		{
			return false;
		}
		if (Name == other.Name)
		{
			return Value.Equals(other.Value);
		}
		return false;
	}
}
