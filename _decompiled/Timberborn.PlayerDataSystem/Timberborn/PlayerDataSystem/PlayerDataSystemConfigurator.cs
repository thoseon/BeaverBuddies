using Bindito.Core;

namespace Timberborn.PlayerDataSystem;

[Context("Bootstrapper")]
internal class PlayerDataSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<IPlayerDataService>().To<PlayerDataService>().AsSingleton().AsExported();
		Bind<PlayerDataSerializer>().AsSingleton();
		Bind<PlayerDataFileService>().AsSingleton();
	}
}
