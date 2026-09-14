using Bindito.Core;

namespace Timberborn.Benchmarking;

[Context("Game")]
internal class BenchmarkingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<Benchmarker>().AsSingleton();
		Bind<SavingBenchmarker>().AsSingleton();
		Bind<PerformanceSampler>().AsSingleton();
		Bind<BenchmarkReportCreator>().AsSingleton();
		Bind<StatisticsCalculator>().AsSingleton();
		Bind<BenchmarkLogger>().AsSingleton();
		Bind<PerformanceSampleFormatter>().AsSingleton();
		Bind<BenchmarkPanel>().AsSingleton();
		Bind<BenchmarkStarter>().AsSingleton();
	}
}
