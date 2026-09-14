using Bindito.Core;
using Timberborn.ToolPanelSystem;

namespace Timberborn.AreaSelectionSystemUI;

[Context("Game")]
[Context("MapEditor")]
internal class AreaSelectionSystemUIConfigurator : Configurator
{
	private class ToolPanelModuleProvider : IProvider<ToolPanelModule>
	{
		private readonly MeasurableAreaDrawer _measurableAreaDrawer;

		public ToolPanelModuleProvider(MeasurableAreaDrawer measurableAreaDrawer)
		{
			_measurableAreaDrawer = measurableAreaDrawer;
		}

		public ToolPanelModule Get()
		{
			ToolPanelModule.Builder builder = new ToolPanelModule.Builder();
			builder.AddFragment(_measurableAreaDrawer, 100);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<MeasurableAreaDrawer>().AsSingleton();
		Bind<BlockObjectSelectionDrawerFactory>().AsSingleton();
		MultiBind<ToolPanelModule>().ToProvider<ToolPanelModuleProvider>().AsSingleton();
	}
}
