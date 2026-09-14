using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.FactionSystem;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using Timberborn.WonderCompletion;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.MapItemsUI;

public class MapItemElementFactory
{
	private static readonly string WonderCompletedLocKey = "WonderCompletion.WonderCompleted";

	private static readonly string FlexibleStartUnlockedLocKey = "FlexibleStart.Unlocked";

	private static readonly string FlexibleStartNeededLocKey = "FlexibleStart.Needed";

	private static readonly string RecommendedLocKey = "MapSelection.Recommended";

	private static readonly string UnconventionalLocKey = "MapSelection.Unconventional";

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly WonderCompletionService _wonderCompletionService;

	private readonly FactionSpecService _factionSpecService;

	private readonly MapItemFactionIconFactory _mapItemFactionIconFactory;

	private readonly ILoc _loc;

	private readonly Dictionary<VisualElement, string> _iconTooltipLocKeys = new Dictionary<VisualElement, string>();

	private readonly StringBuilder _tooltipBuilder = new StringBuilder();

	public MapItemElementFactory(ITooltipRegistrar tooltipRegistrar, VisualElementLoader visualElementLoader, WonderCompletionService wonderCompletionService, FactionSpecService factionSpecService, MapItemFactionIconFactory mapItemFactionIconFactory, ILoc loc)
	{
		_tooltipRegistrar = tooltipRegistrar;
		_visualElementLoader = visualElementLoader;
		_wonderCompletionService = wonderCompletionService;
		_factionSpecService = factionSpecService;
		_mapItemFactionIconFactory = mapItemFactionIconFactory;
		_loc = loc;
	}

	public VisualElement Create()
	{
		VisualElement item = _visualElementLoader.LoadVisualElement("Common/MapItemElement");
		_tooltipRegistrar.Register(item.Q<VisualElement>("Icon"), () => GetIconTooltipLocKey(item));
		_tooltipRegistrar.Register(item.Q<VisualElement>("Recommended"), () => _loc.T(RecommendedLocKey));
		_tooltipRegistrar.Register(item.Q<VisualElement>("Unconventional"), () => _loc.T(UnconventionalLocKey));
		return item;
	}

	public void Bind(VisualElement item, MapItem mapItem, bool showMapGoals)
	{
		if (showMapGoals)
		{
			CreateFactionIcons(item, mapItem);
		}
		item.Q<Label>("MapName").text = mapItem.DisplayName;
		item.Q<Label>("MapSize").text = GetDisplaySize(mapItem.Size);
		item.Q<Image>("Recommended").ToggleDisplayStyle(mapItem.IsRecommended);
		item.Q<Image>("Unconventional").ToggleDisplayStyle(mapItem.IsUnconventional);
		Image image = item.Q<Image>("Icon");
		if (mapItem.MapIcon != null)
		{
			image.sprite = mapItem.MapIcon.Icon;
			image.ToggleDisplayStyle(visible: true);
			_iconTooltipLocKeys[item] = mapItem.MapIcon.TooltipLocKey;
		}
		else
		{
			image.ToggleDisplayStyle(visible: false);
		}
	}

	public void Clear()
	{
		_iconTooltipLocKeys.Clear();
	}

	private void CreateFactionIcons(VisualElement item, MapItem mapItem)
	{
		IEnumerable<string> completedWonderFactionIds = _wonderCompletionService.GetWonderCompletionFactionIds(mapItem.MapFileReference.Name, mapItem.MapFileReference.Resource);
		List<FactionSpec> validFactions = _factionSpecService.Factions.Where((FactionSpec faction) => completedWonderFactionIds.Contains(faction.Id)).ToList();
		VisualElement visualElement = item.Q<VisualElement>("FactionsList");
		visualElement.Clear();
		foreach (FactionSpec item2 in validFactions.OrderByDescending((FactionSpec faction) => faction.Order))
		{
			visualElement.Add(_mapItemFactionIconFactory.Create(item2));
		}
		if (visualElement.childCount == 0)
		{
			visualElement.Add(_mapItemFactionIconFactory.CreateEmpty());
		}
		_tooltipRegistrar.Register(visualElement, () => GetFactionIconsTooltipText(validFactions));
	}

	private string GetFactionIconsTooltipText(IEnumerable<FactionSpec> factionSpecs)
	{
		_tooltipBuilder.Clear();
		foreach (FactionSpec factionSpec in factionSpecs)
		{
			string text = _loc.T(WonderCompletedLocKey, factionSpec.DisplayName.Value);
			_tooltipBuilder.AppendLine(SpecialStrings.RowStarter + text);
		}
		if (_tooltipBuilder.Length > 0)
		{
			_tooltipBuilder.AppendLine(SpecialStrings.RowStarter + _loc.T(FlexibleStartUnlockedLocKey));
			return _tooltipBuilder.ToStringWithoutNewLineEnd();
		}
		return _loc.T(FlexibleStartNeededLocKey);
	}

	private static string GetDisplaySize(Vector2Int? size)
	{
		if (!size.HasValue)
		{
			return string.Empty;
		}
		return $"{size.Value.x}{SpecialStrings.SizeSeparator}{size.Value.y}";
	}

	private string GetIconTooltipLocKey(VisualElement item)
	{
		string text = _iconTooltipLocKeys[item];
		if (!string.IsNullOrEmpty(text))
		{
			return _loc.T(text);
		}
		return null;
	}
}
