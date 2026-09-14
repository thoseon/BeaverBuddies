using Bindito.Core;
using Timberborn.BlockSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.TerrainLevelValidation;

[Context("Game")]
[Context("MapEditor")]
internal class TerrainLevelValidationConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ContinuousTerrainConstraint>().AsTransient();
		Bind<BottomTerrainLevelValidationConstraint>().AsTransient();
		Bind<TopTerrainLevelValidationConstraint>().AsTransient();
		Bind<ContinuousTerrainConstraintValidator>().AsSingleton();
		MultiBind<IBlockObjectValidator>().To<TerrainLevelValidator>().AsSingleton();
		MultiBind<IBlockObjectValidator>().To<UndergroundTerrainValidator>().AsSingleton();
		MultiBind<IBlockObjectValidator>().ToExisting<ContinuousTerrainConstraintValidator>();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<ContinuousTerrainConstraintSpec, ContinuousTerrainConstraint>();
		return builder.Build();
	}
}
