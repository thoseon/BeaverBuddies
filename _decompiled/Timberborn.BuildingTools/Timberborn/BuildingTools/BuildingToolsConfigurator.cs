using Bindito.Core;
using Timberborn.BlockObjectTools;
using Timberborn.ToolSystem;

namespace Timberborn.BuildingTools;

[Context("Game")]
[Context("MapEditor")]
internal class BuildingToolsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BuildingCostSectionProvider>().AsSingleton();
		Bind<UnlockSectionController>().AsSingleton();
		MultiBind<IToolLocker>().To<BuildingToolLocker>().AsSingleton();
		MultiBind<IBlockObjectPlacer>().To<BuildingPlacer>().AsSingleton();
	}
}
