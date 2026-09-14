using Bindito.Core;

namespace Timberborn.TickSystemUI;

[Context("Game")]
internal class TickSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ParallelSingletonDebuggingPanel>().AsSingleton();
	}
}
