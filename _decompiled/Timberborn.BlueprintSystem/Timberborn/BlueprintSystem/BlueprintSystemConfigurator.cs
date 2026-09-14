using Bindito.Core;

namespace Timberborn.BlueprintSystem;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class BlueprintSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BasicDeserializer>().AsSingleton();
		Bind<AdvancedDeserializer>().AsSingleton();
		Bind<AssetRefDeserializer>().AsSingleton();
		Bind<BlueprintDeserializer>().AsSingleton();
		Bind<BlueprintFileBundleLoader>().AsSingleton();
		Bind<BlueprintSourceService>().AsSingleton();
		Bind<ISpecService>().To<SpecService>().AsSingleton();
	}
}
