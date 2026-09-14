using Bindito.Core;

namespace Timberborn.WorldSerialization;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class WorldSerializationConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<WorldSerializer>().AsSingleton();
	}
}
