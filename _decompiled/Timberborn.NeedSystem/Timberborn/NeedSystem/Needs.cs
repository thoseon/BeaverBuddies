using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.NeedSpecs;

namespace Timberborn.NeedSystem;

public class Needs
{
	private readonly Dictionary<string, Need> _needDictionary = new Dictionary<string, Need>();

	private readonly Need[] _needArray;

	public ImmutableArray<Need> AllNeeds { get; }

	public Needs(IEnumerable<Need> needs)
	{
		List<Need> list = needs.ToList();
		_needArray = new Need[list.Count];
		int num = 0;
		foreach (Need item in list)
		{
			_needDictionary[item.NeedSpec.Id] = item;
			_needArray[num++] = item;
		}
		AllNeeds = ((IEnumerable<Need>)_needArray).ToImmutableArray();
	}

	public bool TryGetNeed(string needId, out Need need)
	{
		return _needDictionary.TryGetValue(needId, out need);
	}

	public bool TryGetBackwardCompatibleNeed(string needId, out Need need)
	{
		Need need2 = _needArray.SingleOrDefault((Need n) => n.NeedSpec.BackwardCompatibleIds.Contains(needId));
		if (need2 != null)
		{
			need = need2;
			return true;
		}
		need = null;
		return false;
	}

	public NeedSpec GetNeedSpec(string needId)
	{
		if (!_needDictionary.TryGetValue(needId, out var value))
		{
			return null;
		}
		return value.NeedSpec;
	}

	public bool Has(string needId)
	{
		return _needDictionary.ContainsKey(needId);
	}

	public bool Any(Predicate<Need> predicate)
	{
		for (int i = 0; i < _needArray.Length; i++)
		{
			if (predicate(_needArray[i]))
			{
				return true;
			}
		}
		return false;
	}
}
