using Bindito.Core;

namespace Timberborn.PrefabOptimization;

[Context("Game")]
[Context("MapEditor")]
internal class PrefabOptimizationConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<IPrefabOptimizationChain>().ToProvider<PrefabOptimizationChainProvider>().AsSingleton();
		Bind<OptimizedPrefabInstantiator>().AsSingleton();
		Bind<TimbermeshPrefabOptimizer>().AsSingleton();
		Bind<VertexColorPrefabOptimizer>().AsSingleton();
		Bind<MergeMeshesByMaterialPrefabOptimizer>().AsSingleton();
		Bind<DestroyEmptyChildrenPrefabOptimizer>().AsSingleton();
		Bind<AutoAtlasingPrefabOptimizer>().AsSingleton();
		Bind<VerticalShapeBuilder>().AsSingleton();
		Bind<AutoAtlaser>().AsSingleton();
		Bind<MaterialPropertyProvider>().AsSingleton();
	}
}
