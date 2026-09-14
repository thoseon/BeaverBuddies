using Timberborn.BlueprintSystem;
using Timberborn.CoreSound;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using Timberborn.SoundSystem;
using Timberborn.TimeSystem;
using UnityEngine;

namespace Timberborn.GameSound;

internal class DayNightAmbientSound : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	private readonly ISoundSystem _soundSystem;

	private readonly IDayNightCycle _dayNightCycle;

	private readonly RootObjectProvider _rootObjectProvider;

	private readonly ISpecService _specService;

	private AmbientSpec _ambientSpec;

	private GameObject _parent;

	public DayNightAmbientSound(EventBus eventBus, ISoundSystem soundSystem, IDayNightCycle dayNightCycle, RootObjectProvider rootObjectProvider, ISpecService specService)
	{
		_eventBus = eventBus;
		_soundSystem = soundSystem;
		_dayNightCycle = dayNightCycle;
		_rootObjectProvider = rootObjectProvider;
		_specService = specService;
	}

	public void Load()
	{
		_ambientSpec = _specService.GetSingleSpec<AmbientSpec>();
		_parent = _rootObjectProvider.CreateRootObject("DayNightAmbientSound");
		_eventBus.Register(this);
		string sound = (_dayNightCycle.IsDaytime ? _ambientSpec.DayAmbient : _ambientSpec.NightAmbient);
		StartSound(sound);
	}

	[OnEvent]
	public void OnDaytimeStartEvent(DaytimeStartEvent daytimeStartEvent)
	{
		_soundSystem.StopSound(_parent, _ambientSpec.NightAmbient);
		StartSound(_ambientSpec.DayAmbient);
	}

	[OnEvent]
	public void OnNighttimeStartEvent(NighttimeStartEvent nighttimeStartEvent)
	{
		_soundSystem.StopSound(_parent, _ambientSpec.DayAmbient);
		StartSound(_ambientSpec.NightAmbient);
	}

	private void StartSound(string sound)
	{
		_soundSystem.LoopSingle2DSound(_parent, sound, 20);
		_soundSystem.SetCustomMixer(_parent, sound, MixerNames.AmbientMixerNameKey);
	}
}
