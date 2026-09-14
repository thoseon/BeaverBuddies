using System;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.HazardousWeatherSystem;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using Timberborn.SoundSystem;
using Timberborn.WeatherSystem;
using UnityEngine;

namespace Timberborn.GameSound;

public class GameMusicPlayer : ILoadableSingleton
{
	private readonly ISoundSystem _soundSystem;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly WeatherService _weatherService;

	private readonly EventBus _eventBus;

	private readonly RootObjectProvider _rootObjectProvider;

	private readonly ISpecService _specService;

	private MusicSpec _musicSpec;

	private GameObject _parent;

	public GameMusicPlayer(ISoundSystem soundSystem, IRandomNumberGenerator randomNumberGenerator, WeatherService weatherService, EventBus eventBus, RootObjectProvider rootObjectProvider, ISpecService specService)
	{
		_soundSystem = soundSystem;
		_randomNumberGenerator = randomNumberGenerator;
		_weatherService = weatherService;
		_eventBus = eventBus;
		_rootObjectProvider = rootObjectProvider;
		_specService = specService;
	}

	public void Load()
	{
		_musicSpec = _specService.GetSingleSpec<MusicSpec>();
		_parent = _rootObjectProvider.CreateRootObject("GameMusicPlayer");
		if (_weatherService.IsHazardousWeather)
		{
			StartDroughtMusic();
		}
		else
		{
			StartStandardMusic();
		}
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnHazardousWeatherStarted(HazardousWeatherStartedEvent hazardousWeatherStartedEvent)
	{
		StopStandardMusic();
		StartDroughtMusic();
	}

	[OnEvent]
	public void OnHazardousWeatherEnded(HazardousWeatherEndedEvent hazardousWeatherEndedEvent)
	{
		StopDroughtMusic();
		StartStandardMusic();
	}

	private void StartStandardMusic()
	{
		PlaySound(_musicSpec.StandardTrack, PlayStandardPhrase, _musicSpec.MinDelay);
	}

	private void StopStandardMusic()
	{
		StopSound(_musicSpec.StandardTrack);
		StopSound(_musicSpec.StandardPhrase);
	}

	private void PlayStandardTrack()
	{
		PlaySound(_musicSpec.StandardTrack, PlayStandardPhrase);
	}

	private void PlayStandardPhrase()
	{
		PlaySound(_musicSpec.StandardPhrase, PlayStandardTrack);
	}

	private void StartDroughtMusic()
	{
		PlaySound(_musicSpec.DroughtTrack, PlayDroughtTrack, _musicSpec.MinDelay);
	}

	private void StopDroughtMusic()
	{
		StopSound(_musicSpec.DroughtTrack);
	}

	private void PlayDroughtTrack()
	{
		PlaySound(_musicSpec.DroughtTrack, PlayDroughtTrack);
	}

	private void PlaySound(string soundName, Action callback, float? delay = null)
	{
		_soundSystem.PlaySound2D(_parent, soundName, 0, delay ?? RandomDelay(), callback);
	}

	private void StopSound(string soundName)
	{
		_soundSystem.StopSound(_parent, soundName);
	}

	private float RandomDelay()
	{
		return _randomNumberGenerator.Range(_musicSpec.MinDelay, _musicSpec.MaxDelay);
	}
}
