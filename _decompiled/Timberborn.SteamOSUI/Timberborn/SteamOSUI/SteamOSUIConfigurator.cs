using Bindito.Core;
using Timberborn.CoreUI;

namespace Timberborn.SteamOSUI;

[Context("Game")]
[Context("MapEditor")]
[Context("MainMenu")]
internal class SteamOSUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SteamDeckUIScaleSetter>().AsSingleton();
		MultiBind<IVisualElementInitializer>().To<SteamOnScreenKeyboardController>().AsSingleton();
	}
}
