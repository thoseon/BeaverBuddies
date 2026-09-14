using Bindito.Core;
using Timberborn.Beavers;
using Timberborn.BonusSystem;
using Timberborn.Characters;
using Timberborn.GameDistricts;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Wellbeing;

[Context("Game")]
internal class WellbeingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<WellbeingTierManager>().AsTransient();
		Bind<WellbeingTracker>().AsTransient();
		Bind<WellbeingTrackerRegistrar>().AsTransient();
		Bind<DistrictWellbeingTrackerRegistry>().AsTransient();
		Bind<WellbeingService>().AsSingleton();
		Bind<WellbeingHighscore>().AsSingleton();
		Bind<IWellbeingTierService>().To<WellbeingTierService>().AsSingleton();
		Bind<WellbeingLimitService>().AsSingleton();
		Bind<GlobalWellbeingTrackerRegistry>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BonusManager, WellbeingTierManager>();
		builder.AddDecorator<Character, WellbeingTracker>();
		builder.AddDecorator<DistrictCenter, DistrictWellbeingTrackerRegistry>();
		builder.AddDecorator<Beaver, WellbeingTrackerRegistrar>();
		return builder.Build();
	}
}
