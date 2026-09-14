using Bindito.Core;
using Timberborn.StoreSystem;

namespace Timberborn.SteamStoreSystem;

[Context("Bootstrapper")]
internal class SteamStoreSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<IStore>().To<SteamStore>().AsSingleton().AsExported();
		Bind<SteamManager>().AsSingleton().AsExported();
		Bind<SteamLanguage>().AsSingleton();
	}
}
