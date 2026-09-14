using Bindito.Core;

namespace Timberborn.NewGameConfigurationSystem;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class NewGameConfigurationSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<GameModeSpecService>().AsSingleton();
	}
}
