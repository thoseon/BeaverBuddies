using Bindito.Core;
using Timberborn.BlueprintPrefabSystem;

namespace Timberborn.Timbermesh;

[Context("Game")]
[Context("MapEditor")]
internal class TimbermeshConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<TimbermeshImporter>().AsSingleton();
		Bind<StaticMeshBuilder>().AsSingleton();
		MultiBind<ISpecToPrefabConverter>().To<TimbermeshSpecConverter>().AsSingleton();
	}
}
