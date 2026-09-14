using Bindito.Core;
using Timberborn.Automation;
using Timberborn.Buildings;
using Timberborn.EntityNaming;
using Timberborn.Illumination;
using Timberborn.Navigation;
using Timberborn.TemplateInstantiation;

namespace Timberborn.AutomationBuildings;

[Context("Game")]
internal class AutomationBuildingsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<DepthSensor>().AsTransient();
		Bind<ContaminationSensor>().AsTransient();
		Bind<FlowSensor>().AsTransient();
		Bind<ScienceCounter>().AsTransient();
		Bind<ResourceCounter>().AsTransient();
		Bind<ResourceCounterBannerSetter>().AsTransient();
		Bind<PopulationCounter>().AsTransient();
		Bind<PowerMeter>().AsTransient();
		Bind<Lever>().AsTransient();
		Bind<LeverModel>().AsTransient();
		Bind<Relay>().AsTransient();
		Bind<Memory>().AsTransient();
		Bind<WeatherStation>().AsTransient();
		Bind<PausableBuildingTerminal>().AsTransient();
		Bind<Chronometer>().AsTransient();
		Bind<AutoAutomatableNeeder>().AsTransient();
		Bind<Gate>().AsTransient();
		Bind<GatePlacement>().AsTransient();
		Bind<GateNavMeshBlocker>().AsTransient();
		Bind<GateModel>().AsTransient();
		Bind<Timer>().AsTransient();
		Bind<Detonator>().AsTransient();
		Bind<Indicator>().AsTransient();
		Bind<Speaker>().AsTransient();
		Bind<SpeakerAnimationController>().AsTransient();
		Bind<TimerModel>().AsTransient();
		Bind<SamplingPopulationService>().AsSingleton();
		Bind<SamplingResourcesService>().AsSingleton();
		Bind<SpringReturnService>().AsSingleton();
		Bind<GateUpdater>().AsSingleton();
		Bind<SpeakerPlayer>().AsSingleton();
		Bind<SpeakerSoundService>().AsSingleton();
		Bind<SpeakerBuiltinSounds>().AsSingleton();
		Bind<SpeakerCustomSoundLoader>().AsSingleton();
		Bind<TimerIntervalSerializer>().AsSingleton();
		Bind<TimerIntervalFactory>().AsSingleton();
		MultiBind<IPathTransformer>().To<GatePathTransformer>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<DepthSensorSpec, DepthSensor>();
		builder.AddDecorator<DepthSensor, AutomatorIlluminator>();
		builder.AddDecorator<ContaminationSensorSpec, ContaminationSensor>();
		builder.AddDecorator<ContaminationSensor, AutomatorIlluminator>();
		builder.AddDecorator<FlowSensorSpec, FlowSensor>();
		builder.AddDecorator<FlowSensor, AutomatorIlluminator>();
		builder.AddDecorator<LeverSpec, Lever>();
		builder.AddDecorator<Lever, AutomatorIlluminator>();
		builder.AddDecorator<LeverModelSpec, LeverModel>();
		builder.AddDecorator<RelaySpec, Relay>();
		builder.AddDecorator<Relay, AutomatorIlluminator>();
		builder.AddDecorator<MemorySpec, Memory>();
		builder.AddDecorator<Memory, AutomatorIlluminator>();
		builder.AddDecorator<WeatherStationSpec, WeatherStation>();
		builder.AddDecorator<WeatherStation, AutomatorIlluminator>();
		builder.AddDecorator<IFinishedPausable, PausableBuildingTerminal>();
		builder.AddDecorator<PausableBuildingTerminal, AutoAutomatableNeeder>();
		builder.AddDecorator<ChronometerSpec, Chronometer>();
		builder.AddDecorator<Chronometer, AutomatorIlluminator>();
		builder.AddDecorator<ScienceCounterSpec, ScienceCounter>();
		builder.AddDecorator<ScienceCounter, AutomatorIlluminator>();
		builder.AddDecorator<ResourceCounterSpec, ResourceCounter>();
		builder.AddDecorator<ResourceCounter, AutomatorIlluminator>();
		builder.AddDecorator<ResourceCounter, ResourceCounterBannerSetter>();
		builder.AddDecorator<PopulationCounterSpec, PopulationCounter>();
		builder.AddDecorator<PopulationCounter, AutomatorIlluminator>();
		builder.AddDecorator<PowerMeterSpec, PowerMeter>();
		builder.AddDecorator<PowerMeter, AutomatorIlluminator>();
		builder.AddDecorator<GateSpec, Gate>();
		builder.AddDecorator<Gate, GatePlacement>();
		builder.AddDecorator<Gate, GateNavMeshBlocker>();
		builder.AddDecorator<Gate, Illuminator>();
		builder.AddDecorator<GateModelSpec, GateModel>();
		builder.AddDecorator<TimerSpec, Timer>();
		builder.AddDecorator<Timer, AutomatorIlluminator>();
		builder.AddDecorator<Timer, TimerModel>();
		builder.AddDecorator<DetonatorSpec, Detonator>();
		builder.AddDecorator<Detonator, Illuminator>();
		builder.AddDecorator<IndicatorSpec, Indicator>();
		builder.AddDecorator<Indicator, Illuminator>();
		builder.AddDecorator<Indicator, CustomizableIlluminator>();
		builder.AddDecorator<Indicator, NumberedEntityNamer>();
		builder.AddDecorator<SpeakerSpec, Speaker>();
		builder.AddDecorator<Speaker, Illuminator>();
		builder.AddDecorator<Speaker, SpeakerAnimationController>();
		return builder.Build();
	}
}
