using Bindito.Core;
using Timberborn.AutomationBuildings;
using Timberborn.EntityPanelSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.AutomationBuildingsUI;

[Context("Game")]
internal class AutomationBuildingsUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly LeverFragment _leverFragment;

		private readonly RelayFragment _relayFragment;

		private readonly MemoryFragment _memoryFragment;

		private readonly WeatherStationFragment _weatherStationFragment;

		private readonly ChronometerFragment _chronometerFragment;

		private readonly ScienceCounterFragment _scienceCounterFragment;

		private readonly ResourceCounterFragment _resourceCounterFragment;

		private readonly PopulationCounterFragment _populationCounterFragment;

		private readonly PowerMeterFragment _powerMeterFragment;

		private readonly TimerFragment _timerFragment;

		private readonly GateFragment _gateFragment;

		private readonly IndicatorFragment _indicatorFragment;

		private readonly SpeakerFragment _speakerFragment;

		private readonly DepthSensorFragment _depthSensorFragment;

		private readonly ContaminationSensorFragment _contaminationSensorFragment;

		private readonly FlowSensorFragment _flowSensorFragment;

		public EntityPanelModuleProvider(LeverFragment leverFragment, RelayFragment relayFragment, MemoryFragment memoryFragment, WeatherStationFragment weatherStationFragment, ChronometerFragment chronometerFragment, ScienceCounterFragment scienceCounterFragment, ResourceCounterFragment resourceCounterFragment, PopulationCounterFragment populationCounterFragment, PowerMeterFragment powerMeterFragment, TimerFragment timerFragment, GateFragment gateFragment, IndicatorFragment indicatorFragment, SpeakerFragment speakerFragment, DepthSensorFragment depthSensorFragment, ContaminationSensorFragment contaminationSensorFragment, FlowSensorFragment flowSensorFragment)
		{
			_leverFragment = leverFragment;
			_relayFragment = relayFragment;
			_memoryFragment = memoryFragment;
			_weatherStationFragment = weatherStationFragment;
			_chronometerFragment = chronometerFragment;
			_scienceCounterFragment = scienceCounterFragment;
			_resourceCounterFragment = resourceCounterFragment;
			_populationCounterFragment = populationCounterFragment;
			_powerMeterFragment = powerMeterFragment;
			_timerFragment = timerFragment;
			_gateFragment = gateFragment;
			_indicatorFragment = indicatorFragment;
			_speakerFragment = speakerFragment;
			_depthSensorFragment = depthSensorFragment;
			_contaminationSensorFragment = contaminationSensorFragment;
			_flowSensorFragment = flowSensorFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddTopFragment(_leverFragment);
			builder.AddTopFragment(_relayFragment);
			builder.AddTopFragment(_memoryFragment);
			builder.AddTopFragment(_weatherStationFragment);
			builder.AddTopFragment(_chronometerFragment);
			builder.AddMiddleFragment(_scienceCounterFragment);
			builder.AddMiddleFragment(_resourceCounterFragment);
			builder.AddMiddleFragment(_populationCounterFragment);
			builder.AddMiddleFragment(_powerMeterFragment);
			builder.AddTopFragment(_timerFragment);
			builder.AddTopFragment(_gateFragment);
			builder.AddTopFragment(_indicatorFragment);
			builder.AddTopFragment(_speakerFragment);
			builder.AddTopFragment(_depthSensorFragment);
			builder.AddTopFragment(_contaminationSensorFragment);
			builder.AddTopFragment(_flowSensorFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<ResourceCounterGoodsDropdownProvider>().AsTransient();
		Bind<GateConflictStatus>().AsTransient();
		Bind<TimerIntervalElement>().AsTransient();
		Bind<DepthSensorMarker>().AsTransient();
		Bind<LeverFragment>().AsSingleton();
		Bind<PinnedLeversPanel>().AsSingleton();
		Bind<RelayFragment>().AsSingleton();
		Bind<RelayModeDescriptions>().AsSingleton();
		Bind<MemoryFragment>().AsSingleton();
		Bind<MemoryModeDescriptions>().AsSingleton();
		Bind<WeatherStationFragment>().AsSingleton();
		Bind<ChronometerFragment>().AsSingleton();
		Bind<ScienceCounterFragment>().AsSingleton();
		Bind<ResourceCounterFragment>().AsSingleton();
		Bind<PopulationCounterFragment>().AsSingleton();
		Bind<PowerMeterFragment>().AsSingleton();
		Bind<NumericComparisonModeDropdownFactory>().AsSingleton();
		Bind<TimerFragment>().AsSingleton();
		Bind<GateToggleFactory>().AsSingleton();
		Bind<GateFragment>().AsSingleton();
		Bind<TimerModeDescriptions>().AsSingleton();
		Bind<IndicatorFragment>().AsSingleton();
		Bind<PinnedIndicatorsPanel>().AsSingleton();
		Bind<SpeakerFragment>().AsSingleton();
		Bind<SpeakerSoundDropdownProvider>().AsSingleton();
		Bind<DepthSensorFragment>().AsSingleton();
		Bind<ContaminationSensorFragment>().AsSingleton();
		Bind<FlowSensorFragment>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<ResourceCounter, ResourceCounterGoodsDropdownProvider>();
		builder.AddDecorator<Gate, GateConflictStatus>();
		builder.AddDecorator<DepthSensor, DepthSensorMarker>();
		return builder.Build();
	}
}
