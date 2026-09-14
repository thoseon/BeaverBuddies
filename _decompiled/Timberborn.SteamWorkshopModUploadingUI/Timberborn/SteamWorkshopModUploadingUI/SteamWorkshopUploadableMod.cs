using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.Localization;
using Timberborn.Modding;
using Timberborn.SteamWorkshop;
using Timberborn.SteamWorkshopUI;
using UnityEngine;

namespace Timberborn.SteamWorkshopModUploadingUI;

internal class SteamWorkshopUploadableMod : ISteamWorkshopUploadable
{
	private static readonly string ThumbnailInfoLocKey = "Modding.ThumbnailInfo";

	private readonly ILoc _loc;

	private readonly SteamWorkshopModDataFile _steamWorkshopModDataFile;

	private readonly Mod _mod;

	private readonly SteamWorkshopModThumbnail _steamWorkshopModThumbnail;

	public ulong? ItemId => SteamWorkshopItem?.ItemId;

	public string Name => _mod.Manifest.Name;

	public bool NameIsReadOnly => true;

	public string Description => _mod.Manifest.Description;

	public SteamWorkshopVisibility Visibility
	{
		get
		{
			if (SteamWorkshopItem?.Visibility == null)
			{
				return SteamWorkshopVisibility.Private;
			}
			return Enum.Parse<SteamWorkshopVisibility>(SteamWorkshopItem.Visibility);
		}
	}

	public IEnumerable<string> MandatoryTags => SteamWorkshopModTags.MandatoryTags;

	public IEnumerable<WorkshopTag> AvailableTags => SteamWorkshopModTags.AvailableTags;

	public IEnumerable<string> ChosenTags => SteamWorkshopItem?.Tags ?? ImmutableArray<string>.Empty;

	public string ContentPath => _mod.ModDirectory.OriginPath;

	public Texture2D Preview => _steamWorkshopModThumbnail.Thumbnail;

	public string PreviewInfo => _loc.T(ThumbnailInfoLocKey);

	public string PreviewPath => _steamWorkshopModThumbnail.GetThumbnailPath();

	public bool UpdateDescription => SteamWorkshopItem?.UpdateDescription ?? true;

	public bool UpdateVisibility => SteamWorkshopItem?.UpdateVisibility ?? true;

	public bool UpdateTags => SteamWorkshopItem?.UpdateTags ?? true;

	public bool UpdatePreview
	{
		get
		{
			if (SteamWorkshopItem != null)
			{
				if (SteamWorkshopItem.UpdatePreview)
				{
					return Preview;
				}
				return false;
			}
			return true;
		}
	}

	private SteamWorkshopItem SteamWorkshopItem => _steamWorkshopModDataFile.SteamWorkshopItem;

	public SteamWorkshopUploadableMod(ILoc loc, SteamWorkshopModDataFile steamWorkshopModDataFile, Mod mod, SteamWorkshopModThumbnail steamWorkshopModThumbnail)
	{
		_loc = loc;
		_steamWorkshopModDataFile = steamWorkshopModDataFile;
		_mod = mod;
		_steamWorkshopModThumbnail = steamWorkshopModThumbnail;
	}

	public void RefreshPreview()
	{
		_steamWorkshopModThumbnail.UpdateThumbnail();
	}

	public bool ValidateName(string name)
	{
		return true;
	}

	public void OnItemCreated(ulong itemId, string name, SteamWorkshopVisibility visibility, IEnumerable<string> tags)
	{
		_steamWorkshopModDataFile.SaveSteamWorkshopItem(new SteamWorkshopItem(itemId, name, visibility.ToString(), updateDescription: true, updateVisibility: true, updatePreview: true, updateTags: true, tags));
	}

	public void OnUpdateStarted(string name)
	{
	}

	public void OnUpdateRequestCreated(SteamWorkshopUpdateRequest updateRequest)
	{
		_steamWorkshopModDataFile.SaveSteamWorkshopItem(SteamWorkshopItem.CreateFromUpdateRequest(updateRequest, Name, Visibility));
	}

	public void OnUpdateFinished(SteamWorkshopUpdateResponse updateResponse)
	{
	}

	public void Clear()
	{
		_steamWorkshopModThumbnail.Clear();
	}
}
