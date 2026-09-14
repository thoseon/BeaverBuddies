using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.AutomationBuildings;
using Timberborn.BaseComponentSystem;
using Timberborn.DropdownSystem;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.GoodsUI;
using UnityEngine;

namespace Timberborn.AutomationBuildingsUI;

internal class ResourceCounterGoodsDropdownProvider : BaseComponent, IAwakableComponent, IInitializableEntity, IExtendedTooltipDropdownProvider, IExtendedDropdownProvider, IDropdownProvider
{
	private readonly IGoodService _goodService;

	private readonly GoodDescriber _goodDescriber;

	private ResourceCounter _resourceCounter;

	public IReadOnlyList<string> Items { get; private set; }

	public ResourceCounterGoodsDropdownProvider(IGoodService goodService, GoodDescriber goodDescriber)
	{
		_goodService = goodService;
		_goodDescriber = goodDescriber;
	}

	public void Awake()
	{
		_resourceCounter = GetComponent<ResourceCounter>();
	}

	public void InitializeEntity()
	{
		Items = _goodService.Goods.OrderBy((string good) => FormatDisplayText(good, selected: false)).ToImmutableArray();
	}

	public string GetValue()
	{
		return _resourceCounter.GoodId;
	}

	public void SetValue(string goodId)
	{
		_resourceCounter.SetGoodId(goodId);
	}

	public string FormatDisplayText(string goodId, bool selected)
	{
		return _goodService.GetGood(goodId).DisplayName.Value;
	}

	public Sprite GetIcon(string goodId)
	{
		return _goodDescriber.GetIcon(goodId);
	}

	public ImmutableArray<string> GetItemClasses(string value)
	{
		return ImmutableArray<string>.Empty;
	}

	public string GetDropdownTooltip(string value)
	{
		return FormatDisplayText(value, selected: false);
	}
}
