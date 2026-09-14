using Bindito.Core;
using Timberborn.Autosaving;
using Timberborn.BlockSystem;

namespace Timberborn.GameStartup;

[Context("Game")]
internal class GameStartupConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<StartingBuildingToolDescriber>().AsSingleton();
		Bind<StartingBuildingToolShower>().AsSingleton();
		Bind<StartingGoodsProvider>().AsSingleton();
		Bind<GameStarter>().AsSingleton();
		Bind<GameInitializer>().AsSingleton();
		Bind<StartingBuildingInitializer>().AsSingleton();
		Bind<StartingBeaversInitializer>().AsSingleton();
		Bind<StartingBuildingSpawner>().AsSingleton();
		Bind<StartingBuildingToolFactory>().AsSingleton();
		Bind<StartingBuildingPlacer>().AsSingleton();
		MultiBind<IBlockObjectValidator>().To<StartingBuildingPlacementValidator>().AsSingleton();
		MultiBind<IAutosaveBlocker>().To<GameStartupAutosaveBlocker>().AsSingleton();
	}
}
