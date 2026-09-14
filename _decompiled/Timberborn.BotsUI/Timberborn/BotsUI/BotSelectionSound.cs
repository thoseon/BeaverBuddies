using Timberborn.BaseComponentSystem;
using Timberborn.MortalSystem;
using Timberborn.SelectionSystem;
using Timberborn.SoundSystem;
using Timberborn.StatusSystem;

namespace Timberborn.BotsUI;

internal class BotSelectionSound : BaseComponent, IAwakableComponent, ISelectionListener
{
	private static readonly string ContentKey = "Content";

	private static readonly string DiscontentKey = "Discontent";

	private readonly ISoundSystem _soundSystem;

	private Mortal _mortal;

	private StatusSubject _statusSubject;

	private BotSelectionSoundSpec _botSelectionSoundSpec;

	public BotSelectionSound(ISoundSystem soundSystem)
	{
		_soundSystem = soundSystem;
	}

	public void Awake()
	{
		_mortal = GetComponent<Mortal>();
		_statusSubject = GetComponent<StatusSubject>();
		_botSelectionSoundSpec = GetComponent<BotSelectionSoundSpec>();
	}

	public void OnSelect()
	{
		PlaySound();
	}

	public void OnUnselect()
	{
	}

	private void PlaySound()
	{
		if (!_mortal.Dead)
		{
			string soundName = "UI.Bots.Selected." + _botSelectionSoundSpec.SoundNameKey + "_" + GetKey();
			_soundSystem.PlaySound2D(base.GameObject, soundName, 10);
		}
	}

	private string GetKey()
	{
		if (_statusSubject.ActiveStatuses.Count <= 0)
		{
			return ContentKey;
		}
		return DiscontentKey;
	}
}
