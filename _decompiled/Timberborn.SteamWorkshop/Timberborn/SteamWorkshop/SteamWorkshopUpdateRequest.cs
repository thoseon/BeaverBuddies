using System.Collections.Generic;
using System.Collections.Immutable;

namespace Timberborn.SteamWorkshop;

public class SteamWorkshopUpdateRequest
{
	public class Builder
	{
		private readonly ulong _itemId;

		private readonly string _name;

		private string _description;

		private SteamWorkshopVisibility? _visibility;

		private readonly HashSet<string> _mandatoryTags = new HashSet<string>();

		private readonly HashSet<string> _chosenTags = new HashSet<string>();

		private string _previewPath;

		private string _contentPath;

		private string _changelog;

		public Builder(ulong itemId, string name)
		{
			_itemId = itemId;
			_name = name;
		}

		public void SetDescription(string description)
		{
			_description = description;
		}

		public void SetVisibility(SteamWorkshopVisibility? visibility)
		{
			_visibility = visibility;
		}

		public Builder AddMandatoryTags(IEnumerable<string> tags)
		{
			foreach (string tag in tags)
			{
				_mandatoryTags.Add(tag);
			}
			return this;
		}

		public Builder AddChosenTags(IEnumerable<string> tags)
		{
			foreach (string tag in tags)
			{
				_chosenTags.Add(tag);
			}
			return this;
		}

		public void SetPreviewPath(string previewPath)
		{
			_previewPath = previewPath;
		}

		public Builder SetContentPath(string contentPath)
		{
			_contentPath = contentPath;
			return this;
		}

		public void SetChangelog(string changelog)
		{
			_changelog = changelog;
		}

		public SteamWorkshopUpdateRequest Build()
		{
			return new SteamWorkshopUpdateRequest(_itemId, _name, _description, _visibility, _mandatoryTags, _chosenTags, _previewPath, _contentPath, _changelog);
		}
	}

	public ulong ItemId { get; }

	public string Name { get; }

	public string Description { get; }

	public SteamWorkshopVisibility? Visibility { get; }

	public ImmutableArray<string> MandatoryTags { get; }

	public ImmutableArray<string> ChosenTags { get; }

	public string PreviewPath { get; }

	public string ContentPath { get; }

	public string Changelog { get; }

	private SteamWorkshopUpdateRequest(ulong itemId, string name, string description, SteamWorkshopVisibility? visibility, IEnumerable<string> mandatoryTags, IEnumerable<string> chosenTags, string previewPath, string contentPath, string changelog)
	{
		ItemId = itemId;
		Name = name;
		Description = description;
		Visibility = visibility;
		MandatoryTags = mandatoryTags.ToImmutableArray();
		ChosenTags = chosenTags.ToImmutableArray();
		PreviewPath = previewPath;
		ContentPath = contentPath;
		Changelog = changelog;
	}
}
