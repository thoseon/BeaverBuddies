using Bindito.Core;

namespace Timberborn.AssetSystem;

[Context("Bootstrapper")]
internal class AssetSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<IAssetLoader>().To<AssetLoader>().AsSingleton().AsExported();
		MultiBind<IAssetProvider>().To<ResourceAssetProvider>().AsSingleton();
	}
}
