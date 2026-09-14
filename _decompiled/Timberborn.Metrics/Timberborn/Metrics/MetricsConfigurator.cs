using Bindito.Core;

namespace Timberborn.Metrics;

[Context("Game")]
[Context("MapEditor")]
internal class MetricsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<IMetricsService>().To<MetricsService>().AsSingleton();
		Bind<MetricsRepository>().AsSingleton();
		Bind<MetricsFormatter>().AsSingleton();
	}
}
