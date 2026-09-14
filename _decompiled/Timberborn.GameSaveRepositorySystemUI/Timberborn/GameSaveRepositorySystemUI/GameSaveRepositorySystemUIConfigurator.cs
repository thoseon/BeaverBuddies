using Bindito.Core;

namespace Timberborn.GameSaveRepositorySystemUI;

[Context("MainMenu")]
[Context("Game")]
internal class GameSaveRepositorySystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<GameSaveItemElementFactory>().AsSingleton();
		Bind<GameSaveItemFactory>().AsSingleton();
		Bind<SaveThumbnailCache>().AsSingleton();
		Bind<SimpleModItemFactory>().AsSingleton();
		Bind<LoadGameBox>().AsSingleton();
		Bind<GameSaveModBox>().AsSingleton();
		Bind<SaveVersionCompatibilityService>().AsSingleton();
		Bind<ValidatingGameLoader>().AsSingleton();
		Bind<SettlementList>().AsSingleton();
		Bind<SaveList>().AsSingleton();
		MultiBind<IGameLoadValidator>().To<SaveFileValidator>().AsSingleton();
		MultiBind<IGameLoadValidator>().To<SaveVersionValidator>().AsSingleton();
		MultiBind<IGameLoadValidator>().To<SaveModsValidator>().AsSingleton();
	}
}
