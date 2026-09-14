using Bindito.Core;

namespace Timberborn.GoodCollectionSystem;

[Context("Game")]
[Context("MapEditor")]
internal class GoodCollectionSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<CommonGoodCollectionIdsProvider>().AsSingleton();
		MultiBind<IGoodCollectionIdsProvider>().ToExisting<CommonGoodCollectionIdsProvider>();
	}
}
