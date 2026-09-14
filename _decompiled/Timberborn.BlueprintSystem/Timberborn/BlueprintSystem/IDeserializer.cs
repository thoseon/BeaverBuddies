using System;

namespace Timberborn.BlueprintSystem;

public interface IDeserializer
{
	Type DeserializedType { get; }

	object Deserialize(object source);
}
