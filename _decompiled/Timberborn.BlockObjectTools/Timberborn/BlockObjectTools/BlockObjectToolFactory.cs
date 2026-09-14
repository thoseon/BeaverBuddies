using Timberborn.AreaSelectionSystem;
using Timberborn.BlockSystem;
using Timberborn.InputSystem;
using Timberborn.ToolSystem;
using Timberborn.UISound;
using Timberborn.UndoSystem;

namespace Timberborn.BlockObjectTools;

public class BlockObjectToolFactory
{
	private readonly ToolService _toolService;

	private readonly InputService _inputService;

	private readonly AreaPicker _areaPicker;

	private readonly PreviewPlacerFactory _previewPlacerFactory;

	private readonly UISoundController _uiSoundController;

	private readonly ToolUnlockingService _toolUnlockingService;

	private readonly IUndoRegistry _undoRegistry;

	private readonly PreviewPlacement _previewPlacement;

	public BlockObjectToolFactory(ToolService toolService, InputService inputService, AreaPicker areaPicker, PreviewPlacerFactory previewPlacerFactory, UISoundController uiSoundController, ToolUnlockingService toolUnlockingService, IUndoRegistry undoRegistry, PreviewPlacement previewPlacement)
	{
		_toolService = toolService;
		_inputService = inputService;
		_areaPicker = areaPicker;
		_previewPlacerFactory = previewPlacerFactory;
		_uiSoundController = uiSoundController;
		_toolUnlockingService = toolUnlockingService;
		_undoRegistry = undoRegistry;
		_previewPlacement = previewPlacement;
	}

	public BlockObjectTool Create(PlaceableBlockObjectSpec template, IBlockObjectPlacer blockObjectPlacer, IBlockObjectToolDescriber blockObjectToolDescriber)
	{
		return new BlockObjectTool(template, _toolService, _inputService, _areaPicker, _uiSoundController, _toolUnlockingService, blockObjectPlacer, blockObjectToolDescriber, _previewPlacerFactory.Create(template), _undoRegistry, _previewPlacement);
	}
}
