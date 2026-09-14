using Timberborn.GameSceneLoading;
using Timberborn.NeedSpecs;
using Timberborn.NewGameConfigurationSystem;
using Timberborn.Persistence;
using Timberborn.SceneLoading;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.GameFactionSystem;

public class NeedModificationService : ILoadableSingleton, ISaveableSingleton
{
	private static readonly SingletonKey NeedModificationServiceKey = new SingletonKey("NeedModificationService");

	private static readonly PropertyKey<float> FoodConsumptionKey = new PropertyKey<float>("FoodConsumption");

	private static readonly PropertyKey<float> WaterConsumptionKey = new PropertyKey<float>("WaterConsumption");

	private static readonly string FoodNeedId = "Hunger";

	private static readonly string WaterNeedId = "Thirst";

	private static readonly string FoodGroupId = "Nutrition";

	private readonly ISceneLoader _sceneLoader;

	private readonly ISingletonLoader _singletonLoader;

	private float _foodConsumption;

	private float _waterConsumption;

	public NeedModificationService(ISceneLoader sceneLoader, ISingletonLoader singletonLoader)
	{
		_sceneLoader = sceneLoader;
		_singletonLoader = singletonLoader;
	}

	public void Load()
	{
		GameSceneParameters sceneParameters = _sceneLoader.GetSceneParameters<GameSceneParameters>();
		if (sceneParameters.NewGame)
		{
			GameModeSpec gameMode = sceneParameters.NewGameConfiguration.GameMode;
			_foodConsumption = gameMode.FoodConsumption;
			_waterConsumption = gameMode.WaterConsumption;
		}
		else
		{
			IObjectLoader singleton = _singletonLoader.GetSingleton(NeedModificationServiceKey);
			_foodConsumption = singleton.Get(FoodConsumptionKey);
			_waterConsumption = singleton.Get(WaterConsumptionKey);
		}
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		IObjectSaver singleton = singletonSaver.GetSingleton(NeedModificationServiceKey);
		singleton.Set(FoodConsumptionKey, _foodConsumption);
		singleton.Set(WaterConsumptionKey, _waterConsumption);
	}

	public NeedSpec ModifyIfEligible(NeedSpec needSpec)
	{
		if (needSpec.NeedGroupId == FoodGroupId)
		{
			return needSpec with
			{
				DailyDelta = _foodConsumption * needSpec.DailyDelta
			};
		}
		if (needSpec.Id == FoodNeedId)
		{
			return needSpec with
			{
				DailyDelta = _foodConsumption * needSpec.DailyDelta
			};
		}
		if (needSpec.Id == WaterNeedId)
		{
			return needSpec with
			{
				DailyDelta = _waterConsumption * needSpec.DailyDelta
			};
		}
		return needSpec;
	}
}
