using System.Collections.Generic;
using Timberborn.CoreUI;
using Timberborn.NeedSystem;
using UnityEngine.UIElements;

namespace Timberborn.WellbeingUI;

internal class NeedGroupView
{
	private readonly List<NeedView> _needViews = new List<NeedView>();

	private readonly VisualElement _items;

	public VisualElement Root { get; }

	public NeedGroupView(VisualElement root, VisualElement items)
	{
		Root = root;
		_items = items;
	}

	public void AddNeed(NeedView needView)
	{
		_needViews.Add(needView);
		_items.Add(needView.Root);
	}

	public void Update(NeedManager needManager, bool showControls)
	{
		if (!_items.IsDisplayed())
		{
			return;
		}
		bool flag = false;
		foreach (NeedView needView in _needViews)
		{
			needView.Update(needManager, showControls);
			needView.UpdateVisibility(needManager, showControls);
			flag |= needView.Root.IsDisplayed();
		}
		UpdateVisibility(flag);
	}

	public void UpdateVisibility()
	{
		UpdateVisibility(_needViews.Count > 0);
	}

	public void ClearNeedViews()
	{
		_items.Clear();
		_needViews.Clear();
	}

	private void UpdateVisibility(bool visible)
	{
		Root.ToggleDisplayStyle(visible);
	}
}
