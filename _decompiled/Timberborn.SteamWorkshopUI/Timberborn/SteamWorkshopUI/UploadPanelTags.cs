using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.CoreUI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.SteamWorkshopUI;

public class UploadPanelTags
{
	private static readonly string CategoryMarginClass = "steam-workshop-tag--margin";

	private readonly VisualElementLoader _visualElementLoader;

	private VisualElement _root;

	private ScrollView _tagsScrollView;

	private readonly List<Toggle> _tags = new List<Toggle>();

	public event EventHandler TagsChanged;

	public UploadPanelTags(VisualElementLoader visualElementLoader)
	{
		_visualElementLoader = visualElementLoader;
	}

	public void Initialize(VisualElement root)
	{
		_root = root;
		_tagsScrollView = root.Q<ScrollView>();
	}

	public void Open(ISteamWorkshopUploadable steamWorkshopUploadable)
	{
		foreach (IGrouping<WorkshopTagCategory, WorkshopTag> item in from tag in steamWorkshopUploadable.AvailableTags
			group tag by tag.Category into @group
			orderby @group.Key.Order
			select @group)
		{
			AddCategory(item.Key.Name);
			foreach (WorkshopTag item2 in item.OrderBy((WorkshopTag tag) => tag.Order))
			{
				AddTag(item2.Name, steamWorkshopUploadable.ChosenTags.Contains(item2.Name));
			}
		}
		_root.ToggleDisplayStyle(_tags.Count > 0);
	}

	public IEnumerable<string> GetChosenTags()
	{
		return from tagToggle in _tags
			where tagToggle.value
			select tagToggle.text;
	}

	public void Clear()
	{
		_tags.Clear();
		_tagsScrollView.Clear();
		_tagsScrollView.scrollOffset = Vector2.zero;
	}

	private void AddCategory(string category)
	{
		Label label = (Label)_visualElementLoader.LoadVisualElement("Common/SteamWorkshop/SteamWorkshopTagCategory");
		label.text = category;
		if (_tagsScrollView.childCount > 0)
		{
			label.AddToClassList(CategoryMarginClass);
		}
		_tagsScrollView.Add(label);
	}

	private void AddTag(string tag, bool enabled)
	{
		Toggle toggle = (Toggle)_visualElementLoader.LoadVisualElement("Common/SteamWorkshop/SteamWorkshopTag");
		toggle.text = tag;
		toggle.value = enabled;
		_tagsScrollView.Add(toggle);
		_tags.Add(toggle);
		toggle.RegisterValueChangedCallback(delegate
		{
			TagsChanged?.Invoke(this, EventArgs.Empty);
		});
	}
}
