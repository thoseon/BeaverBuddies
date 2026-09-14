using Timberborn.CoreSound;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using Timberborn.SoundSystem;
using UnityEngine;

namespace Timberborn.SleepSystem;

public class SleepSoundController : ILoadableSingleton
{
	private readonly ISoundSystem _soundSystem;

	private readonly RootObjectProvider _rootObjectProvider;

	private Transform _parent;

	public SleepSoundController(ISoundSystem soundSystem, RootObjectProvider rootObjectProvider)
	{
		_soundSystem = soundSystem;
		_rootObjectProvider = rootObjectProvider;
	}

	public void Load()
	{
		_parent = _rootObjectProvider.CreateRootObject("SleepSoundController").transform;
		string ambientMixerNameKey = MixerNames.AmbientMixerNameKey;
		_soundSystem.AddLimitedAreaSound(_parent, "Environment.Beavers.Sleeping", 40, 4, ambientMixerNameKey);
	}

	public void AddSleepingBeaver(SleepSoundEmitter sleepSoundEmitter)
	{
		_soundSystem.AddAreaEmitter(_parent, sleepSoundEmitter.GameObject);
	}

	public void RemoveSleepingBeaver(SleepSoundEmitter sleepSoundEmitter)
	{
		_soundSystem.RemoveAreaEmitter(_parent, sleepSoundEmitter.GameObject);
	}
}
