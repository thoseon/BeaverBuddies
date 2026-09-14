using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace Timberborn.BottomBarSystem;

internal class BottomBarPanel : ILoadableSingleton
{
	private readonly ImmutableArray<BottomBarModule> _bottomBarModules;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly UILayout _uiLayout;

	private readonly EventBus _eventBus;

	private VisualElement _mainElements;

	private VisualElement _subElements;

	private VisualElement _root;

	public BottomBarPanel(IEnumerable<BottomBarModule> bottomBarModules, VisualElementLoader visualElementLoader, UILayout uiLayout, EventBus eventBus)
	{
		_bottomBarModules = bottomBarModules.ToImmutableArray();
		_visualElementLoader = visualElementLoader;
		_uiLayout = uiLayout;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Common/BottomBar/BottomBarPanel");
		_mainElements = _root.Q<VisualElement>("MainSection");
		_subElements = _root.Q<VisualElement>("SubSection");
		_uiLayout.AddBottomBar(_root, 100);
		_root.ToggleDisplayStyle(visible: false);
		InitializeSections();
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		_root.ToggleDisplayStyle(visible: true);
	}

	private void InitializeSections()
	{
		InitializeLeftSection();
		InitializeMiddleSection();
		InitializeRightSection();
	}

	private void InitializeLeftSection()
	{
		VisualElement visualElement = _mainElements.Q<VisualElement>("LeftSection");
		Dictionary<int, IBottomBarElementsProvider> dictionary = new Dictionary<int, IBottomBarElementsProvider>();
		foreach (KeyValuePair<int, IBottomBarElementsProvider> item in _bottomBarModules.SelectMany((BottomBarModule module) => module.LeftElements))
		{
			dictionary.Add(item.Key, item.Value);
		}
		foreach (int item2 in dictionary.Keys.OrderBy((int key) => key))
		{
			foreach (BottomBarElement element in dictionary[item2].GetElements())
			{
				AddElement(element, visualElement);
			}
		}
		visualElement.ToggleDisplayStyle(dictionary.Count > 0);
	}

	private void InitializeMiddleSection()
	{
		VisualElement visualElement = _mainElements.Q<VisualElement>("MiddleSection");
		foreach (IBottomBarElementsProvider item in _bottomBarModules.SelectMany((BottomBarModule module) => module.MiddleElements))
		{
			foreach (BottomBarElement element in item.GetElements())
			{
				AddElement(element, visualElement);
			}
		}
		visualElement.ToggleDisplayStyle(visualElement.childCount > 0);
	}

	private void InitializeRightSection()
	{
		VisualElement visualElement = _mainElements.Q<VisualElement>("RightSection");
		foreach (IBottomBarElementsProvider item in _bottomBarModules.SelectMany((BottomBarModule module) => module.RightElements))
		{
			foreach (BottomBarElement element in item.GetElements())
			{
				AddElement(element, visualElement);
			}
		}
		visualElement.ToggleDisplayStyle(visualElement.childCount > 0);
	}

	private void AddElement(BottomBarElement bottomBarElement, VisualElement elementRoot)
	{
		elementRoot.Add(bottomBarElement.MainElement);
		if (bottomBarElement.SubElement != null)
		{
			_subElements.Add(bottomBarElement.SubElement);
		}
	}
}
