using Bindito.Core;
using Timberborn.AreaSelectionSystem;
using Timberborn.ConstructionSites;
using Timberborn.Rendering;
using Timberborn.TemplateInstantiation;
using Timberborn.TerrainLevelValidation;

namespace Timberborn.Terraforming;

[Context("Game")]
internal class TerraformingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<Drill>().AsTransient();
		Bind<DrillHeadVisualizer>().AsTransient();
		Bind<DrillScrewBuilder>().AsTransient();
		Bind<DrillScrewRotator>().AsTransient();
		Bind<GroundRaiser>().AsTransient();
		Bind<TerraformingDirectionalBlocker>().AsTransient();
		Bind<GroundRaisingService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<GroundRaiserSpec, GroundRaiser>();
		builder.AddDecorator<GroundRaiser, TerraformingDirectionalBlocker>();
		builder.AddDecorator<GroundRaiser, TopTerrainLevelValidationConstraint>();
		builder.AddDecorator<GroundRaiser, DeleteOnFinishConstructionSite>();
		builder.AddDecorator<GroundRaiser, PhysicallySupportedConstructionSite>();
		builder.AddDecorator<DrillScrewBuilderSpec, DrillScrewBuilder>();
		builder.AddDecorator<DrillScrewBuilder, DrillScrewRotator>();
		builder.AddDecorator<DrillScrewBuilder, EntityMaterials>();
		builder.AddDecorator<DrillHeadVisualizerSpec, DrillHeadVisualizer>();
		builder.AddDecorator<DrillSpec, Drill>();
		builder.AddDecorator<Slope, AreaBoundsDrawingBlocker>();
		return builder.Build();
	}
}
