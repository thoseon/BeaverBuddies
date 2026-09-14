using Bindito.Core;

namespace Timberborn.ExperimentalModeSystem;

[Context("Bootstrapper")]
internal class ExperimentalModeSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ExperimentalMode>().AsSingleton().AsExported();
	}
}
