using UnityEngine;

namespace Timberborn.GoodsUI;

public readonly struct DescribedGood
{
	public string DisplayName { get; }

	public Sprite Icon { get; }

	public DescribedGood(string displayName, Sprite icon)
	{
		DisplayName = displayName;
		Icon = icon;
	}
}
