using System;
using Timberborn.BlueprintSystem;
using Timberborn.Localization;

namespace Timberborn.LocalizationSerialization;

internal class LocalizedTextDeserializer : IDeserializer
{
	private readonly ILoc _loc;

	public Type DeserializedType => typeof(LocalizedText);

	public LocalizedTextDeserializer(ILoc loc)
	{
		_loc = loc;
	}

	public object Deserialize(object source)
	{
		string text = (string)source;
		if (!string.IsNullOrWhiteSpace(text))
		{
			return new LocalizedText(_loc.T(text));
		}
		return null;
	}
}
