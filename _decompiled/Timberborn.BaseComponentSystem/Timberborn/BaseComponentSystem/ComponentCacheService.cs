using System.Collections.Generic;

namespace Timberborn.BaseComponentSystem;

internal class ComponentCacheService
{
	private static readonly int MinimumComponentsCount = 25;

	private readonly Dictionary<string, int> _components = new Dictionary<string, int>();

	public int GetComponentsCount(string name)
	{
		return _components.GetValueOrDefault(name, MinimumComponentsCount);
	}

	public void SaveComponentsCount(string name, int count)
	{
		if (!_components.TryGetValue(name, out var value) || count > value)
		{
			_components[name] = count;
		}
	}
}
