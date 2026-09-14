using Bindito.Core;

namespace Timberborn.FactionSystem;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class FactionSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<FactionSpecService>().AsSingleton();
		Bind<FactionUnlockingService>().AsSingleton();
		Bind<FactionUnlockConditionDescriber>().AsSingleton();
	}
}
