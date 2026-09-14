using Bindito.Core;
using Timberborn.AlertPanelSystem;
using Timberborn.EntityPanelSystem;

namespace Timberborn.WellbeingUI;

[Context("Game")]
internal class WellbeingUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly WellbeingFragment _wellbeingFragment;

		public EntityPanelModuleProvider(WellbeingFragment wellbeingFragment)
		{
			_wellbeingFragment = wellbeingFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddMiddleFragment(_wellbeingFragment);
			return builder.Build();
		}
	}

	private class AlertPanelModuleProvider : IProvider<AlertPanelModule>
	{
		private readonly WellbeingHighscoreAlertFragment _wellbeingHighscoreAlertFragment;

		public AlertPanelModuleProvider(WellbeingHighscoreAlertFragment wellbeingHighscoreAlertFragment)
		{
			_wellbeingHighscoreAlertFragment = wellbeingHighscoreAlertFragment;
		}

		public AlertPanelModule Get()
		{
			AlertPanelModule.Builder builder = new AlertPanelModule.Builder();
			builder.AddAlertFragment(_wellbeingHighscoreAlertFragment, 1);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<NeedViewFactory>().AsSingleton();
		Bind<WellbeingFragment>().AsSingleton();
		Bind<PopulationWellbeingBox>().AsSingleton();
		Bind<WellbeingServiceDistrictSwitcher>().AsSingleton();
		Bind<GoalRowFactory>().AsSingleton();
		Bind<BasicStatisticsPanel>().AsSingleton();
		Bind<WellbeingHighscoreAlertFragment>().AsSingleton();
		Bind<NeedGroupViewFactory>().AsSingleton();
		Bind<WellbeingBatchControlRowItemFactory>().AsSingleton();
		Bind<PopulationWellbeingCounterGroupFactory>().AsSingleton();
		Bind<NeedEffectDescriptionService>().AsSingleton();
		Bind<WellbeingSummaryFactory>().AsSingleton();
		Bind<WellbeingSummaryBonusFactory>().AsSingleton();
		Bind<WellbeingBonusTooltipFactory>().AsSingleton();
		Bind<WellbeingNameHelper>().AsSingleton();
		Bind<BasicStatisticsPanelFactory>().AsSingleton();
		Bind<PopulationWellbeingGoals>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
		MultiBind<AlertPanelModule>().ToProvider<AlertPanelModuleProvider>().AsSingleton();
	}
}
