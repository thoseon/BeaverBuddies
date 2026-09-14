using Bindito.Core;

namespace Timberborn.GameExitSystem;

[Context("Game")]
internal class GameExitSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<GoodbyeBoxFactory>().AsSingleton();
	}
}
