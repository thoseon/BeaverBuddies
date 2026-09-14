using System.Collections.Generic;
using System.Collections.Immutable;

namespace Timberborn.SteamWorkshop;

public class SteamWorkshopItem
{
	public ulong ItemId { get; }

	public string Name { get; }

	public string Visibility { get; }

	public bool UpdateDescription { get; }

	public bool UpdateVisibility { get; }

	public bool UpdatePreview { get; }

	public bool UpdateTags { get; }

	public ImmutableArray<string> Tags { get; }

	public SteamWorkshopItem(ulong itemId, string name, string visibility, bool updateDescription, bool updateVisibility, bool updatePreview, bool updateTags, IEnumerable<string> tags)
	{
		ItemId = itemId;
		Name = name;
		Visibility = visibility;
		UpdateDescription = updateDescription;
		UpdateVisibility = updateVisibility;
		UpdatePreview = updatePreview;
		UpdateTags = updateTags;
		Tags = tags.ToImmutableArray();
	}

	public static SteamWorkshopItem CreateFromUpdateRequest(SteamWorkshopUpdateRequest updateRequest, string defaultName, SteamWorkshopVisibility defaultVisibility)
	{
		return new SteamWorkshopItem(updateRequest.ItemId, updateRequest.Name ?? defaultName, (updateRequest.Visibility ?? defaultVisibility).ToString(), updateRequest.Description != null, updateRequest.Visibility.HasValue, updateRequest.PreviewPath != null, updateRequest.MandatoryTags.Length > 0 || updateRequest.ChosenTags.Length > 0, updateRequest.ChosenTags);
	}
}
