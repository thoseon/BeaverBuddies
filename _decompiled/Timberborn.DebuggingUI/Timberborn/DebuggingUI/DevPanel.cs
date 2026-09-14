using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.KeyBindingSystemUI;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.DebuggingUI;

public class DevPanel : ILoadableSingleton
{
	private static readonly string FavouritesPrefValue = "DevPanel.Favourites";

	private static readonly string FavouritesSeparator = "|";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly EventBus _eventBus;

	private readonly DevModeManager _devModeManager;

	private readonly UILayout _uiLayout;

	private readonly InputBindingDescriber _inputBindingDescriber;

	private readonly ImmutableArray<IDevModule> _devModules;

	private readonly HashSet<string> _favouriteMethods = new HashSet<string>();

	private readonly Dictionary<string, VisualElement> _buttons = new Dictionary<string, VisualElement>();

	private List<DevMethod> _devMethods;

	private VisualElement _root;

	private VisualElement _devPanelContent;

	private VisualElement _favouriteMethodsContainer;

	private VisualElement _otherMethodsContainer;

	private TextField _filter;

	private bool _expanded;

	public DevPanel(VisualElementLoader visualElementLoader, EventBus eventBus, DevModeManager devModeManager, UILayout uiLayout, InputBindingDescriber inputBindingDescriber, IEnumerable<IDevModule> devModules)
	{
		_visualElementLoader = visualElementLoader;
		_eventBus = eventBus;
		_devModeManager = devModeManager;
		_uiLayout = uiLayout;
		_inputBindingDescriber = inputBindingDescriber;
		_devModules = devModules.ToImmutableArray();
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Common/DevPanel/DevPanel");
		_eventBus.Register(this);
		_root.ToggleDisplayStyle(_devModeManager.Enabled);
		_root.Q<Button>("DevPanelTitle").RegisterCallback<ClickEvent>(DevPanelTitleClicked);
		_filter = _root.Q<TextField>("DevPanelFilter");
		_filter.RegisterValueChangedCallback(delegate(ChangeEvent<string> evt)
		{
			FilterButtons(evt.newValue);
		});
		_filter.textEdition.placeholder = "Type to filter...";
		_devPanelContent = _root.Q("DevPanelContent");
		_root.Q<ScrollView>("DevPanelScroll").verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
		_favouriteMethodsContainer = _root.Q("FavouriteMethods");
		_otherMethodsContainer = _root.Q("OtherMethods");
		LoadDevMethods();
		LoadFavoriteMethods();
		UpdateContentVisibility();
		CreateDevMethodButtons();
		_uiLayout.AddBottomLeft(_root, 2);
	}

	[OnEvent]
	public void OnDevModeToggled(DevModeToggledEvent devModeToggledEvent)
	{
		UpdateRootVisibility();
	}

	private void LoadDevMethods()
	{
		_devMethods = (from devMethod in _devModules.Select((IDevModule devModule) => devModule.GetDefinition()).SelectMany((DevModuleDefinition devModuleDefinition) => devModuleDefinition.Methods)
			orderby devMethod.Name
			select devMethod).ToList();
	}

	private void LoadFavoriteMethods()
	{
		if (!PlayerPrefs.HasKey(FavouritesPrefValue))
		{
			return;
		}
		string[] array = PlayerPrefs.GetString(FavouritesPrefValue).Split(FavouritesSeparator, StringSplitOptions.RemoveEmptyEntries);
		foreach (string favourite in array)
		{
			if (_devMethods.Any((DevMethod devMethod) => devMethod.Name == favourite))
			{
				_favouriteMethods.Add(favourite);
			}
		}
	}

	private void CreateDevMethodButtons()
	{
		_buttons.Clear();
		foreach (DevMethod devMethod in _devMethods)
		{
			_buttons.Add(devMethod.Name.ToLower(), CreateDevMethodButton(devMethod));
		}
		bool visible = _favouriteMethods.Any();
		_root.Q<VisualElement>("FavouritesLabel").ToggleDisplayStyle(visible);
		_root.Q<VisualElement>("OtherLabel").ToggleDisplayStyle(visible);
	}

	private VisualElement CreateDevMethodButton(DevMethod devMethod)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Common/DevPanel/DevPanelButton");
		bool flag = _favouriteMethods.Contains(devMethod.Name);
		Button button = visualElement.Q<Button>("Invoke");
		button.text = string.Concat(str1: string.IsNullOrEmpty(devMethod.KeyBindingId) ? "" : (" [" + _inputBindingDescriber.GetInputBindingText(devMethod.KeyBindingId) + "]"), str0: devMethod.Name);
		button.RegisterCallback<ClickEvent>(delegate
		{
			InvokeAction(devMethod);
		});
		Button button2 = visualElement.Q<Button>("Add");
		button2.RegisterCallback<ClickEvent>(delegate
		{
			AddToFavourites(devMethod);
		});
		button2.ToggleDisplayStyle(!flag);
		Button button3 = visualElement.Q<Button>("Remove");
		button3.ToggleDisplayStyle(flag);
		button3.RegisterCallback<ClickEvent>(delegate
		{
			RemoveFromFavourites(devMethod);
		});
		if (flag)
		{
			_favouriteMethodsContainer.Add(visualElement);
		}
		else
		{
			_otherMethodsContainer.Add(visualElement);
		}
		return visualElement;
	}

	private void DevPanelTitleClicked(ClickEvent evt)
	{
		_expanded = !_expanded;
		UpdateContentVisibility();
	}

	private void InvokeAction(DevMethod devMethod)
	{
		ShowAllButtons();
		devMethod.Invoke();
	}

	private void AddToFavourites(DevMethod devMethod)
	{
		_favouriteMethods.Add(devMethod.Name);
		SaveFavouritesAndRebuildPanel();
	}

	private void RemoveFromFavourites(DevMethod devMethod)
	{
		_favouriteMethods.Remove(devMethod.Name);
		SaveFavouritesAndRebuildPanel();
	}

	private void SaveFavouritesAndRebuildPanel()
	{
		ResetFilter();
		SaveFavouriteMethods();
		ClearDevMethodButtons();
		CreateDevMethodButtons();
	}

	private void SaveFavouriteMethods()
	{
		string value = string.Join(FavouritesSeparator, _favouriteMethods);
		PlayerPrefs.SetString(FavouritesPrefValue, value);
	}

	private void ClearDevMethodButtons()
	{
		_favouriteMethodsContainer.Clear();
		_otherMethodsContainer.Clear();
	}

	private void UpdateRootVisibility()
	{
		ShowAllButtons();
		_root.ToggleDisplayStyle(_devModeManager.Enabled);
	}

	private void UpdateContentVisibility()
	{
		ShowAllButtons();
		_devPanelContent.ToggleDisplayStyle(_expanded);
	}

	private void FilterButtons(string textFilter)
	{
		if (string.IsNullOrEmpty(textFilter))
		{
			ShowAllButtons();
			return;
		}
		string value = textFilter.ToLower();
		foreach (var (text2, visualElement2) in _buttons)
		{
			visualElement2.ToggleDisplayStyle(text2.Contains(value));
		}
	}

	private void ShowAllButtons()
	{
		ResetFilter();
		foreach (VisualElement value in _buttons.Values)
		{
			value.ToggleDisplayStyle(visible: true);
		}
	}

	private void ResetFilter()
	{
		_filter.SetValueWithoutNotify("");
	}
}
