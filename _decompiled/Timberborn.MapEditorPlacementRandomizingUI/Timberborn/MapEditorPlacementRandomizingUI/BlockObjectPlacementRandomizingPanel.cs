using Timberborn.BlockObjectTools;
using Timberborn.CoreUI;
using Timberborn.MapEditorPlacementRandomizing;
using Timberborn.SingletonSystem;
using Timberborn.ToolPanelSystem;
using Timberborn.ToolSystem;
using UnityEngine.UIElements;

namespace Timberborn.MapEditorPlacementRandomizingUI;

internal class BlockObjectPlacementRandomizingPanel : IToolFragment
{
	private readonly EventBus _eventBus;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly BlockObjectPlacementRandomizingService _blockObjectPlacementRandomizingService;

	private VisualElement _root;

	private Toggle _randomizeToggle;

	public BlockObjectPlacementRandomizingPanel(EventBus eventBus, VisualElementLoader visualElementLoader, BlockObjectPlacementRandomizingService blockObjectPlacementRandomizingService)
	{
		_eventBus = eventBus;
		_visualElementLoader = visualElementLoader;
		_blockObjectPlacementRandomizingService = blockObjectPlacementRandomizingService;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "MapEditor/ToolPanel/BlockObjectPlacementRandomizingPanel";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_root.ToggleDisplayStyle(visible: false);
		_randomizeToggle = _root.Q<Toggle>("RandomizeToggle");
		_randomizeToggle.value = _blockObjectPlacementRandomizingService.Randomize;
		_randomizeToggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> evt)
		{
			_blockObjectPlacementRandomizingService.Randomize = evt.newValue;
		});
		_eventBus.Register(this);
		return _root;
	}

	[OnEvent]
	public void OnToolEntered(ToolEnteredEvent toolEnteredEvent)
	{
		if (toolEnteredEvent.Tool is BlockObjectTool blockObjectTool && blockObjectTool.Template.HasSpec<BlockObjectRandomizablePlacementSpec>())
		{
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	[OnEvent]
	public void OnToolExited(ToolExitedEvent toolExitedEvent)
	{
		_root.ToggleDisplayStyle(visible: false);
	}
}
