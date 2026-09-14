using Bindito.Core;

namespace Timberborn.BenchmarkingUI;

[Context("Game")]
internal class BenchmarkingUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BenchmarkDebuggingPanel>().AsSingleton();
	}
}
