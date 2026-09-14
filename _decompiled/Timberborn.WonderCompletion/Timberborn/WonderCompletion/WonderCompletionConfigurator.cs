using Bindito.Core;

namespace Timberborn.WonderCompletion;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class WonderCompletionConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<WonderCompletionService>().AsSingleton();
	}
}
