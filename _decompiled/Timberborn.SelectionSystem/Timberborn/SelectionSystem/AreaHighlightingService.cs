using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Rendering;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.SelectionSystem;

public class AreaHighlightingService : ILoadableSingleton
{
	private readonly RollingHighlighter _rollingHighlighter;

	private readonly MarkerDrawerFactory _markerDrawerFactory;

	private readonly ISpecService _specService;

	private MeshDrawer _meshDrawer;

	private readonly HashSet<BaseComponent> _objetsToHighlight = new HashSet<BaseComponent>();

	private Color _selectionToolHighlightColor;

	public AreaHighlightingService(RollingHighlighter rollingHighlighter, MarkerDrawerFactory markerDrawerFactory, ISpecService specService)
	{
		_rollingHighlighter = rollingHighlighter;
		_markerDrawerFactory = markerDrawerFactory;
		_specService = specService;
	}

	public void Load()
	{
		_meshDrawer = _markerDrawerFactory.CreateTileDrawer();
		_selectionToolHighlightColor = _specService.GetSingleSpec<SelectionColorsSpec>().SelectionToolHighlight;
	}

	public void DrawTile(Vector3Int coordinates, Color color)
	{
		_meshDrawer.DrawAtCoordinates(coordinates, 0.02f, color);
	}

	public void AddForHighlight(BaseComponent target)
	{
		_objetsToHighlight.Add(target);
	}

	public void Highlight()
	{
		_rollingHighlighter.HighlightPrimary(_objetsToHighlight, _selectionToolHighlightColor);
		_objetsToHighlight.Clear();
	}

	public void UnhighlightAll()
	{
		_rollingHighlighter.UnhighlightAllPrimary();
	}
}
