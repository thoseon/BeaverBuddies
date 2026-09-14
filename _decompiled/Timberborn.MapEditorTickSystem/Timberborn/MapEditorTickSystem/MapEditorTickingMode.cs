using System;
using System.Reflection;
using Timberborn.TickSystem;

namespace Timberborn.MapEditorTickSystem;

internal class MapEditorTickingMode : ITickingMode
{
	private static readonly Type AttributeType = typeof(MapEditorTickableAttribute);

	public bool SingletonIsActiveInThisMode(object singleton)
	{
		return singleton.GetType().GetCustomAttribute(AttributeType) != null;
	}
}
