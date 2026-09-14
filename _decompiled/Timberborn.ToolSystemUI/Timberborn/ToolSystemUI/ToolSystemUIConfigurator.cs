using Bindito.Core;
using Timberborn.ToolPanelSystem;

namespace Timberborn.ToolSystemUI;

[Context("Game")]
[Context("MapEditor")]
internal class ToolSystemUIConfigurator : Configurator
{
	private class ToolPanelModuleProvider : IProvider<ToolPanelModule>
	{
		private readonly DescriptionPanelController _descriptionPanelController;

		public ToolPanelModuleProvider(DescriptionPanelController descriptionPanelController)
		{
			_descriptionPanelController = descriptionPanelController;
		}

		public ToolPanelModule Get()
		{
			ToolPanelModule.Builder builder = new ToolPanelModule.Builder();
			builder.AddFragment(_descriptionPanelController, 10);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<DescriptionPanelController>().AsSingleton();
		Bind<DescriptionPanels>().AsSingleton();
		Bind<PanelToolSwitcher>().AsSingleton();
		Bind<ToolWaterToggler>().AsSingleton();
		MultiBind<ToolPanelModule>().ToProvider<ToolPanelModuleProvider>().AsSingleton();
	}
}
