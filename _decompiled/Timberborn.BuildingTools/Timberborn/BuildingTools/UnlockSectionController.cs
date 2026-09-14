using Timberborn.BlockObjectTools;
using Timberborn.CoreUI;
using Timberborn.ToolSystem;
using UnityEngine.UIElements;

namespace Timberborn.BuildingTools;

public class UnlockSectionController
{
	private static readonly string HighlightClass = "highlight";

	private readonly ToolUnlockingService _toolUnlockingService;

	private ITool _toolToHighlight;

	public UnlockSectionController(ToolUnlockingService toolUnlockingService)
	{
		_toolUnlockingService = toolUnlockingService;
	}

	public void UpdateSection(VisualElement section, BlockObjectTool tool)
	{
		if (_toolUnlockingService.IsLocked(tool))
		{
			section.ToggleDisplayStyle(visible: true);
			section.EnableInClassList(HighlightClass, _toolToHighlight != null && _toolToHighlight == tool);
		}
		else
		{
			section.ToggleDisplayStyle(visible: false);
			section.RemoveFromClassList(HighlightClass);
		}
	}

	public void ToggleHighlight(bool state, ITool tool)
	{
		_toolToHighlight = (state ? tool : null);
	}
}
