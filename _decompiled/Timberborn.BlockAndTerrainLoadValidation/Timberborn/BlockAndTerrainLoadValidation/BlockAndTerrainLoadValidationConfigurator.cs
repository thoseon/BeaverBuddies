using Bindito.Core;
using Timberborn.WorldPersistence;

namespace Timberborn.BlockAndTerrainLoadValidation;

[Context("Game")]
[Context("MapEditor")]
internal class BlockAndTerrainLoadValidationConfigurator : Configurator
{
	protected override void Configure()
	{
		MultiBind<IEntityBatchLoader>().To<BlockAndTerrainBatchLoader>().AsSingleton();
	}
}
