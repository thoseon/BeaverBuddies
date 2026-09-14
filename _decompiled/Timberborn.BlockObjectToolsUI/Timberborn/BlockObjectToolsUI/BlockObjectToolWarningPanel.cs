using Timberborn.BlockObjectTools;
using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using Timberborn.ToolPanelSystem;
using Timberborn.ToolSystem;
using UnityEngine.UIElements;

namespace Timberborn.BlockObjectToolsUI;

internal class BlockObjectToolWarningPanel : IToolFragment, IUpdatableSingleton
{
	private readonly ToolService _toolService;

	private readonly VisualElementLoader _visualElementLoader;

	private VisualElement _root;

	private Label _text;

	public BlockObjectToolWarningPanel(ToolService toolService, VisualElementLoader visualElementLoader)
	{
		_toolService = toolService;
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Common/ToolPanel/BlockObjectToolWarningPanel");
		_text = _root.Q<Label>("Warning");
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void UpdateSingleton()
	{
		if (_toolService.ActiveTool is BlockObjectTool blockObjectTool)
		{
			UpdateText(blockObjectTool.WarningText);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	private void UpdateText(string warning)
	{
		if (!string.IsNullOrWhiteSpace(warning))
		{
			_root.ToggleDisplayStyle(visible: true);
			_text.text = warning;
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}
}
