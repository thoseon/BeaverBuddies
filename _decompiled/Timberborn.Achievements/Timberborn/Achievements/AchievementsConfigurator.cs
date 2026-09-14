using Bindito.Core;
using Timberborn.AchievementSystem;
using Timberborn.Beavers;
using Timberborn.Explosions;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Achievements;

[Context("Game")]
internal class AchievementsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<PlaceDynamiteAtBottomTracker>().AsTransient();
		Bind<InjuredJustBornBeaverTracker>().AsTransient();
		Bind<TreePlantingCounter>().AsSingleton();
		Bind<PlaceDynamiteAtBottomAchievement>().AsSingleton();
		Bind<InjuredJustBornBeaverAchievement>().AsSingleton();
		MultiBind<Achievement>().ToExisting<PlaceDynamiteAtBottomAchievement>();
		MultiBind<Achievement>().ToExisting<InjuredJustBornBeaverAchievement>();
		MultiBind<Achievement>().To<BadtideStreakAchievement>().AsSingleton();
		MultiBind<Achievement>().To<BuildCampfireAchievement>().AsSingleton();
		MultiBind<Achievement>().To<BuildDamAchievement>().AsSingleton();
		MultiBind<Achievement>().To<BatteryChargeStorageAchievement>().AsSingleton();
		MultiBind<Achievement>().To<ExplodeUnitWithDynamiteAchievement>().AsSingleton();
		MultiBind<Achievement>().To<Cycle5SurvivalAchievement>().AsSingleton();
		MultiBind<Achievement>().To<Cycle10SurvivalAchievement>().AsSingleton();
		MultiBind<Achievement>().To<Cycle20SurvivalAchievement>().AsSingleton();
		MultiBind<Achievement>().To<Cycle50SurvivalAchievement>().AsSingleton();
		MultiBind<Achievement>().To<ReachBuildHeightLimitAchievement>().AsSingleton();
		MultiBind<Achievement>().To<BuildWonderBeforeCycleAchievement>().AsSingleton();
		MultiBind<Achievement>().To<ExplodeDynamiteInSingleDayAchievement>().AsSingleton();
		MultiBind<Achievement>().To<BuildBotAchievement>().AsSingleton();
		MultiBind<Achievement>().To<BuildBotAfterBeaverExtinctionAchievement>().AsSingleton();
		MultiBind<Achievement>().To<WorkAllDayForWeekAchievement>().AsSingleton();
		MultiBind<Achievement>().To<SurviveBadtideAchievement>().AsSingleton();
		MultiBind<Achievement>().To<SurviveDroughtAchievement>().AsSingleton();
		MultiBind<Achievement>().To<BuildEveryStructureFolktailsAchievement>().AsSingleton();
		MultiBind<Achievement>().To<BuildEveryStructureIronTeethAchievement>().AsSingleton();
		MultiBind<Achievement>().To<UnlockIronTeethAchievement>().AsSingleton();
		MultiBind<Achievement>().To<BuildManyHedgesAchievement>().AsSingleton();
		MultiBind<Achievement>().To<ReachAverageWellbeing4Achievement>().AsSingleton();
		MultiBind<Achievement>().To<ReachAverageWellbeing10Achievement>().AsSingleton();
		MultiBind<Achievement>().To<ReachAverageWellbeing20Achievement>().AsSingleton();
		MultiBind<Achievement>().To<ReachAverageWellbeing30Achievement>().AsSingleton();
		MultiBind<Achievement>().To<ReachAverageWellbeing40Achievement>().AsSingleton();
		MultiBind<Achievement>().To<ReachAverageWellbeing50Achievement>().AsSingleton();
		MultiBind<Achievement>().To<ReachAverageWellbeing60Achievement>().AsSingleton();
		MultiBind<Achievement>().To<ReachMaxAverageWellbeingAchievement>().AsSingleton();
		MultiBind<Achievement>().To<ReachBeaverPopulation100Achievement>().AsSingleton();
		MultiBind<Achievement>().To<ReachBeaverPopulation250Achievement>().AsSingleton();
		MultiBind<Achievement>().To<ReachBeaverPopulation500Achievement>().AsSingleton();
		MultiBind<Achievement>().To<BuildStackedHydroponicGardensAchievement>().AsSingleton();
		MultiBind<Achievement>().To<Plant1000TreesAchievement>().AsSingleton();
		MultiBind<Achievement>().To<Plant5000TreesAchievement>().AsSingleton();
		MultiBind<Achievement>().To<Plant10000TreesAchievement>().AsSingleton();
		MultiBind<Achievement>().To<GeneratePowerWithWaterWheelsOnlyAchievement>().AsSingleton();
		MultiBind<Achievement>().To<GeneratePowerWithPowerWheelsOnlyAchievement>().AsSingleton();
		MultiBind<Achievement>().To<GeneratePowerWithWindTurbinesOnlyAchievement>().AsSingleton();
		MultiBind<Achievement>().To<ReachPopulationWithoutDwellingsAchievement>().AsSingleton();
		MultiBind<Achievement>().To<ActivateWonderFolktailsAchievement>().AsSingleton();
		MultiBind<Achievement>().To<ActivateWonderIronTeethAchievement>().AsSingleton();
		MultiBind<Achievement>().To<ActivateMultipleWondersAchievement>().AsSingleton();
		MultiBind<Achievement>().To<ReachMaxAverageWellbeingPopulatedAchievement>().AsSingleton();
		MultiBind<Achievement>().To<ZiplineNetworkLengthAchievement>().AsSingleton();
		MultiBind<Achievement>().To<CureContaminatedBeaverAchievement>().AsSingleton();
		MultiBind<Achievement>().To<LargeTubewayNetworkAchievement>().AsSingleton();
		MultiBind<Achievement>().To<BornBeaverAfterBeaverExtinctionAchievement>().AsSingleton();
		MultiBind<Achievement>().To<PlugAnyBadwaterSourceAchievement>().AsSingleton();
		MultiBind<Achievement>().To<PlugAllBadwaterSourcesAchievement>().AsSingleton();
		MultiBind<Achievement>().To<FloodBuildingAchievement>().AsSingleton();
		MultiBind<Achievement>().To<ProducePlanksInDayAchievement>().AsSingleton();
		MultiBind<Achievement>().To<BeaverStungByBeeAchievement>().AsSingleton();
		MultiBind<Achievement>().To<DemolishAndRebuildAchievement>().AsSingleton();
		MultiBind<Achievement>().To<WorkingRefineryForEachRecipeAchievement>().AsSingleton();
		MultiBind<Achievement>().To<BeaverDiesMiserableAchievement>().AsSingleton();
		MultiBind<Achievement>().To<MaplePastryOnlyAchievement>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Dynamite, PlaceDynamiteAtBottomTracker>();
		builder.AddDecorator<AdultSpec, InjuredJustBornBeaverTracker>();
		return builder.Build();
	}
}
