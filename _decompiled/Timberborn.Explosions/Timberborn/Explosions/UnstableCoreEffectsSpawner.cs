using Bindito.Unity;
using Timberborn.AssetSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using UnityEngine;

namespace Timberborn.Explosions;

internal class UnstableCoreEffectsSpawner : BaseComponent, IAwakableComponent
{
	private readonly IAssetLoader _assetLoader;

	private readonly IInstantiator _instantiator;

	private readonly ExplosionSoundPlayer _explosionSoundPlayer;

	private GameObject _explosionPrefab;

	public UnstableCoreEffectsSpawner(IAssetLoader assetLoader, IInstantiator instantiator, ExplosionSoundPlayer explosionSoundPlayer)
	{
		_assetLoader = assetLoader;
		_instantiator = instantiator;
		_explosionSoundPlayer = explosionSoundPlayer;
	}

	public void Awake()
	{
		UnstableCoreEffectsSpawnerSpec component = GetComponent<UnstableCoreEffectsSpawnerSpec>();
		_explosionPrefab = _assetLoader.Load<GameObject>(component.ExplosionPrefabPath);
	}

	public void SpawnEffects()
	{
		BlockObjectCenter component = GetComponent<BlockObjectCenter>();
		GameObject gameObject = _instantiator.Instantiate(_explosionPrefab, null);
		gameObject.transform.position = component.WorldCenterAtBaseZ;
		_explosionSoundPlayer.PlayGlobal(gameObject);
	}
}
