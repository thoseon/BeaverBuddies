using Timberborn.BlueprintSystem;
using Timberborn.GameFactionSystem;
using Timberborn.GameWonderCompletion;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using Timberborn.SoundSystem;
using UnityEngine;

namespace Timberborn.GameSound;

public class GameUISoundController : ILoadableSingleton
{
	private readonly ISoundSystem _soundSystem;

	private readonly FactionService _factionService;

	private readonly RootObjectProvider _rootObjectProvider;

	private readonly ISpecService _specService;

	private GameObject _parent;

	private GameUISoundSpec _spec;

	public GameUISoundController(ISoundSystem soundSystem, FactionService factionService, RootObjectProvider rootObjectProvider, ISpecService specService)
	{
		_soundSystem = soundSystem;
		_factionService = factionService;
		_rootObjectProvider = rootObjectProvider;
		_specService = specService;
	}

	public void Load()
	{
		_parent = _rootObjectProvider.CreateRootObject("GameUISoundController");
		_spec = _specService.GetSingleSpec<GameUISoundSpec>();
	}

	public void PlayWellbeingHighscoreSound()
	{
		PlaySound2D(_spec.WellbeingHighscore);
	}

	public void PlayFieldPlacedSound()
	{
		PlaySound2D(_spec.FieldPlaced);
	}

	public void PlayBlinkingSound()
	{
		PlaySound2D(_spec.BlinkingSoundKey);
	}

	public void PlayBadtideStartedSound()
	{
		PlaySound2D(_spec.BadtideStartedSoundKey);
	}

	public void PlayDroughtStartedSound()
	{
		PlaySound2D(_spec.DroughtStartedSoundKey);
	}

	public void PlayTemperateWeatherStartedSound()
	{
		PlaySound2D(_spec.TemperateWeatherStartedSoundKey);
	}

	public void PlayWonderLaunchSound()
	{
		PlaySound2D(_factionService.Current.GetSpec<FactionWonderSpec>().WonderLaunchSound);
	}

	public void PlayWonderCongratulationSound()
	{
		PlaySound2D(_spec.WonderCongratulationSoundKey);
	}

	public void PlaySound2D(string sound)
	{
		_soundSystem.PlaySound2D(_parent, sound, 10);
	}
}
