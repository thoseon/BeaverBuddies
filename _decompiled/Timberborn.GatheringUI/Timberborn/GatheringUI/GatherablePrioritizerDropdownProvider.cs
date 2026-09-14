using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.DropdownSystem;
using Timberborn.EntitySystem;
using Timberborn.Gathering;
using Timberborn.GoodsUI;
using Timberborn.Localization;
using UnityEngine;

namespace Timberborn.GatheringUI;

internal class GatherablePrioritizerDropdownProvider : BaseComponent, IAwakableComponent, IInitializableEntity, IExtendedDropdownProvider, IDropdownProvider
{
	private static readonly string NothingItemLocKey = "Gathering.Nothing";

	private readonly GoodDescriber _goodDescriber;

	private readonly ILoc _loc;

	private GatherablePrioritizer _gatherablePrioritizer;

	private GathererFlag _gathererFlag;

	private readonly List<string> _items = new List<string>();

	public IReadOnlyList<string> Items => _items.AsReadOnlyList();

	public bool HasMultipleOptions => _gathererFlag.AllowedGatherables.Length > 1;

	public GatherablePrioritizerDropdownProvider(GoodDescriber goodDescriber, ILoc loc)
	{
		_goodDescriber = goodDescriber;
		_loc = loc;
	}

	public void Awake()
	{
		_gatherablePrioritizer = GetComponent<GatherablePrioritizer>();
		_gathererFlag = GetComponent<GathererFlag>();
	}

	public void InitializeEntity()
	{
		ImmutableArray<GatherableSpec> allowedGatherables = _gathererFlag.AllowedGatherables;
		_items.Add(_loc.T(NothingItemLocKey));
		_items.AddRange(allowedGatherables.Select(GatherableGoodName));
	}

	public string GetValue()
	{
		if (!(_gatherablePrioritizer.PrioritizedGatherable != null))
		{
			return _loc.T(NothingItemLocKey);
		}
		return GatherableGoodName(_gatherablePrioritizer.PrioritizedGatherable);
	}

	public void SetValue(string value)
	{
		GatherableSpec prioritizedGatherable = GetPrioritizedGatherable(value);
		_gatherablePrioritizer.PrioritizeGatherable(prioritizedGatherable);
	}

	public string FormatDisplayText(string value, bool selected)
	{
		return value;
	}

	public Sprite GetIcon(string value)
	{
		GatherableSpec prioritizedGatherable = GetPrioritizedGatherable(value);
		if (prioritizedGatherable != null)
		{
			string id = prioritizedGatherable.Yielder.Yield.Id;
			return _goodDescriber.GetIcon(id);
		}
		return null;
	}

	public ImmutableArray<string> GetItemClasses(string value)
	{
		return ImmutableArray<string>.Empty;
	}

	private string GatherableGoodName(GatherableSpec gatherableSpec)
	{
		return _goodDescriber.Describe(gatherableSpec.Yielder.Yield.Id);
	}

	private GatherableSpec GetPrioritizedGatherable(string value)
	{
		return _gathererFlag.AllowedGatherables.SingleOrDefault((GatherableSpec gatherable) => GatherableGoodName(gatherable) == value);
	}
}
