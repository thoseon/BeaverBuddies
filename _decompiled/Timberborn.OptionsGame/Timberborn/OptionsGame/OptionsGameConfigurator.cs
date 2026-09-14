using Bindito.Core;
using Timberborn.Options;

namespace Timberborn.OptionsGame;

[Context("Game")]
internal class OptionsGameConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<IOptionsBox>().To<GameOptionsBox>().AsSingleton();
	}
}
