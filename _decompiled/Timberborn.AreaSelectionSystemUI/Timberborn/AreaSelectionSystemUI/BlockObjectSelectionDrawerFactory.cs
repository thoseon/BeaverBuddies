using Timberborn.AreaSelectionSystem;
using Timberborn.SelectionSystem;
using UnityEngine;

namespace Timberborn.AreaSelectionSystemUI;

public class BlockObjectSelectionDrawerFactory
{
	private readonly Highlighter _highlighter;

	private readonly RectangleBoundsDrawerFactory _rectangleBoundsDrawerFactory;

	private readonly MeasurableAreaDrawer _measurableAreaDrawer;

	public BlockObjectSelectionDrawerFactory(Highlighter highlighter, RectangleBoundsDrawerFactory rectangleBoundsDrawerFactory, MeasurableAreaDrawer measurableAreaDrawer)
	{
		_highlighter = highlighter;
		_rectangleBoundsDrawerFactory = rectangleBoundsDrawerFactory;
		_measurableAreaDrawer = measurableAreaDrawer;
	}

	public BlockObjectSelectionDrawer Create(Color blockObjectHighlightColor, Color areaTileColor, Color areaSideColor)
	{
		return new BlockObjectSelectionDrawer(_rectangleBoundsDrawerFactory.Create(areaTileColor, areaSideColor), new RollingHighlighter(_highlighter), blockObjectHighlightColor, _measurableAreaDrawer);
	}
}
