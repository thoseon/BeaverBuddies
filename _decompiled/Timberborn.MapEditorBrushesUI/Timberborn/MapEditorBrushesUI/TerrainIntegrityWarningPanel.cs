using Timberborn.CoreUI;
using Timberborn.ToolPanelSystem;
using UnityEngine.UIElements;

namespace Timberborn.MapEditorBrushesUI;

internal class TerrainIntegrityWarningPanel : IToolFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly TerrainIntegrityService _terrainIntegrityService;

	private VisualElement _root;

	public TerrainIntegrityWarningPanel(VisualElementLoader visualElementLoader, TerrainIntegrityService terrainIntegrityService)
	{
		_visualElementLoader = visualElementLoader;
		_terrainIntegrityService = terrainIntegrityService;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("MapEditor/ToolPanel/TerrainIntegrityWarningPanel");
		_root.ToggleDisplayStyle(visible: false);
		_terrainIntegrityService.HighlightChanged += OnHighlightChanged;
		return _root;
	}

	private void OnHighlightChanged(object sender, bool isHighlighted)
	{
		_root.ToggleDisplayStyle(isHighlighted);
	}
}
