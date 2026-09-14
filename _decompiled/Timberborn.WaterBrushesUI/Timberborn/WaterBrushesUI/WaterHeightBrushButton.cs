using System.Collections.Generic;
using Timberborn.BottomBarSystem;
using Timberborn.ToolButtonSystem;

namespace Timberborn.WaterBrushesUI;

internal class WaterHeightBrushButton : IBottomBarElementsProvider
{
	private static readonly string ToolImageKey = "WaterHeightBrushTool";

	private readonly WaterHeightBrushTool _waterHeightBrushTool;

	private readonly ToolButtonFactory _toolButtonFactory;

	public WaterHeightBrushButton(WaterHeightBrushTool waterHeightBrushTool, ToolButtonFactory toolButtonFactory)
	{
		_waterHeightBrushTool = waterHeightBrushTool;
		_toolButtonFactory = toolButtonFactory;
	}

	public IEnumerable<BottomBarElement> GetElements()
	{
		ToolButton toolButton = _toolButtonFactory.CreateGrouplessRed(_waterHeightBrushTool, ToolImageKey);
		yield return BottomBarElement.CreateSingleLevel(toolButton.Root);
	}
}
