using System.Collections.Generic;
using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.UILayoutSystem;

public class UILayout : ILoadableSingleton
{
	private static readonly string VisualTreeAssetName = "Common/GameUI";

	private static readonly string ContainerName = "Panels";

	private static readonly string TopRightButtonsClass = "game-ui__top-right-buttons";

	private static readonly int TopRightButtonsOrder = 7;

	private readonly PanelStack _panelStack;

	private VisualElement _topLeft;

	private VisualElement _topRight;

	private VisualElement _topRightButtons;

	private VisualElement _topBar;

	private VisualElement _bottomLeft;

	private VisualElement _bottomRight;

	private VisualElement _bottomBar;

	private VisualElement _absoluteItems;

	private readonly Dictionary<VisualElement, int> _elementOrder = new Dictionary<VisualElement, int>();

	public bool BottomBarVisible => _bottomBar.IsDisplayed();

	public UILayout(PanelStack panelStack)
	{
		_panelStack = panelStack;
	}

	public void Load()
	{
		VisualElement e = _panelStack.Initialize(VisualTreeAssetName, ContainerName);
		_topLeft = e.Q<VisualElement>("Top-left");
		_topRight = e.Q<VisualElement>("Top-right");
		_topBar = e.Q<VisualElement>("Top-bar");
		_bottomLeft = e.Q<VisualElement>("Bottom-left");
		_bottomRight = e.Q<VisualElement>("Bottom-right");
		_bottomBar = e.Q<VisualElement>("Bottom-bar");
		_absoluteItems = e.Q<VisualElement>("Absolute-items");
		_topRightButtons = new VisualElement();
		_topRightButtons.AddToClassList(TopRightButtonsClass);
		AddTopRight(_topRightButtons, TopRightButtonsOrder);
	}

	public void AddTopLeft(VisualElement visualElement, int order)
	{
		AddOrderablePanel(_topLeft, visualElement, order);
	}

	public void AddTopRight(VisualElement visualElement, int order)
	{
		AddOrderablePanel(_topRight, visualElement, order);
	}

	public void AddTopRightButton(VisualElement visualElement, int order)
	{
		AddOrderablePanel(_topRightButtons, visualElement, order);
	}

	public void AddTopBar(VisualElement visualElement)
	{
		AddPanel(_topBar, visualElement);
	}

	public void AddBottomLeft(VisualElement visualElement, int order)
	{
		AddOrderablePanel(_bottomLeft, visualElement, order);
	}

	public void AddBottomRight(VisualElement visualElement, int order)
	{
		AddOrderablePanel(_bottomRight, visualElement, order);
	}

	public void AddBottomBar(VisualElement visualElement, int order)
	{
		AddOrderablePanel(_bottomBar, visualElement, order);
	}

	public void AddAbsoluteItem(VisualElement visualElement)
	{
		AddPanel(_absoluteItems, visualElement);
	}

	public void ShowLeftAndCenterItems()
	{
		_topLeft.ToggleDisplayStyle(visible: true);
		_topBar.ToggleDisplayStyle(visible: true);
		_bottomLeft.ToggleDisplayStyle(visible: true);
		_bottomBar.ToggleDisplayStyle(visible: true);
	}

	public void HideLeftAndCenterItems()
	{
		_topLeft.ToggleDisplayStyle(visible: false);
		_topBar.ToggleDisplayStyle(visible: false);
		_bottomLeft.ToggleDisplayStyle(visible: false);
		_bottomBar.ToggleDisplayStyle(visible: false);
	}

	private static void AddPanel(VisualElement root, VisualElement item)
	{
		root.Add(item);
	}

	private void AddOrderablePanel(VisualElement root, VisualElement visualElement, int order)
	{
		int index = FindInsertionIndex(root, order);
		root.Insert(index, visualElement);
		_elementOrder[visualElement] = order;
	}

	private int FindInsertionIndex(VisualElement root, int order)
	{
		for (int i = 0; i < root.childCount; i++)
		{
			if (_elementOrder[root[i]] > order)
			{
				return i;
			}
		}
		return root.childCount;
	}
}
