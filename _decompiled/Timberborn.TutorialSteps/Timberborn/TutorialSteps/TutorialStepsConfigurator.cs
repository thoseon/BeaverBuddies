using Bindito.Core;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

[Context("Game")]
internal class TutorialStepsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BuiltBuildingService>().AsSingleton();
		Bind<PlantableResourceCounter>().AsSingleton();
		Bind<CameraMovementService>().AsSingleton();
		Bind<FirstbornService>().AsSingleton();
		Bind<MissingDamTrigger>().AsSingleton();
		Bind<StairsUnlockedTrigger>().AsSingleton();
		Bind<SurvivedFirstBadtideTrigger>().AsSingleton();
		Bind<SurvivedFirstDroughtTrigger>().AsSingleton();
		Bind<PlatformBuiltTrigger>().AsSingleton();
		Bind<VisibleLevelChangeService>().AsSingleton();
		Bind<UnemployedBeaversTrigger>().AsSingleton();
		MultiBind<IStepDeserializer>().To<BuildingTutorialStepDeserializer>().AsSingleton();
		MultiBind<IStepDeserializer>().To<ConnectBuildingsTutorialStepDeserializer>().AsSingleton();
		MultiBind<IStepDeserializer>().To<MarkTreesTutorialStepDeserializer>().AsSingleton();
		MultiBind<IStepDeserializer>().To<MarkPlantablesTutorialStepDeserializer>().AsSingleton();
		MultiBind<IStepDeserializer>().To<PowerBuildingsTutorialStepDeserializer>().AsSingleton();
		MultiBind<IStepDeserializer>().To<CameraMovementStepDeserializer>().AsSingleton();
		MultiBind<IStepDeserializer>().To<CameraRotationStepDeserializer>().AsSingleton();
		MultiBind<IStepDeserializer>().To<CameraZoomStepDeserializer>().AsSingleton();
		MultiBind<IStepDeserializer>().To<SetPauseStepDeserializer>().AsSingleton();
		MultiBind<IStepDeserializer>().To<GameSpeedStepDeserializer>().AsSingleton();
		MultiBind<IStepDeserializer>().To<AccumulateScienceForBuildingStepDeserializer>().AsSingleton();
		MultiBind<IStepDeserializer>().To<UnlockBuildingTutorialStepDeserializer>().AsSingleton();
		MultiBind<IStepDeserializer>().To<BeaverBirthStepDeserializer>().AsSingleton();
		MultiBind<IStepDeserializer>().To<SelectEntityStepDeserializer>().AsSingleton();
		MultiBind<IStepDeserializer>().To<OpenWellbeingPanelStepDeserializer>().AsSingleton();
		MultiBind<IStepDeserializer>().To<SelectStockpileGoodTutorialStepDeserializer>().AsSingleton();
		MultiBind<IStepDeserializer>().To<SetWorkingHoursStepDeserializer>().AsSingleton();
		MultiBind<IStepDeserializer>().To<VisibleLevelChangeStepDeserializer>().AsSingleton();
		MultiBind<IStepDeserializer>().To<ChangePausedStateStepDeserializer>().AsSingleton();
		MultiBind<IStepDeserializer>().To<DecreasePriorityStepDeserializer>().AsSingleton();
		MultiBind<IStepDeserializer>().To<IncreaseDesiredWorkersStepDeserializer>().AsSingleton();
	}
}
