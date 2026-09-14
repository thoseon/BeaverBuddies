using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.Common;
using Timberborn.DropdownSystem;
using Timberborn.SteamWorkshop;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.SteamWorkshopUI;

public class VisibilityDropdownProvider : IExtendedDropdownProvider, IDropdownProvider
{
	private static readonly string[] VisibilityNames = Enum.GetNames(typeof(SteamWorkshopVisibility));

	private Toggle _updateVisibilityToggle;

	public SteamWorkshopVisibility CurrentValue { get; private set; }

	public IReadOnlyList<string> Items => VisibilityNames;

	public void Initialize(Toggle updateVisibilityToggle)
	{
		Asserts.FieldIsNull(this, _updateVisibilityToggle, "_updateVisibilityToggle");
		_updateVisibilityToggle = updateVisibilityToggle;
	}

	public string GetValue()
	{
		return CurrentValue.ToString();
	}

	public void SetInitialValue(SteamWorkshopVisibility value)
	{
		CurrentValue = value;
	}

	public void SetValue(string value)
	{
		SteamWorkshopVisibility currentValue = CurrentValue;
		CurrentValue = Enum.Parse<SteamWorkshopVisibility>(value);
		if (currentValue != CurrentValue)
		{
			_updateVisibilityToggle.value = true;
		}
	}

	public string FormatDisplayText(string value, bool selected)
	{
		return "SteamWorkshop.Visibility." + value;
	}

	public Sprite GetIcon(string value)
	{
		return null;
	}

	public ImmutableArray<string> GetItemClasses(string value)
	{
		return ImmutableArray<string>.Empty;
	}
}
