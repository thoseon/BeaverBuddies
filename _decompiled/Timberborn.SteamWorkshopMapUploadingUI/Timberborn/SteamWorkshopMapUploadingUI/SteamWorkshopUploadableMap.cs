using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.FileSystem;
using Timberborn.MapEditorPersistenceUI;
using Timberborn.MapMetadataSystem;
using Timberborn.MapRepositorySystem;
using Timberborn.SteamWorkshop;
using Timberborn.SteamWorkshopUI;
using UnityEngine;

namespace Timberborn.SteamWorkshopMapUploadingUI;

internal class SteamWorkshopUploadableMap : ISteamWorkshopUploadable
{
	private static readonly string[] MapMandatoryTags = new string[1] { "Map" };

	private readonly SteamWorkshopMapDataService _steamWorkshopMapDataService;

	private readonly FilenameValidator _filenameValidator;

	private readonly MapSaverLoader _mapSaverLoader;

	private readonly SteamWorkshopMapContent _steamWorkshopMapContent;

	private readonly MapFileReference _mapFileReference;

	private readonly MapMetadata _mapMetadata;

	public IEnumerable<WorkshopTag> AvailableTags { get; } = Enumerable.Empty<WorkshopTag>();

	public IEnumerable<string> ChosenTags { get; } = Enumerable.Empty<string>();

	public ulong? ItemId => SteamWorkshopItem?.ItemId;

	public string Name => SteamWorkshopItem?.Name ?? _mapFileReference.Name;

	public bool NameIsReadOnly => false;

	public string Description => _mapMetadata?.MapDescription;

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

	public IEnumerable<string> MandatoryTags => MapMandatoryTags;

	public string ContentPath => _steamWorkshopMapContent.ContentDirectory;

	public Texture2D Preview => _steamWorkshopMapContent.Thumbnail;

	public string PreviewInfo => string.Empty;

	public string PreviewPath => _steamWorkshopMapContent.ThumbnailPath;

	public bool UpdateDescription => SteamWorkshopItem?.UpdateDescription ?? true;

	public bool UpdateVisibility => SteamWorkshopItem?.UpdateVisibility ?? true;

	public bool UpdatePreview => SteamWorkshopItem?.UpdatePreview ?? true;

	public bool UpdateTags => false;

	private SteamWorkshopItem SteamWorkshopItem => _steamWorkshopMapDataService.SteamWorkshopItem;

	public SteamWorkshopUploadableMap(SteamWorkshopMapDataService steamWorkshopMapDataService, FilenameValidator filenameValidator, MapSaverLoader mapSaverLoader, SteamWorkshopMapContent steamWorkshopMapContent, MapFileReference mapFileReference, MapMetadata mapMetadata)
	{
		_steamWorkshopMapDataService = steamWorkshopMapDataService;
		_mapSaverLoader = mapSaverLoader;
		_steamWorkshopMapContent = steamWorkshopMapContent;
		_mapFileReference = mapFileReference;
		_mapMetadata = mapMetadata;
		_filenameValidator = filenameValidator;
	}

	public void RefreshPreview()
	{
	}

	public bool ValidateName(string name)
	{
		return !_filenameValidator.NameIsInvalid(name);
	}

	public void OnItemCreated(ulong itemId, string name, SteamWorkshopVisibility visibility, IEnumerable<string> tags)
	{
		_steamWorkshopMapDataService.SetMapData(new SteamWorkshopItem(itemId, name, visibility.ToString(), updateDescription: true, updateVisibility: true, updatePreview: true, updateTags: true, tags));
		_mapSaverLoader.SaveCurrentSilently();
	}

	public void OnUpdateStarted(string name)
	{
		_steamWorkshopMapContent.CreateTemporaryFiles(name);
	}

	public void OnUpdateRequestCreated(SteamWorkshopUpdateRequest updateRequest)
	{
	}

	public void OnUpdateFinished(SteamWorkshopUpdateResponse updateResponse)
	{
		if (updateResponse.Successful)
		{
			_steamWorkshopMapDataService.SetMapData(SteamWorkshopItem.CreateFromUpdateRequest(updateResponse.Request, Name, Visibility));
			_mapSaverLoader.SaveCurrentSilently();
		}
		_steamWorkshopMapContent.DeleteTemporaryFiles();
	}

	public void Clear()
	{
	}
}
