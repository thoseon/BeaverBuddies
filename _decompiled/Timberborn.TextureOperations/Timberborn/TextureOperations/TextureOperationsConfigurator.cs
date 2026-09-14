using Bindito.Core;

namespace Timberborn.TextureOperations;

[Context("Bootstrapper")]
internal class TextureOperationsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<TextureFactory>().AsSingleton().AsExported();
	}
}
