using Bindito.Core;
using Timberborn.SaveSystem;

namespace Timberborn.WorldPersistence;

[Context("Game")]
[Context("MapEditor")]
internal class WorldPersistenceConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ReferenceSerializer>().AsSingleton();
		Bind<WorldEntitiesLoader>().AsSingleton();
		Bind<EntitiesLoader>().AsSingleton();
		Bind<SerializedWorldFactory>().AsSingleton();
		Bind<ISingletonLoader>().To<WorldSingletonLoader>().AsSingleton();
		MultiBind<ISaveEntryWriter>().To<WorldEntryWriter>().AsSingleton();
	}
}
