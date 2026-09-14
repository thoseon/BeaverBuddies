using System.Collections.Generic;
using Timberborn.SteamWorkshop;
using UnityEngine;

namespace Timberborn.SteamWorkshopUI;

public interface ISteamWorkshopUploadable
{
	ulong? ItemId { get; }

	string Name { get; }

	bool NameIsReadOnly { get; }

	string Description { get; }

	SteamWorkshopVisibility Visibility { get; }

	IEnumerable<string> MandatoryTags { get; }

	IEnumerable<WorkshopTag> AvailableTags { get; }

	IEnumerable<string> ChosenTags { get; }

	string ContentPath { get; }

	Texture2D Preview { get; }

	string PreviewInfo { get; }

	string PreviewPath { get; }

	bool UpdateDescription { get; }

	bool UpdateVisibility { get; }

	bool UpdatePreview { get; }

	bool UpdateTags { get; }

	void RefreshPreview();

	bool ValidateName(string name);

	void OnItemCreated(ulong itemId, string name, SteamWorkshopVisibility visibility, IEnumerable<string> tags);

	void OnUpdateStarted(string name);

	void OnUpdateRequestCreated(SteamWorkshopUpdateRequest updateRequest);

	void OnUpdateFinished(SteamWorkshopUpdateResponse updateResponse);

	void Clear();
}
