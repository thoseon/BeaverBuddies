using Bindito.Core;

namespace Timberborn.Persistence;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class PersistenceConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<InvariantDateTimeSerializer>().AsSingleton();
	}
}
