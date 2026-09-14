using UnityEngine;

namespace Timberborn.MapItemsUI;

public class MapIcon
{
	public Sprite Icon { get; }

	public string TooltipLocKey { get; }

	public MapIcon(Sprite icon, string tooltipLocKey)
	{
		Icon = icon;
		TooltipLocKey = tooltipLocKey;
	}
}
