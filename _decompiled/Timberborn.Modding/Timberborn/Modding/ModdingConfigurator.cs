using Bindito.Core;

namespace Timberborn.Modding;

[Context("Bootstrapper")]
internal class ModdingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ModRepository>().AsSingleton().AsExported();
		Bind<ModSorter>().AsSingleton().AsExported();
		Bind<ModLoader>().AsSingleton();
		Bind<ManifestLoader>().AsSingleton();
		MultiBind<IModsProvider>().To<UserFolderModsProvider>().AsSingleton();
	}
}
