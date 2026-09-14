using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.DropdownSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using UnityEngine;

namespace Timberborn.AutomationUI;

public class TransmitterDropdownProvider : BaseComponent, IExtendedDropdownProvider, IDropdownProvider
{
	private static readonly ImmutableArray<string> NoneDropdownItemClasses = ImmutableArray.Create("dropdown-item--none");

	private readonly AutomatorRegistry _automatorRegistry;

	private readonly ILoc _loc;

	private readonly Func<Automator> _getter;

	private readonly Action<Automator> _setter;

	private readonly string _noneLocKey;

	private readonly string _selectedNoneLocKey;

	private readonly List<string> _itemCache = new List<string>();

	public IReadOnlyList<string> Items
	{
		get
		{
			_itemCache.Clear();
			_itemCache.Add(string.Empty);
			_itemCache.AddRange(_automatorRegistry.SortedTransmitterIds);
			return _itemCache.AsReadOnlyList();
		}
	}

	public TransmitterDropdownProvider(AutomatorRegistry automatorRegistry, ILoc loc, Func<Automator> getter, Action<Automator> setter, string noneLocKey, string selectedNoneLocKey)
	{
		_automatorRegistry = automatorRegistry;
		_loc = loc;
		_getter = getter;
		_setter = setter;
		_noneLocKey = noneLocKey;
		_selectedNoneLocKey = selectedNoneLocKey;
	}

	public string GetValue()
	{
		Automator automator = _getter();
		if (!automator)
		{
			return "";
		}
		return automator.GetComponent<EntityComponent>().EntityId.ToString();
	}

	public void SetValue(string value)
	{
		_setter(string.IsNullOrEmpty(value) ? null : _automatorRegistry.FindTransmitterById(Guid.Parse(value)));
	}

	public string FormatDisplayText(string value, bool selected)
	{
		if (!string.IsNullOrEmpty(value))
		{
			return _automatorRegistry.FindTransmitterById(Guid.Parse(value)).AutomatorName;
		}
		return _loc.T(selected ? _selectedNoneLocKey : _noneLocKey);
	}

	public Sprite GetIcon(string value)
	{
		return null;
	}

	public ImmutableArray<string> GetItemClasses(string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			return ImmutableArray<string>.Empty;
		}
		return NoneDropdownItemClasses;
	}
}
