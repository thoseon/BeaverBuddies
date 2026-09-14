using Bindito.Core;

namespace Timberborn.BlueprintPrefabSystem;

[Context("Game")]
[Context("MapEditor")]
internal class BlueprintPrefabSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BlueprintPrefabConverter>().AsSingleton();
	}
}
