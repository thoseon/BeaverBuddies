using System.Collections.Generic;

namespace Timberborn.BlueprintSystem;

public class BlueprintSourceService
{
	private readonly Dictionary<Blueprint, BlueprintFileBundle> _sources = new Dictionary<Blueprint, BlueprintFileBundle>();

	public void Add(Blueprint blueprint, BlueprintFileBundle source)
	{
		_sources[blueprint] = source;
	}

	public BlueprintFileBundle Get(Blueprint blueprint)
	{
		return _sources[blueprint];
	}
}
