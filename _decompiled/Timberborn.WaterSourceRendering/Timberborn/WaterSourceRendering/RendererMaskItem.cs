using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlockSystem;
using UnityEngine;

namespace Timberborn.WaterSourceRendering;

internal class RendererMaskItem
{
	private readonly WaterSourceRenderer _renderer;

	public ImmutableArray<Vector3Int> Coordinates { get; }

	public bool IsVisible { get; private set; }

	public RendererMaskItem(WaterSourceRenderer renderer)
	{
		_renderer = renderer;
		Coordinates = (from coordinate in _renderer.GetComponent<BlockObject>().PositionedBlocks.GetFoundationCoordinates()
			orderby coordinate.x, coordinate.y
			select coordinate).ToImmutableArray();
	}

	public bool UpdateVisibility(bool hasFullyVisibleWaterSurfaceAbove)
	{
		bool isVisible = IsVisible;
		IsVisible = _renderer.CanBeRendered & hasFullyVisibleWaterSurfaceAbove;
		return IsVisible != isVisible;
	}
}
