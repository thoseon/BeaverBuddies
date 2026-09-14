using Bindito.Core;
using Timberborn.ToolPanelSystem;

namespace Timberborn.BlockObjectToolsUI;

[Context("Game")]
[Context("MapEditor")]
internal class BlockObjectToolsUIConfigurator : Configurator
{
	private class ToolPanelModuleProvider : IProvider<ToolPanelModule>
	{
		private readonly BlockObjectPlacementPanel _blockObjectPlacementPanel;

		private readonly BlockObjectToolWarningPanel _blockObjectToolWarningPanel;

		public ToolPanelModuleProvider(BlockObjectPlacementPanel blockObjectPlacementPanel, BlockObjectToolWarningPanel blockObjectToolWarningPanel)
		{
			_blockObjectPlacementPanel = blockObjectPlacementPanel;
			_blockObjectToolWarningPanel = blockObjectToolWarningPanel;
		}

		public ToolPanelModule Get()
		{
			ToolPanelModule.Builder builder = new ToolPanelModule.Builder();
			builder.AddFragment(_blockObjectPlacementPanel, 20);
			builder.AddFragment(_blockObjectToolWarningPanel, 40);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<BlockObjectToolButtonFactory>().AsSingleton();
		Bind<BlockObjectToolGroupButtonFactory>().AsSingleton();
		Bind<BlockObjectPlacementPanel>().AsSingleton();
		Bind<BlockObjectToolWarningPanel>().AsSingleton();
		MultiBind<ToolPanelModule>().ToProvider<ToolPanelModuleProvider>().AsSingleton();
	}
}
