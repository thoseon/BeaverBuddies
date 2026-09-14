using Bindito.Core;

namespace Timberborn.Analytics;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class AnalyticsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<AnalyticsConsent>().AsSingleton();
	}
}
