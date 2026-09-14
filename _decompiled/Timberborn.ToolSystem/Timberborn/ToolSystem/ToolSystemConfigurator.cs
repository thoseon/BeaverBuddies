using Bindito.Core;

namespace Timberborn.ToolSystem;

[Context("Game")]
[Context("MapEditor")]
internal class ToolSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ToolService>().AsSingleton();
		Bind<ToolGroupService>().AsSingleton();
		Bind<ToolUnlockingService>().AsSingleton();
	}
}
