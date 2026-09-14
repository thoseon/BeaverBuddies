using Bindito.Core;
using Timberborn.ToolPanelSystem;

namespace Timberborn.MapEditorNaturalResourcesUI;

[Context("MapEditor")]
internal class MapEditorNaturalResourcesUIConfigurator : Configurator
{
	private class ToolPanelModuleProvider : IProvider<ToolPanelModule>
	{
		private readonly NaturalResourceSpawningBrushPanel _naturalResourceSpawningBrushPanel;

		public ToolPanelModuleProvider(NaturalResourceSpawningBrushPanel naturalResourceSpawningBrushPanel)
		{
			_naturalResourceSpawningBrushPanel = naturalResourceSpawningBrushPanel;
		}

		public ToolPanelModule Get()
		{
			ToolPanelModule.Builder builder = new ToolPanelModule.Builder();
			builder.AddFragment(_naturalResourceSpawningBrushPanel, 50);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<NaturalResourceSpawningBrushTool>().AsSingleton();
		Bind<NaturalResourceRemovalBrushTool>().AsSingleton();
		Bind<NaturalResourceLayerToggle>().AsSingleton();
		Bind<NaturalResourceSpawningBrushPanel>().AsSingleton();
		Bind<NaturalResourceBrushIterator>().AsSingleton();
		MultiBind<ToolPanelModule>().ToProvider<ToolPanelModuleProvider>().AsSingleton();
	}
}
