using Bindito.Core;

namespace Timberborn.TooltipSystem;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class TooltipSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MouseTooltipPositioner>().AsSingleton();
		Bind<TooltipBlocker>().AsSingleton();
		Bind<ITooltipRegistrar>().To<TooltipRegistrar>().AsSingleton();
		Bind<Tooltip>().AsSingleton();
		Bind<TooltipContainer>().AsSingleton();
	}
}
