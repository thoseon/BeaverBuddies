using Bindito.Core;

namespace Timberborn.UISound;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class UISoundConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<UISoundController>().AsSingleton();
	}
}
