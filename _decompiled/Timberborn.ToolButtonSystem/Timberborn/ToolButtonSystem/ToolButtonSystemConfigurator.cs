using Bindito.Core;

namespace Timberborn.ToolButtonSystem;

[Context("Game")]
[Context("MapEditor")]
internal class ToolButtonSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ToolButtonFactory>().AsSingleton();
		Bind<ToolButtonService>().AsSingleton();
		Bind<ToolGroupButtonFactory>().AsSingleton();
		Bind<ToolButtonSelector>().AsSingleton();
		Bind<ToolbarButtonRetriever>().AsSingleton();
	}
}
