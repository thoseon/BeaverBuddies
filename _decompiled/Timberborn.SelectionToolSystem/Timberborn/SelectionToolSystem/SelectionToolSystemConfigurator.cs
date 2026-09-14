using Bindito.Core;

namespace Timberborn.SelectionToolSystem;

[Context("Game")]
internal class SelectionToolSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<SelectionToolProcessorFactory>().AsSingleton();
	}
}
