using Timberborn.BlueprintSystem;
using Timberborn.CoreSound;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using Timberborn.SoundSystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.GameSound;

public class WaterAmbientSound : ILoadableSingleton, IEmitterMap
{
	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private readonly ISoundSystem _soundSystem;

	private readonly RootObjectProvider _rootObjectProvider;

	private readonly ISpecService _specService;

	private AmbientSpec _ambientSpec;

	private Transform _parent;

	public WaterAmbientSound(IThreadSafeWaterMap threadSafeWaterMap, ISoundSystem soundSystem, RootObjectProvider rootObjectProvider, ISpecService specService)
	{
		_threadSafeWaterMap = threadSafeWaterMap;
		_soundSystem = soundSystem;
		_rootObjectProvider = rootObjectProvider;
		_specService = specService;
	}

	public void Load()
	{
		_ambientSpec = _specService.GetSingleSpec<AmbientSpec>();
		_parent = _rootObjectProvider.CreateRootObject("WaterAmbientSound").transform;
		AddAreaSound();
	}

	public bool IsEmitterAt(Vector2Int coordinates)
	{
		return _threadSafeWaterMap.IsWaterOnAnyHeight(coordinates);
	}

	private void AddAreaSound()
	{
		string ambientMixerNameKey = MixerNames.AmbientMixerNameKey;
		_soundSystem.AddLargeAreaSound(_parent, this, _ambientSpec.WaterAmbient, 50, 10, ambientMixerNameKey);
	}
}
