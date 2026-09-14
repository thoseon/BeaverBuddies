using Bindito.Core;

namespace Timberborn.AnalyticsUI;

[Context("MainMenu")]
internal class AnalyticsUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<AnalyticsConsentBox>().AsSingleton();
	}
}
