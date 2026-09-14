using Bindito.Core;
using Timberborn.ToolPanelSystem;

namespace Timberborn.BrushesUI;

[Context("Game")]
[Context("MapEditor")]
internal class BrushesUIConfigurator : Configurator
{
	private class ToolPanelModuleProvider : IProvider<ToolPanelModule>
	{
		private readonly BrushDirectionPanel _brushDirectionPanel;

		private readonly BrushHeightPanel _brushHeightPanel;

		private readonly BrushSizePanel _brushSizePanel;

		private readonly BrushShapePanel _brushShapePanel;

		public ToolPanelModuleProvider(BrushDirectionPanel brushDirectionPanel, BrushHeightPanel brushHeightPanel, BrushSizePanel brushSizePanel, BrushShapePanel brushShapePanel)
		{
			_brushDirectionPanel = brushDirectionPanel;
			_brushHeightPanel = brushHeightPanel;
			_brushSizePanel = brushSizePanel;
			_brushShapePanel = brushShapePanel;
		}

		public ToolPanelModule Get()
		{
			ToolPanelModule.Builder builder = new ToolPanelModule.Builder();
			builder.AddFragment(_brushDirectionPanel, 60);
			builder.AddFragment(_brushHeightPanel, 70);
			builder.AddFragment(_brushSizePanel, 80);
			builder.AddFragment(_brushShapePanel, 90);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<BrushDirectionPanel>().AsSingleton();
		Bind<BrushHeightPanel>().AsSingleton();
		Bind<BrushSizePanel>().AsSingleton();
		Bind<BrushShapePanel>().AsSingleton();
		MultiBind<ToolPanelModule>().ToProvider<ToolPanelModuleProvider>().AsSingleton();
	}
}
