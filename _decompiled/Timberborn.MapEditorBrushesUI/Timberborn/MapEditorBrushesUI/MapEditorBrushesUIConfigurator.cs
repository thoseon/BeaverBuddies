using Bindito.Core;
using Timberborn.ToolPanelSystem;

namespace Timberborn.MapEditorBrushesUI;

[Context("MapEditor")]
internal class MapEditorBrushesUIConfigurator : Configurator
{
	private class ToolPanelModuleProvider : IProvider<ToolPanelModule>
	{
		private readonly TerrainIntegrityWarningPanel _terrainIntegrityWarningPanel;

		public ToolPanelModuleProvider(TerrainIntegrityWarningPanel terrainIntegrityWarningPanel)
		{
			_terrainIntegrityWarningPanel = terrainIntegrityWarningPanel;
		}

		public ToolPanelModule Get()
		{
			ToolPanelModule.Builder builder = new ToolPanelModule.Builder();
			builder.AddFragment(_terrainIntegrityWarningPanel, 100);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<AbsoluteTerrainHeightBrushTool>().AsSingleton();
		Bind<RelativeTerrainHeightBrushTool>().AsSingleton();
		Bind<SculptingTerrainBrushTool>().AsSingleton();
		Bind<TerrainIntegrityService>().AsSingleton();
		Bind<TerrainIntegrityWarningPanel>().AsSingleton();
		MultiBind<ToolPanelModule>().ToProvider<ToolPanelModuleProvider>().AsSingleton();
	}
}
