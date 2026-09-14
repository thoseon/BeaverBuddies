using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace Timberborn.ToolPanelSystem;

internal class ToolPanel : ILoadableSingleton
{
	private readonly UILayout _uiLayout;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ImmutableArray<ToolPanelModule> _toolPanelModules;

	public ToolPanel(UILayout uiLayout, VisualElementLoader visualElementLoader, IEnumerable<ToolPanelModule> toolPanelModules)
	{
		_uiLayout = uiLayout;
		_visualElementLoader = visualElementLoader;
		_toolPanelModules = toolPanelModules.ToImmutableArray();
	}

	public void Load()
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Common/ToolPanel/ToolPanel");
		AddFragments(visualElement);
		_uiLayout.AddBottomBar(visualElement, 50);
	}

	private void AddFragments(VisualElement root)
	{
		List<OrderedToolFragment> list = new List<OrderedToolFragment>();
		foreach (ToolPanelModule toolPanelModule in _toolPanelModules)
		{
			list.AddRange(toolPanelModule.ToolFragments);
		}
		foreach (OrderedToolFragment item in list.OrderByDescending((OrderedToolFragment fragment) => fragment.Order))
		{
			root.Add(item.ToolFragment.InitializeFragment());
		}
	}
}
