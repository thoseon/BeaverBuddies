using Timberborn.GameSceneLoading;
using Timberborn.MapStateSystem;
using Timberborn.Persistence;
using Timberborn.SceneLoading;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.RecoverableGoodSystem;

public class GoodRecoveryRateService : ILoadableSingleton, ISaveableSingleton
{
	private static readonly SingletonKey GoodRecoveryRateServiceKey = new SingletonKey("GoodRecoveryRateService");

	private static readonly PropertyKey<float> DemolishableRecoveryRateKey = new PropertyKey<float>("DemolishableRecoveryRate");

	private readonly MapEditorMode _mapEditorMode;

	private readonly ISingletonLoader _singletonLoader;

	private readonly ISceneLoader _sceneLoader;

	public float DemolishableRecoveryRate { get; private set; }

	public GoodRecoveryRateService(MapEditorMode mapEditorMode, ISingletonLoader singletonLoader, ISceneLoader sceneLoader)
	{
		_mapEditorMode = mapEditorMode;
		_singletonLoader = singletonLoader;
		_sceneLoader = sceneLoader;
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (!_mapEditorMode.IsMapEditor)
		{
			singletonSaver.GetSingleton(GoodRecoveryRateServiceKey).Set(DemolishableRecoveryRateKey, DemolishableRecoveryRate);
		}
	}

	public void Load()
	{
		IObjectLoader objectLoader;
		if (_sceneLoader.TryGetSceneParameters<GameSceneParameters>(out var sceneParameters) && sceneParameters.NewGame)
		{
			DemolishableRecoveryRate = sceneParameters.NewGameConfiguration.GameMode.DemolishableRecoveryRate;
		}
		else if (_singletonLoader.TryGetSingleton(GoodRecoveryRateServiceKey, out objectLoader))
		{
			DemolishableRecoveryRate = objectLoader.Get(DemolishableRecoveryRateKey);
		}
	}
}
