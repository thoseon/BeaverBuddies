using Bindito.Core;

namespace Timberborn.BottomBarSystem;

[Context("Game")]
[Context("MapEditor")]
internal class BottomBarSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BottomBarPanel>().AsSingleton();
	}
}
