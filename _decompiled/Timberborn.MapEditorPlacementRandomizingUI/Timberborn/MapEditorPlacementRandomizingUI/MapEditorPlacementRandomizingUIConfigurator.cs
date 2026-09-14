using Bindito.Core;
using Timberborn.ToolPanelSystem;

namespace Timberborn.MapEditorPlacementRandomizingUI;

[Context("MapEditor")]
internal class MapEditorPlacementRandomizingUIConfigurator : Configurator
{
	private class ToolPanelModuleProvider : IProvider<ToolPanelModule>
	{
		private readonly BlockObjectPlacementRandomizingPanel _blockObjectPlacementRandomizingPanel;

		public ToolPanelModuleProvider(BlockObjectPlacementRandomizingPanel blockObjectPlacementRandomizingPanel)
		{
			_blockObjectPlacementRandomizingPanel = blockObjectPlacementRandomizingPanel;
		}

		public ToolPanelModule Get()
		{
			ToolPanelModule.Builder builder = new ToolPanelModule.Builder();
			builder.AddFragment(_blockObjectPlacementRandomizingPanel, 30);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<BlockObjectPlacementRandomizingPanel>().AsSingleton();
		MultiBind<ToolPanelModule>().ToProvider<ToolPanelModuleProvider>().AsSingleton();
	}
}
