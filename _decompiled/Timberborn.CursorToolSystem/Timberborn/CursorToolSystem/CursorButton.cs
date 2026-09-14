using System.Collections.Generic;
using Timberborn.BottomBarSystem;
using Timberborn.Localization;
using Timberborn.ToolButtonSystem;

namespace Timberborn.CursorToolSystem;

public class CursorButton : IBottomBarElementsProvider
{
	private static readonly string ToolImageKey = "Cursor";

	private static readonly string CursorTooltipLocKey = "Tool.Cursor.Tooltip";

	private readonly ILoc _loc;

	private readonly CursorTool _cursorTool;

	private readonly ToolButtonFactory _toolButtonFactory;

	public CursorButton(ILoc loc, CursorTool cursorTool, ToolButtonFactory toolButtonFactory)
	{
		_loc = loc;
		_cursorTool = cursorTool;
		_toolButtonFactory = toolButtonFactory;
	}

	public IEnumerable<BottomBarElement> GetElements()
	{
		ToolButton toolButton = _toolButtonFactory.CreateGrouplessRed(_cursorTool, ToolImageKey);
		toolButton.InitializeTooltip(_loc.T(CursorTooltipLocKey));
		yield return BottomBarElement.CreateSingleLevel(toolButton.Root);
	}
}
