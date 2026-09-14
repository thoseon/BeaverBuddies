using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.MapItemsUI;
using Timberborn.MapRepositorySystem;
using Timberborn.MapThumbnail;
using Timberborn.WonderCompletion;
using UnityEngine.UIElements;

namespace Timberborn.MapRepositorySystemUI;

public class SelectedMapPanel
{
	private static readonly string NoDescriptionLocKey = "MapSelection.NoDescription";

	private readonly ILoc _loc;

	private readonly MapThumbnailCache _mapThumbnailCache;

	private readonly WonderCompletionService _wonderCompletionService;

	private Label _mapName;

	private Label _description;

	private Image _thumbnail;

	private VisualElement _recommendedMap;

	private VisualElement _unconventionalMap;

	private VisualElement _flexibleStart;

	private bool _showFlexibleStartInfo;

	public SelectedMapPanel(ILoc loc, MapThumbnailCache mapThumbnailCache, WonderCompletionService wonderCompletionService)
	{
		_loc = loc;
		_mapThumbnailCache = mapThumbnailCache;
		_wonderCompletionService = wonderCompletionService;
	}

	public void InitializeWithFlexibleStartInfoShown(VisualElement root)
	{
		Initialize(root, showFlexibleStartInfo: true);
	}

	public void InitializeWithFlexibleStartInfoHidden(VisualElement root)
	{
		Initialize(root, showFlexibleStartInfo: false);
	}

	public void Open()
	{
		_mapThumbnailCache.Clear();
	}

	public void Update(MapItem mapItem)
	{
		SetDescription(mapItem);
		_mapName.text = mapItem.DisplayName;
		_thumbnail.image = _mapThumbnailCache.GetThumbnail(mapItem.MapFileReference);
		_recommendedMap.ToggleDisplayStyle(mapItem.IsRecommended);
		_unconventionalMap.ToggleDisplayStyle(mapItem.IsUnconventional);
		_flexibleStart.ToggleDisplayStyle(_showFlexibleStartInfo && IsMapGoalUnlocked(mapItem));
	}

	public void ClearSelection()
	{
		_mapName.text = string.Empty;
		_description.text = string.Empty;
		_thumbnail.image = null;
		_recommendedMap.ToggleDisplayStyle(visible: false);
		_unconventionalMap.ToggleDisplayStyle(visible: false);
		_flexibleStart.ToggleDisplayStyle(visible: false);
	}

	private void Initialize(VisualElement root, bool showFlexibleStartInfo)
	{
		_showFlexibleStartInfo = showFlexibleStartInfo;
		_mapName = root.Q<Label>("MapName");
		_description = root.Q<Label>("DescriptionText");
		_thumbnail = root.Q<Image>("ThumbnailImage");
		_recommendedMap = root.Q<VisualElement>("RecommendedMap");
		_unconventionalMap = root.Q<VisualElement>("UnconventionalMap");
		_flexibleStart = root.Q<VisualElement>("FlexibleStart");
	}

	private void SetDescription(MapItem mapItem)
	{
		bool flag = string.IsNullOrEmpty(mapItem.DisplayDescription);
		_description.text = (flag ? _loc.T(NoDescriptionLocKey) : mapItem.DisplayDescription);
	}

	private bool IsMapGoalUnlocked(MapItem mapItem)
	{
		MapFileReference mapFileReference = mapItem.MapFileReference;
		return _wonderCompletionService.IsWonderCompletedWithAnyFaction(mapFileReference.Name, mapFileReference.Resource);
	}
}
