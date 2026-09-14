using Bindito.Core;
using Timberborn.BlueprintPrefabSystem;

namespace Timberborn.UnityEngineSpecs;

[Context("Game")]
[Context("MapEditor")]
internal class UnityEngineSpecsConfigurator : Configurator
{
	protected override void Configure()
	{
		MultiBind<ISpecToPrefabConverter>().To<TransformSpecPrefabConverter>().AsSingleton();
		MultiBind<ISpecToPrefabConverter>().To<CollidersSpecPrefabConverter>().AsSingleton();
	}
}
