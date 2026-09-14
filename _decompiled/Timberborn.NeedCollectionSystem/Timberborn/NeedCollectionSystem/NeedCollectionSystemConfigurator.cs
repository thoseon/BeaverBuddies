using Bindito.Core;

namespace Timberborn.NeedCollectionSystem;

[Context("Game")]
internal class NeedCollectionSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<CommonNeedCollectionIdsProvider>().AsSingleton();
		MultiBind<INeedCollectionIdsProvider>().ToExisting<CommonNeedCollectionIdsProvider>();
	}
}
