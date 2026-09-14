using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.Common;
using Timberborn.Persistence;

namespace Timberborn.SteamWorkshop;

public class SteamWorkshopItemSerializer : IValueSerializer<SteamWorkshopItem>
{
	private static readonly PropertyKey<string> ItemIdKey = new PropertyKey<string>("ItemId");

	private static readonly PropertyKey<string> NameKey = new PropertyKey<string>("Name");

	private static readonly PropertyKey<string> VisibilityKey = new PropertyKey<string>("Visibility");

	private static readonly PropertyKey<bool> UpdateDescriptionKey = new PropertyKey<bool>("UpdateDescription");

	private static readonly PropertyKey<bool> UpdateVisibilityKey = new PropertyKey<bool>("UpdateVisibility");

	private static readonly PropertyKey<bool> UpdatePreviewKey = new PropertyKey<bool>("UpdatePreview");

	private static readonly PropertyKey<bool> UpdateTagsKey = new PropertyKey<bool>("UpdateTags");

	private static readonly ListKey<string> TagsKey = new ListKey<string>("Tags");

	public void Serialize(SteamWorkshopItem value, IValueSaver valueSaver)
	{
		IObjectSaver objectSaver = valueSaver.AsObject();
		objectSaver.Set(ItemIdKey, value.ItemId.ToString());
		objectSaver.Set(NameKey, value.Name);
		objectSaver.Set(VisibilityKey, value.Visibility);
		objectSaver.Set(UpdateDescriptionKey, value.UpdateDescription);
		objectSaver.Set(UpdateVisibilityKey, value.UpdateVisibility);
		objectSaver.Set(UpdatePreviewKey, value.UpdatePreview);
		objectSaver.Set(UpdateTagsKey, value.UpdateTags);
		objectSaver.Set(TagsKey, value.Tags);
	}

	[BackwardCompatible(2025, 11, 14, Compatibility.Map | Compatibility.Mod)]
	public Obsoletable<SteamWorkshopItem> Deserialize(IValueLoader valueLoader)
	{
		IObjectLoader objectLoader = valueLoader.AsObject();
		ulong itemId = ulong.Parse(objectLoader.Get(ItemIdKey));
		string name = objectLoader.Get(NameKey);
		string visibility = objectLoader.Get(VisibilityKey);
		bool updateDescription = objectLoader.Get(UpdateDescriptionKey);
		bool updateVisibility = objectLoader.Get(UpdateVisibilityKey);
		bool updatePreview = objectLoader.Get(UpdatePreviewKey);
		bool updateTags = objectLoader.Has(UpdateTagsKey) && objectLoader.Get(UpdateTagsKey);
		IEnumerable<string> tags;
		if (!objectLoader.Has(TagsKey))
		{
			IEnumerable<string> enumerable = ImmutableArray<string>.Empty;
			tags = enumerable;
		}
		else
		{
			IEnumerable<string> enumerable = objectLoader.Get(TagsKey);
			tags = enumerable;
		}
		return new SteamWorkshopItem(itemId, name, visibility, updateDescription, updateVisibility, updatePreview, updateTags, tags);
	}
}
