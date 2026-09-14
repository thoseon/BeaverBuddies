using Bindito.Core;

namespace Timberborn.WebNavigation;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class WebNavigationConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<UrlOpener>().AsSingleton();
	}
}
