using System.Collections.Generic;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.Forestry;
using Timberborn.LevelVisibilitySystem;
using Timberborn.Rendering;
using Timberborn.RootProviders;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.ToolSystem;
using Timberborn.Yielding;
using UnityEngine;

namespace Timberborn.ForestryUI;

public class TreeCuttingAreaVisualizer : ILoadableSingleton
{
	private readonly Highlighter _highlighter;

	private readonly EventBus _eventBus;

	private readonly TreeCuttingArea _treeCuttingArea;

	private readonly ISpecService _specService;

	private readonly AreaTileDrawerFactory _areaTileDrawerFactory;

	private readonly RootObjectProvider _rootObjectProvider;

	private readonly ILevelVisibilityService _levelVisibilityService;

	private GameObject _parent;

	private AreaTileDrawer _areaTileDrawer;

	private bool _updateAreaOnEnter;

	private bool _enabled;

	private Color _cuttingAreaHighlightColor;

	public TreeCuttingAreaVisualizer(Highlighter highlighter, EventBus eventBus, TreeCuttingArea treeCuttingArea, ISpecService specService, AreaTileDrawerFactory areaTileDrawerFactory, RootObjectProvider rootObjectProvider, ILevelVisibilityService levelVisibilityService)
	{
		_highlighter = highlighter;
		_eventBus = eventBus;
		_treeCuttingArea = treeCuttingArea;
		_specService = specService;
		_areaTileDrawerFactory = areaTileDrawerFactory;
		_rootObjectProvider = rootObjectProvider;
		_levelVisibilityService = levelVisibilityService;
	}

	public void Load()
	{
		_parent = _rootObjectProvider.CreateRootObject("TreeCuttingAreaVisualizer");
		_levelVisibilityService.MaxVisibleLevelChanged += OnMaxVisibleLevelChanged;
		TreeCuttingColorsSpec singleSpec = _specService.GetSingleSpec<TreeCuttingColorsSpec>();
		_areaTileDrawer = _areaTileDrawerFactory.Create(singleSpec.CuttingAreaTile, _parent);
		_cuttingAreaHighlightColor = singleSpec.CuttingAreaHighlight;
		_updateAreaOnEnter = true;
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnToolGroupEntered(ToolGroupEnteredEvent toolGroupEnteredEvent)
	{
		ToolGroupSpec toolGroup = toolGroupEnteredEvent.ToolGroup;
		if ((object)toolGroup != null && toolGroup.HasSpec<TreeCuttingToolGroupSpec>())
		{
			_enabled = true;
			if (_updateAreaOnEnter)
			{
				_updateAreaOnEnter = false;
				_areaTileDrawer.UpdateArea(GetCuttingArea());
			}
			Highlight();
		}
	}

	[OnEvent]
	public void OnToolGroupExited(ToolGroupExitedEvent toolGroupExitedEvent)
	{
		ToolGroupSpec toolGroup = toolGroupExitedEvent.ToolGroup;
		if ((object)toolGroup != null && toolGroup.HasSpec<TreeCuttingToolGroupSpec>())
		{
			_enabled = false;
			_highlighter.UnhighlightAllSecondary();
			_areaTileDrawer.HideAllTiles();
		}
	}

	[OnEvent]
	public void OnTreeCuttingAreaChanged(TreeCuttingAreaChangedEvent treeCuttingAreaChangedEvent)
	{
		UpdateOrMarkForUpdate();
	}

	[OnEvent]
	public void OnTreeAddedToCuttingArea(TreeAddedToCuttingAreaEvent treeAddedToCuttingAreaEvent)
	{
		if (_enabled)
		{
			TreeComponent treeComponent = treeAddedToCuttingAreaEvent.TreeComponent;
			_highlighter.HighlightSecondary(treeComponent, _cuttingAreaHighlightColor);
		}
	}

	private void OnMaxVisibleLevelChanged(object sender, int e)
	{
		UpdateOrMarkForUpdate();
	}

	private void UpdateOrMarkForUpdate()
	{
		if (_enabled)
		{
			_areaTileDrawer.UpdateArea(GetCuttingArea());
			Highlight();
		}
		else
		{
			_updateAreaOnEnter = true;
		}
	}

	private IEnumerable<Vector3Int> GetCuttingArea()
	{
		return _treeCuttingArea.CuttingArea.Where((Vector3Int coords) => coords.z <= _levelVisibilityService.MaxVisibleLevel);
	}

	private void Highlight()
	{
		_highlighter.UnhighlightAllSecondary();
		_areaTileDrawer.ShowAllTiles();
		foreach (Yielder item in _treeCuttingArea.YieldersInArea)
		{
			_highlighter.HighlightSecondary(item, _cuttingAreaHighlightColor);
		}
	}
}
