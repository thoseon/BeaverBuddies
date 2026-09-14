using Bindito.Core;
using Timberborn.BlockSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.TerrainPhysics;

[Context("Game")]
[Context("MapEditor")]
internal class TerrainPhysicsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<TerrainPhysicsDeletionBlocker>().AsTransient();
		Bind<ITerrainPhysicsService>().To<TerrainPhysicsService>().AsSingleton();
		Bind<TerrainPhysicsValidatorFactory>().AsSingleton();
		Bind<TerrainPhysicsUpdater>().AsSingleton();
		Bind<TerrainDestroyer>().AsSingleton();
		Bind<TerrainAndBlockObjectsToDeleteFinder>().AsSingleton();
		Bind<TerrainOnBlockObjectFinder>().AsSingleton();
		Bind<SupportsToBeDeleted>().AsSingleton();
		Bind<TerrainPhysicsValidationEnabler>().AsSingleton();
		Bind<TerrainPhysicsPostLoader>().AsSingleton();
		MultiBind<IBlockObjectValidator>().To<TerrainPhysicsBlockObjectValidator>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BlockObject, TerrainPhysicsDeletionBlocker>();
		return builder.Build();
	}
}
