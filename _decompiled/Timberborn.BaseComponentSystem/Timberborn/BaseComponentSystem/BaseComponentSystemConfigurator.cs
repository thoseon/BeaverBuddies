using Bindito.Core;

namespace Timberborn.BaseComponentSystem;

[Context("Game")]
[Context("MapEditor")]
internal class BaseComponentSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BaseInstantiator>().AsSingleton();
		Bind<ComponentCacheService>().AsSingleton();
		Bind<TypeBlacklist>().AsSingleton();
	}
}
