using Timberborn.BlueprintSystem;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using Timberborn.SoundSystem;
using UnityEngine;

namespace Timberborn.CoreSound;

public class WindAmbientSound : ILoadableSingleton
{
	private readonly ISoundSystem _soundSystem;

	private readonly RootObjectProvider _rootObjectProvider;

	private readonly ISpecService _specService;

	private GameObject _parent;

	public WindAmbientSound(ISoundSystem soundSystem, RootObjectProvider rootObjectProvider, ISpecService specService)
	{
		_soundSystem = soundSystem;
		_rootObjectProvider = rootObjectProvider;
		_specService = specService;
	}

	public void Load()
	{
		_parent = _rootObjectProvider.CreateRootObject("WindAmbientSound");
		string windAmbientKey = _specService.GetSingleSpec<CoreSoundSpec>().WindAmbientKey;
		_soundSystem.LoopSingle2DSound(_parent, windAmbientKey, 20);
		_soundSystem.SetCustomMixer(_parent, windAmbientKey, MixerNames.WindMixerNameKey);
	}
}
