using Timberborn.FactionSystem;
using Timberborn.GameSceneLoading;
using Timberborn.MapStateSystem;
using Timberborn.Persistence;
using Timberborn.SceneLoading;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.GameFactionSystem;

public class FactionService : ISaveableSingleton, ILoadableSingleton
{
	private static readonly SingletonKey FactionServiceKey = new SingletonKey("FactionService");

	private static readonly PropertyKey<string> IdKey = new PropertyKey<string>("Id");

	private readonly ISingletonLoader _singletonLoader;

	private readonly FactionSpecService _factionSpecService;

	private readonly MapEditorMode _mapEditorMode;

	private readonly ISceneLoader _sceneLoader;

	private readonly FactionBlueprintModifierProvider _factionBlueprintModifierProvider;

	public FactionSpec Current { get; private set; }

	public FactionService(ISingletonLoader singletonLoader, FactionSpecService factionSpecService, MapEditorMode mapEditorMode, ISceneLoader sceneLoader, FactionBlueprintModifierProvider factionBlueprintModifierProvider)
	{
		_singletonLoader = singletonLoader;
		_factionSpecService = factionSpecService;
		_mapEditorMode = mapEditorMode;
		_sceneLoader = sceneLoader;
		_factionBlueprintModifierProvider = factionBlueprintModifierProvider;
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(FactionServiceKey, out var objectLoader))
		{
			SetCurrentFaction(objectLoader.Get(IdKey));
			return;
		}
		GameSceneParameters sceneParameters = _sceneLoader.GetSceneParameters<GameSceneParameters>();
		SetCurrentFaction(sceneParameters.NewGameConfiguration.FactionId);
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (!_mapEditorMode.IsMapEditor)
		{
			singletonSaver.GetSingleton(FactionServiceKey).Set(IdKey, Current.Id);
		}
	}

	private void SetCurrentFaction(string factionId)
	{
		Current = _factionSpecService.GetFaction(factionId);
		_factionBlueprintModifierProvider.Initialize(Current.BlueprintModifiers);
	}
}
