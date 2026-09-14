using Timberborn.SingletonSystem;
using Timberborn.SoundSystem;

namespace Timberborn.TimeSystem;

public class GameSpeedSoundController : ILoadableSingleton
{
	private static readonly string EnvironmentRootMixerVolumeKey = "EnvironmentRoot_Volume";

	private readonly ISoundSystem _soundSystem;

	private readonly EventBus _eventBus;

	public GameSpeedSoundController(ISoundSystem soundSystem, EventBus eventBus)
	{
		_soundSystem = soundSystem;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnCurrentSpeedChanged(CurrentSpeedChangedEvent currentSpeedChangedEvent)
	{
		UpdateSoundState(currentSpeedChangedEvent.CurrentSpeed);
	}

	private void UpdateSoundState(float currentSpeed)
	{
		if (currentSpeed == 0f)
		{
			MuteSound();
		}
		else
		{
			UnmuteSound();
		}
	}

	private void MuteSound()
	{
		_soundSystem.SetMixerVolume(EnvironmentRootMixerVolumeKey, 0f);
	}

	private void UnmuteSound()
	{
		_soundSystem.SetMixerVolume(EnvironmentRootMixerVolumeKey, 1f);
	}
}
