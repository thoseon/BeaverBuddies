using Timberborn.MapRepositorySystem;
using UnityEngine;

namespace Timberborn.MapItemsUI;

public class MapItem
{
	public MapFileReference MapFileReference { get; }

	public string DisplayName { get; }

	public string DisplayDescription { get; }

	public Vector2Int? Size { get; }

	public bool IsRecommended { get; }

	public bool IsUnconventional { get; }

	public bool IsDeletable { get; }

	public bool IsDev { get; }

	public MapIcon MapIcon { get; }

	public MapItem(MapFileReference mapFileReference, string displayName, string displayDescription, Vector2Int? size, bool isRecommended, bool isUnconventional, bool isDeletable, bool isDev, MapIcon mapIcon)
	{
		MapFileReference = mapFileReference;
		DisplayName = displayName;
		DisplayDescription = displayDescription;
		Size = size;
		IsRecommended = isRecommended;
		IsUnconventional = isUnconventional;
		IsDeletable = isDeletable;
		IsDev = isDev;
		MapIcon = mapIcon;
	}
}
