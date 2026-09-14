using Bindito.Core;

namespace Timberborn.ResourceCountingSystemUI;

[Context("Game")]
internal class ResourceCountingSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ContextualResourceCountingService>().AsSingleton();
	}
}
