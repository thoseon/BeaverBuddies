using Bindito.Core;

namespace Timberborn.RootProviders;

[Context("Bootstrapper")]
internal class RootProvidersConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<RootObjectProvider>().AsSingleton().AsExported();
		Bind<RootVisualElementProvider>().AsSingleton().AsExported();
	}
}
