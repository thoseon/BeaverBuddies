using Bindito.Core;
using Timberborn.EntityPanelSystem;
using Timberborn.TemplateInstantiation;
using Timberborn.WaterBuildings;

namespace Timberborn.WaterBuildingsUI;

[Context("Game")]
internal class WaterBuildingsUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly FloodgateFragment _floodgateFragment;

		private readonly ThrottlingValveFragment _throttlingValveFragment;

		private readonly ThrottlingValveDebugFragment _throttlingValveDebugFragment;

		private readonly FillValveFragment _fillValveFragment;

		private readonly StreamGaugeFragment _streamGaugeFragment;

		private readonly WaterMoverFragment _waterMoverFragment;

		private readonly WaterInputPipeDepthFragment _waterInputPipeDepthFragment;

		public EntityPanelModuleProvider(FloodgateFragment floodgateFragment, ThrottlingValveFragment throttlingValveFragment, ThrottlingValveDebugFragment throttlingValveDebugFragment, FillValveFragment fillValveFragment, StreamGaugeFragment streamGaugeFragment, WaterMoverFragment waterMoverFragment, WaterInputPipeDepthFragment waterInputPipeDepthFragment)
		{
			_floodgateFragment = floodgateFragment;
			_throttlingValveFragment = throttlingValveFragment;
			_throttlingValveDebugFragment = throttlingValveDebugFragment;
			_fillValveFragment = fillValveFragment;
			_streamGaugeFragment = streamGaugeFragment;
			_waterMoverFragment = waterMoverFragment;
			_waterInputPipeDepthFragment = waterInputPipeDepthFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddTopFragment(_floodgateFragment);
			builder.AddTopFragment(_throttlingValveFragment);
			builder.AddDiagnosticFragment(_throttlingValveDebugFragment);
			builder.AddTopFragment(_fillValveFragment);
			builder.AddTopFragment(_streamGaugeFragment);
			builder.AddTopFragment(_waterMoverFragment);
			builder.AddTopFragment(_waterInputPipeDepthFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<FillValveMarker>().AsTransient();
		Bind<WaterOutputParticleLength>().AsTransient();
		Bind<FloodedBuildingStatus>().AsTransient();
		Bind<NeedsWaterBuildingStatus>().AsTransient();
		Bind<WaterDirectionPreviewMarker>().AsTransient();
		Bind<WaterBuildingDescriber>().AsTransient();
		Bind<WaterInputPipeSpecDescriber>().AsTransient();
		Bind<WaterOutputParticle>().AsTransient();
		Bind<WaterOutputParticleColorer>().AsTransient();
		Bind<FloodgateFragment>().AsSingleton();
		Bind<ThrottlingValveFragment>().AsSingleton();
		Bind<ThrottlingValveDebugFragment>().AsSingleton();
		Bind<FillValveFragment>().AsSingleton();
		Bind<StreamGaugeFragment>().AsSingleton();
		Bind<WaterMoverToggleFactory>().AsSingleton();
		Bind<WaterMoverFragment>().AsSingleton();
		Bind<WaterInputPipeDepthFragment>().AsSingleton();
		Bind<WaterOutputParticleColors>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<FloodableBuilding, FloodedBuildingStatus>();
		builder.AddDecorator<IWaterNeedingBuilding, NeedsWaterBuildingStatus>();
		builder.AddDecorator<IWaterNeedingBuilding, WaterBuildingDescriber>();
		builder.AddDecorator<WaterInput, WaterBuildingDescriber>();
		builder.AddDecorator<WaterInputPipeSpec, WaterInputPipeSpecDescriber>();
		builder.AddDecorator<StreamGauge, WaterBuildingDescriber>();
		builder.AddDecorator<WaterWheelSpec, WaterBuildingDescriber>();
		builder.AddDecorator<WaterOutputParticleSpec, WaterOutputParticle>();
		builder.AddDecorator<WaterOutputParticle, WaterOutputParticleColorer>();
		builder.AddDecorator<WaterOutputParticle, WaterOutputParticleLength>();
		builder.AddDecorator<ThrottlingValve, WaterDirectionPreviewMarker>();
		builder.AddDecorator<FillValve, WaterDirectionPreviewMarker>();
		builder.AddDecorator<FillValve, FillValveMarker>();
		return builder.Build();
	}
}
