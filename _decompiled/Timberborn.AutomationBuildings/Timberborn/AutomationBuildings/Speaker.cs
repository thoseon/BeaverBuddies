using System;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.CoreSound;
using Timberborn.DuplicationSystem;
using Timberborn.EntitySystem;
using Timberborn.Illumination;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.SoundSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.AutomationBuildings;

public class Speaker : BaseComponent, IAwakableComponent, IInitializableEntity, IPersistentEntity, IDuplicable<Speaker>, IDuplicable, IAutomatableNeeder, IFinishedStateListener, ITerminal, IRegisteredComponent
{
	private static readonly int Priority = 5;

	private static readonly ComponentKey ComponentKey = new ComponentKey("Speaker");

	private static readonly PropertyKey<SpeakerPlaybackMode> PlaybackModeKey = new PropertyKey<SpeakerPlaybackMode>("PlaybackMode");

	private static readonly PropertyKey<SpeakerSpatialMode> SpatialModeKey = new PropertyKey<SpeakerSpatialMode>("SpatialMode");

	private static readonly PropertyKey<string> SoundIdKey = new PropertyKey<string>("SoundId");

	private readonly SpeakerSoundService _speakerSoundService;

	private readonly SpeakerPlayer _speakerPlayer;

	private readonly ISoundSystem _soundSystem;

	private readonly EventBus _eventBus;

	private Automatable _automatable;

	private IlluminatorToggle _illuminatorToggle;

	private bool? _previousState;

	private string _playedSoundId;

	private bool _playRequested;

	public SpeakerPlaybackMode PlaybackMode { get; private set; }

	public SpeakerSpatialMode SpatialMode { get; private set; }

	public string SoundId { get; private set; }

	public bool NeedsAutomatable => true;

	internal bool IsPlaying => IsSoundIdValid(_playedSoundId);

	public event EventHandler PlaybackStateChanged;

	internal Speaker(SpeakerSoundService speakerSoundService, SpeakerPlayer speakerPlayer, ISoundSystem soundSystem, EventBus eventBus)
	{
		_speakerSoundService = speakerSoundService;
		_speakerPlayer = speakerPlayer;
		_soundSystem = soundSystem;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_automatable = GetComponent<Automatable>();
		_illuminatorToggle = GetComponent<Illuminator>().CreateToggle();
	}

	public void InitializeEntity()
	{
		ValidateSoundId();
		_eventBus.Register(this);
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(ComponentKey);
		component.Set(PlaybackModeKey, PlaybackMode);
		component.Set(SpatialModeKey, SpatialMode);
		component.Set(SoundIdKey, SoundId);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(ComponentKey);
		PlaybackMode = component.Get(PlaybackModeKey);
		SpatialMode = component.Get(SpatialModeKey);
		SoundId = component.Get(SoundIdKey);
	}

	public void OnEnterFinishedState()
	{
		_speakerPlayer.AddSpeaker(this);
		PlayIfContinuous();
	}

	public void OnExitFinishedState()
	{
		_speakerPlayer.RemoveSpeaker(this);
	}

	public void Evaluate()
	{
		bool flag = _automatable.State == ConnectionState.On;
		if (_previousState != flag)
		{
			EvaluatePlayback(flag);
			_illuminatorToggle.Toggle(flag);
			_previousState = flag;
		}
	}

	public void DuplicateFrom(Speaker source)
	{
		SoundId = source.SoundId;
		PlaybackMode = source.PlaybackMode;
		SpatialMode = source.SpatialMode;
		StopAndPlayIfContinuous();
	}

	public void SetPlaybackMode(SpeakerPlaybackMode playbackMode)
	{
		PlaybackMode = playbackMode;
		StopAndPlayIfContinuous();
	}

	public void SetSpatialMode(SpeakerSpatialMode spatialMode)
	{
		SpatialMode = spatialMode;
		StopAndPlayIfContinuous();
	}

	public void SetSoundId(string soundId)
	{
		SoundId = _speakerSoundService.GetValidatedSoundId(soundId);
		StopAndPlayIfContinuous();
	}

	[OnEvent]
	public void OnSpeakerSoundsReloaded(SpeakerSoundsReloadedEvent speakerSoundsReloadedEvent)
	{
		Stop();
		_soundSystem.InvalidateSounds(base.GameObject);
		ValidateSoundId();
		PlayIfContinuous();
	}

	internal void PlayIfRequested()
	{
		if (_playRequested)
		{
			UpdateMixer(SoundId);
			if (PlaybackMode == SpeakerPlaybackMode.Once)
			{
				PlayOnce(SoundId);
			}
			else
			{
				PlayLooped(SoundId);
			}
			_playedSoundId = SoundId;
			_playRequested = false;
			PlaybackStateChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private void ValidateSoundId()
	{
		SoundId = _speakerSoundService.GetValidatedSoundId(SoundId);
	}

	private void EvaluatePlayback(bool isOn)
	{
		if (isOn && ((PlaybackMode == SpeakerPlaybackMode.Once && _previousState == false) || PlaybackMode == SpeakerPlaybackMode.Continuously))
		{
			Play();
		}
		else if (!isOn && PlaybackMode == SpeakerPlaybackMode.Continuously)
		{
			Stop();
		}
	}

	private void StopAndPlayIfContinuous()
	{
		Stop();
		PlayIfContinuous();
	}

	private void PlayIfContinuous()
	{
		if (_automatable.State == ConnectionState.On && PlaybackMode == SpeakerPlaybackMode.Continuously)
		{
			Play();
		}
	}

	private void Stop()
	{
		if (IsSoundIdValid(_playedSoundId))
		{
			_soundSystem.StopSound(base.GameObject, _playedSoundId);
			_playedSoundId = string.Empty;
			PlaybackStateChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	private void Play()
	{
		if (IsSoundIdValid(SoundId))
		{
			if (!string.IsNullOrWhiteSpace(_playedSoundId))
			{
				Stop();
			}
			_playRequested = true;
		}
	}

	private void UpdateMixer(string soundIdToPlay)
	{
		_soundSystem.SetCustomMixer(base.GameObject, soundIdToPlay, (SpatialMode == SpeakerSpatialMode.Spatial) ? MixerNames.EnvironmentMixerNameKey : MixerNames.UIMixerNameKey);
	}

	private void PlayOnce(string soundId)
	{
		if (SpatialMode == SpeakerSpatialMode.NonSpatial)
		{
			_soundSystem.PlaySound2D(base.GameObject, soundId, Priority, 0f, OnPlaybackFinished);
		}
		else
		{
			_soundSystem.PlaySound3D(base.GameObject, soundId, Priority, OnPlaybackFinished);
		}
	}

	private void PlayLooped(string soundId)
	{
		if (SpatialMode == SpeakerSpatialMode.NonSpatial)
		{
			_soundSystem.LoopSingle2DSound(base.GameObject, soundId, Priority);
		}
		else
		{
			_soundSystem.LoopSingle3DSound(base.GameObject, soundId, Priority);
		}
	}

	private void OnPlaybackFinished()
	{
		_playedSoundId = string.Empty;
		PlaybackStateChanged?.Invoke(this, EventArgs.Empty);
	}

	private static bool IsSoundIdValid(string id)
	{
		return !string.IsNullOrWhiteSpace(id);
	}
}
