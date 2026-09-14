using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.FireworkSystem;

public class FireworkSpecService : ILoadableSingleton
{
	private readonly ISpecService _specService;

	private ImmutableArray<string> _fireworkIds;

	private FrozenDictionary<string, FireworkSpec> _fireworkSpecsById;

	public FireworkSpecService(ISpecService specService)
	{
		_specService = specService;
	}

	public void Load()
	{
		_fireworkSpecsById = _specService.GetSpecs<FireworkSpec>().ToFrozenDictionary((FireworkSpec spec) => spec.FireworkId);
		_fireworkIds = (from spec in _fireworkSpecsById.Values
			orderby spec.DisplayName.Value
			select spec.FireworkId).ToImmutableArray();
	}

	public ImmutableArray<string> GetFireworkIds()
	{
		return _fireworkIds;
	}

	public FireworkSpec GetFireworkSpec(string id)
	{
		if (!_fireworkSpecsById.TryGetValue(id, out var value))
		{
			throw new KeyNotFoundException("Firework spec for id '" + id + "' not found.");
		}
		return value;
	}

	public bool HasSpec(string id)
	{
		return _fireworkSpecsById.ContainsKey(id);
	}
}
