using Timberborn.GameSceneLoading;
using Timberborn.MapRepositorySystem;
using Timberborn.Persistence;
using Timberborn.SceneLoading;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.GameWonderCompletion;

public class MapNameService : ILoadableSingleton, ISaveableSingleton
{
	private static readonly SingletonKey MapNameServiceKey = new SingletonKey("MapNameService");

	private static readonly PropertyKey<string> NameKey = new PropertyKey<string>("Name");

	private static readonly PropertyKey<bool> IsResourceKey = new PropertyKey<bool>("IsResource");

	private readonly ISingletonLoader _singletonLoader;

	private readonly ISceneLoader _sceneLoader;

	public string Name { get; private set; }

	public bool IsResource { get; private set; }

	public bool HasMapName => Name != null;

	public MapNameService(ISingletonLoader singletonLoader, ISceneLoader sceneLoader)
	{
		_singletonLoader = singletonLoader;
		_sceneLoader = sceneLoader;
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		if (HasMapName)
		{
			IObjectSaver singleton = singletonSaver.GetSingleton(MapNameServiceKey);
			singleton.Set(NameKey, Name);
			singleton.Set(IsResourceKey, IsResource);
		}
	}

	public void Load()
	{
		if (_singletonLoader.TryGetSingleton(MapNameServiceKey, out var objectLoader))
		{
			Name = objectLoader.Get(NameKey);
			IsResource = objectLoader.Get(IsResourceKey);
			return;
		}
		GameSceneParameters sceneParameters = _sceneLoader.GetSceneParameters<GameSceneParameters>();
		if (sceneParameters.NewGameConfiguration != null)
		{
			MapFileReference mapFileReference = sceneParameters.NewGameConfiguration.MapFileReference;
			Name = mapFileReference.Name;
			IsResource = mapFileReference.Resource;
		}
	}
}
