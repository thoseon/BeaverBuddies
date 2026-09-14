using Timberborn.FactionSystem;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using Timberborn.SoundSystem;
using UnityEngine;

namespace Timberborn.MainMenuPanels;

public class MainMenuSoundController : ILoadableSingleton
{
	private static readonly string MainMenuThemeSoundName = "Music_MainMenu.Theme";

	private static readonly string MainMenuCreditsSoundName = "Music_MainMenu.Credits";

	private readonly ISoundSystem _soundSystem;

	private readonly RootObjectProvider _rootObjectProvider;

	private GameObject _parent;

	public MainMenuSoundController(ISoundSystem soundSystem, RootObjectProvider rootObjectProvider)
	{
		_soundSystem = soundSystem;
		_rootObjectProvider = rootObjectProvider;
	}

	public void Load()
	{
		_parent = _rootObjectProvider.CreateRootObject("MainMenuSoundController");
	}

	public void PlayThemeMusic()
	{
		StopAllMusic();
		_soundSystem.PlaySound2D(_parent, MainMenuThemeSoundName, 0);
	}

	public void PlayCreditsMusic()
	{
		StopAllMusic();
		_soundSystem.PlaySound2D(_parent, MainMenuCreditsSoundName, 0);
	}

	public void PlayFactionSelectedSound(FactionSpec factionSpec)
	{
		string soundName = "UI.Beavers." + factionSpec.SoundId + ".Selected.Adult_Content";
		_soundSystem.PlaySound2D(_parent, soundName, 10);
	}

	private void StopAllMusic()
	{
		_soundSystem.StopSound(_parent, MainMenuThemeSoundName);
		_soundSystem.StopSound(_parent, MainMenuCreditsSoundName);
	}
}
