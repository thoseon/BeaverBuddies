using Bindito.Core;
using Timberborn.BlueprintSystem;
using Timberborn.GoodCollectionSystem;
using Timberborn.NeedCollectionSystem;
using Timberborn.TemplateCollectionSystem;
using Timberborn.TimbermeshMaterials;

namespace Timberborn.GameFactionSystem;

[Context("Game")]
internal class GameFactionSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<FactionService>().AsSingleton();
		Bind<FactionNeedService>().AsSingleton();
		Bind<NeedModificationService>().AsSingleton();
		Bind<FactionBlueprintModifierProvider>().AsSingleton();
		Bind<NeedVerifier>().AsSingleton();
		MultiBind<ITemplateCollectionIdProvider>().To<FactionTemplateCollectionIdProvider>().AsSingleton();
		MultiBind<IMaterialCollectionIdsProvider>().To<FactionMaterialCollectionIdsProvider>().AsSingleton();
		MultiBind<IBlueprintModifierProvider>().ToExisting<FactionBlueprintModifierProvider>();
		MultiBind<IGoodCollectionIdsProvider>().To<FactionGoodCollectionIdsProvider>().AsSingleton();
		MultiBind<INeedCollectionIdsProvider>().To<FactionNeedCollectionIdsProvider>().AsSingleton();
	}
}
