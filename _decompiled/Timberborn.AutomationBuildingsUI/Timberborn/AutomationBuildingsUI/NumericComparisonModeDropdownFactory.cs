using System;
using System.Collections.Generic;
using Timberborn.AssetSystem;
using Timberborn.AutomationBuildings;
using Timberborn.DropdownSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.AutomationBuildingsUI;

public class NumericComparisonModeDropdownFactory : ILoadableSingleton
{
	private static readonly string IconPath = "Sprites/Comparison/Comparison";

	private readonly EnumDropdownProviderFactory _enumDropdownProviderFactory;

	private readonly IAssetLoader _assetLoader;

	private readonly Dictionary<string, Sprite> _icons = new Dictionary<string, Sprite>();

	public NumericComparisonModeDropdownFactory(EnumDropdownProviderFactory enumDropdownProviderFactory, IAssetLoader assetLoader)
	{
		_enumDropdownProviderFactory = enumDropdownProviderFactory;
		_assetLoader = assetLoader;
	}

	public void Load()
	{
		foreach (object value in Enum.GetValues(typeof(NumericComparisonMode)))
		{
			string text = value.ToString();
			_icons[text] = _assetLoader.Load<Sprite>(IconPath + "." + text);
		}
	}

	public EnumDropdownProvider<NumericComparisonMode> Create(Func<NumericComparisonMode> getter, Action<NumericComparisonMode> setter)
	{
		return _enumDropdownProviderFactory.CreateWithIcon(getter, setter, (string _) => string.Empty, GetIcon);
	}

	private Sprite GetIcon(string mode)
	{
		return _icons[mode];
	}
}
