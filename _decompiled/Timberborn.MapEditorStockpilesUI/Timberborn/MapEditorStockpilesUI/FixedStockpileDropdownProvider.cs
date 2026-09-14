using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.DropdownSystem;
using Timberborn.EntitySystem;
using Timberborn.EntityUndoSystem;
using Timberborn.InventorySystem;
using Timberborn.Stockpiles;
using UnityEngine;

namespace Timberborn.MapEditorStockpilesUI;

internal class FixedStockpileDropdownProvider : BaseComponent, IAwakableComponent, IInitializableEntity, IExtendedTooltipDropdownProvider, IExtendedDropdownProvider, IDropdownProvider
{
	private readonly EntityChangeRecorderFactory _entityChangeRecorderFactory;

	private readonly FixedStockpileGoodProvider _fixedStockpileGoodProvider;

	private FixedStockpileInventorySetter _fixedStockpileInventorySetter;

	private SingleGoodAllower _singleGoodAllower;

	private Stockpile _stockpile;

	public IReadOnlyList<string> Items { get; private set; }

	public FixedStockpileDropdownProvider(EntityChangeRecorderFactory entityChangeRecorderFactory, FixedStockpileGoodProvider fixedStockpileGoodProvider)
	{
		_entityChangeRecorderFactory = entityChangeRecorderFactory;
		_fixedStockpileGoodProvider = fixedStockpileGoodProvider;
	}

	public void Awake()
	{
		_fixedStockpileInventorySetter = GetComponent<FixedStockpileInventorySetter>();
		_singleGoodAllower = GetComponent<SingleGoodAllower>();
		_stockpile = GetComponent<Stockpile>();
	}

	public void InitializeEntity()
	{
		Items = _fixedStockpileGoodProvider.GetGoods(_stockpile.WhitelistedGoodType);
	}

	public string GetValue()
	{
		return _singleGoodAllower.AllowedGood;
	}

	public void SetValue(string goodId)
	{
		using (_entityChangeRecorderFactory.CreateChangeRecorder(_fixedStockpileInventorySetter))
		{
			_fixedStockpileInventorySetter.SetGoodId(goodId);
		}
	}

	public string FormatDisplayText(string goodId, bool selected)
	{
		return _fixedStockpileGoodProvider.GetDisplayText(goodId);
	}

	public Sprite GetIcon(string goodId)
	{
		return _fixedStockpileGoodProvider.GetIcon(goodId);
	}

	public ImmutableArray<string> GetItemClasses(string value)
	{
		return ImmutableArray<string>.Empty;
	}

	public string GetDropdownTooltip(string value)
	{
		return _fixedStockpileGoodProvider.GetTooltip(value);
	}
}
