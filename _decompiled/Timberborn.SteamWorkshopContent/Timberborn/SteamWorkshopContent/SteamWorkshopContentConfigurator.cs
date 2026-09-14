using Bindito.Core;

namespace Timberborn.SteamWorkshopContent;

[Context("Bootstrapper")]
internal class SteamWorkshopContentConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SteamWorkshopContentProvider>().AsSingleton().AsExported();
	}
}
