using Timberborn.CoreSound;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using Timberborn.SoundSystem;
using UnityEngine;

namespace Timberborn.ModularShafts;

internal class ShaftSoundController : ILoadableSingleton
{
	private readonly ISoundSystem _soundSystem;

	private readonly RootObjectProvider _rootObjectProvider;

	private Transform _parent;

	public ShaftSoundController(ISoundSystem soundSystem, RootObjectProvider rootObjectProvider)
	{
		_soundSystem = soundSystem;
		_rootObjectProvider = rootObjectProvider;
	}

	public void Load()
	{
		_parent = _rootObjectProvider.CreateRootObject("ShaftSoundController").transform;
		Initialize();
	}

	public void AddEmitter(ShaftSoundEmitter emitter)
	{
		_soundSystem.AddAreaEmitter(_parent, emitter.GameObject);
	}

	public void RemoveEmitter(ShaftSoundEmitter emitter)
	{
		_soundSystem.RemoveAreaEmitter(_parent, emitter.GameObject);
	}

	private void Initialize()
	{
		string buildingMixerNameKey = MixerNames.BuildingMixerNameKey;
		_soundSystem.AddLimitedAreaSound(_parent, "Environment.Buildings.ShaftWorking", 60, 10, buildingMixerNameKey);
	}
}
