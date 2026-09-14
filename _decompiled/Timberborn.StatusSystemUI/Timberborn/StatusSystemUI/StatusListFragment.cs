using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.StatusSystem;
using UnityEngine.UIElements;

namespace Timberborn.StatusSystemUI;

public class StatusListFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private VisualElement _root;

	private StatusSubject _selectedStatusSubject;

	private readonly List<VisualElement> _statusListElements = new List<VisualElement>();

	public StatusListFragment(VisualElementLoader visualElementLoader)
	{
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/StatusListFragment");
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		StatusSubject component = entity.GetComponent<StatusSubject>();
		if (component != null)
		{
			_selectedStatusSubject = component;
		}
	}

	public void ClearFragment()
	{
		_selectedStatusSubject = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if (!_selectedStatusSubject)
		{
			return;
		}
		int num = 0;
		foreach (StatusInstance activeStatus in _selectedStatusSubject.ActiveStatuses)
		{
			if (activeStatus.IsVisible())
			{
				Show(GetStatusListElement(num++), activeStatus);
			}
		}
		HideStatusListElements(num);
		_root.ToggleDisplayStyle(num > 0);
	}

	private static void Show(VisualElement statusListElement, StatusInstance statusInstance)
	{
		statusListElement.Q<VisualElement>("Icon").style.backgroundImage = new StyleBackground(statusInstance.IconSmall);
		statusListElement.Q<Label>("Text").text = statusInstance.StatusDescription;
		statusListElement.ToggleDisplayStyle(visible: true);
	}

	private VisualElement GetStatusListElement(int index)
	{
		while (index >= _statusListElements.Count)
		{
			string elementName = "Game/EntityPanel/StatusListElement";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			_statusListElements.Add(visualElement);
			_root.Add(visualElement);
		}
		return _statusListElements[index];
	}

	private void HideStatusListElements(int startingIndex)
	{
		for (int i = startingIndex; i < _statusListElements.Count; i++)
		{
			_statusListElements[i].ToggleDisplayStyle(visible: false);
		}
	}
}
