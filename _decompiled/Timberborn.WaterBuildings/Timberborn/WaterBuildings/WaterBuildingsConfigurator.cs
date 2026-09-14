using Bindito.Core;
using Timberborn.Automation;
using Timberborn.Buildings;
using Timberborn.Particles;
using Timberborn.PathSystem;
using Timberborn.Rendering;
using Timberborn.TemplateInstantiation;
using Timberborn.WaterObjects;

namespace Timberborn.WaterBuildings;

[Context("Game")]
internal class WaterBuildingsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<StreamGauge>().AsTransient();
		Bind<TickableWaterBuilding>().AsTransient();
		Bind<WaterMover>().AsTransient();
		Bind<WaterMoverParticleController>().AsTransient();
		Bind<WaterNeeder>().AsTransient();
		Bind<AccessibleFloodableBuilding>().AsTransient();
		Bind<FloodableBuilding>().AsTransient();
		Bind<FloodableFire>().AsTransient();
		Bind<FloodedBuildingPreviewValidator>().AsTransient();
		Bind<Floodgate>().AsTransient();
		Bind<FloodgateAnimationController>().AsTransient();
		Bind<FloodgateGateCutoff>().AsTransient();
		Bind<PreviewWaterInputPipeBlocker>().AsTransient();
		Bind<WaterObstacleController>().AsTransient();
		Bind<StreamGaugeAnimationController>().AsTransient();
		Bind<ThrottlingValve>().AsTransient();
		Bind<FillValve>().AsTransient();
		Bind<WaterInput>().AsTransient();
		Bind<WaterInputPipeCoordinates>().AsTransient();
		Bind<WaterInputPipe>().AsTransient();
		Bind<WaterInputPipeSegmentCreator>().AsTransient();
		Bind<WaterOutput>().AsTransient();
		Bind<WaterInputPipeValidator>().AsTransient();
		Bind<WaterInputPipeObstructedStatus>().AsTransient();
		Bind<WaterInputFixedCoordinates>().AsTransient();
		Bind<FloodgateSynchronizer>().AsSingleton();
		Bind<PreviewWaterInputPipeBlockerService>().AsSingleton();
		Bind<ThrottlingValveSynchronizer>().AsSingleton();
		Bind<FillValveSynchronizer>().AsSingleton();
		Bind<WaterInputService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<WaterInputSpec, WaterInput>();
		builder.AddDecorator<WaterInputPipeSpec, WaterInputPipe>();
		builder.AddDecorator<WaterInputPipe, WaterInputPipeCoordinates>();
		builder.AddDecorator<WaterInputPipe, EntityMaterials>();
		builder.AddDecorator<WaterInputPipe, WaterInputPipeSegmentCreator>();
		builder.AddDecorator<WaterInputFixedSpec, WaterInputFixedCoordinates>();
		builder.AddDecorator<BuildingSpec, PreviewWaterInputPipeBlocker>();
		builder.AddDecorator<BuildingAccessible, AccessibleFloodableBuilding>();
		builder.AddDecorator<BuildingAccessible, BlockableFloodableObject>();
		builder.AddDecorator<WaterMoverSpec, WaterMover>();
		builder.AddDecorator<FloodgateSpec, Floodgate>();
		builder.AddDecorator<Floodgate, FloodgateGateCutoff>();
		builder.AddDecorator<Floodgate, AutoAutomatableNeeder>();
		builder.AddDecorator<ThrottlingValveSpec, ThrottlingValve>();
		builder.AddDecorator<ThrottlingValve, WaterObstacleController>();
		builder.AddDecorator<ThrottlingValve, AutoAutomatableNeeder>();
		builder.AddDecorator<FillValveSpec, FillValve>();
		builder.AddDecorator<FillValve, WaterObstacleController>();
		builder.AddDecorator<FillValve, AutoAutomatableNeeder>();
		builder.AddDecorator<ImpermeableFloorSpec, DynamicPathModelUpdater>();
		builder.AddDecorator<FloodableBuildingSpec, FloodableBuilding>();
		builder.AddDecorator<AccessibleFloodableBuilding, FloodableBuilding>();
		builder.AddDecorator<FloodgateAnimationControllerSpec, FloodgateAnimationController>();
		builder.AddDecorator<StreamGaugeSpec, StreamGauge>();
		builder.AddDecorator<StreamGaugeAnimationControllerSpec, StreamGaugeAnimationController>();
		builder.AddDecorator<WaterMoverParticleControllerSpec, WaterMoverParticleController>();
		builder.AddDecorator<WaterMoverParticleController, ParticlesCache>();
		builder.AddDecorator<WaterOutputSpec, WaterOutput>();
		builder.AddDecorator<TickableWaterBuildingSpec, TickableWaterBuilding>();
		builder.AddDecorator<FloodableFireSpec, FloodableFire>();
		builder.AddDecorator<WaterNeederSpec, WaterNeeder>();
		builder.AddDecorator<FloodableBuilding, FloodedBuildingPreviewValidator>();
		builder.AddDecorator<FloodableBuilding, FloodableObject>();
		builder.AddDecorator<WaterInputPipe, WaterInputPipeValidator>();
		builder.AddDecorator<WaterInputPipe, WaterInputPipeObstructedStatus>();
		return builder.Build();
	}
}
