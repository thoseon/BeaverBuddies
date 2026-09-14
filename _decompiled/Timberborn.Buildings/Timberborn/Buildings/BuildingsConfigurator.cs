using Bindito.Core;
using Timberborn.BlockingSystem;
using Timberborn.Particles;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Buildings;

[Context("Game")]
[Context("MapEditor")]
internal class BuildingsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BuildingAccessible>().AsTransient();
		Bind<BuildingBlockedAccessible>().AsTransient();
		Bind<BuildingModel>().AsTransient();
		Bind<BuildingModelGroundCutoff>().AsTransient();
		Bind<BuildingSelectionSound>().AsTransient();
		Bind<BuildingSounds>().AsTransient();
		Bind<BuildingTerrainCutout>().AsTransient();
		Bind<Fire>().AsTransient();
		Bind<FireIntensityController>().AsTransient();
		Bind<UncoveredModelSwitcher>().AsTransient();
		Bind<Building>().AsTransient();
		Bind<PausableBuilding>().AsTransient();
		Bind<BuildingSoundController>().AsTransient();
		Bind<BuildingDetailTexture>().AsTransient();
		Bind<BuildingService>().AsSingleton();
		Bind<BuildingModelUpdater>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BuildingSpec, Building>();
		builder.AddDecorator<BuildingSpec, BuildingSelectionSound>();
		builder.AddDecorator<BuildingSpec, BuildingSounds>();
		builder.AddDecorator<BuildingAccessibleSpec, BuildingAccessible>();
		builder.AddDecorator<BuildingAccessible, BuildingBlockedAccessible>();
		builder.AddDecorator<BuildingModelSpec, BuildingModel>();
		builder.AddDecorator<BuildingModelGroundCutoffSpec, BuildingModelGroundCutoff>();
		builder.AddDecorator<BuildingTerrainCutoutSpec, BuildingTerrainCutout>();
		builder.AddDecorator<FireSpec, Fire>();
		builder.AddDecorator<Fire, ParticlesCache>();
		builder.AddDecorator<UncoveredModelSwitcherSpec, UncoveredModelSwitcher>();
		builder.AddDecorator<BuildingSpec, BlockableObject>();
		builder.AddDecorator<BuildingSpec, PausableBuilding>();
		builder.AddDecorator<BuildingDetailTextureSpec, BuildingDetailTexture>();
		return builder.Build();
	}
}
