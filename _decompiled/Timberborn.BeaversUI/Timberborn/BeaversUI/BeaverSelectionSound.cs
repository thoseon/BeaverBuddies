using Timberborn.BaseComponentSystem;
using Timberborn.Beavers;
using Timberborn.BehaviorSystem;
using Timberborn.GameFactionSystem;
using Timberborn.MortalSystem;
using Timberborn.NeedBehaviorSystem;
using Timberborn.SelectionSystem;
using Timberborn.SleepSystem;
using Timberborn.SoundSystem;
using Timberborn.StatusSystem;

namespace Timberborn.BeaversUI;

public class BeaverSelectionSound : BaseComponent, IAwakableComponent, ISelectionListener
{
	private static readonly string ChildKey = "Child_";

	private static readonly string AdultKey = "Adult_";

	private static readonly string SleepingKey = "Sleeping";

	private static readonly string SleepyKey = "Sleepy";

	private static readonly string ContentKey = "Content";

	private static readonly string DiscontentKey = "Discontent";

	private readonly ISoundSystem _soundSystem;

	private readonly FactionService _factionService;

	private Child _child;

	private Mortal _mortal;

	private BehaviorManager _behaviorManager;

	private StatusSubject _statusSubject;

	public BeaverSelectionSound(ISoundSystem soundSystem, FactionService factionService)
	{
		_soundSystem = soundSystem;
		_factionService = factionService;
	}

	public void Awake()
	{
		_child = GetComponent<Child>();
		_mortal = GetComponent<Mortal>();
		_behaviorManager = GetComponent<BehaviorManager>();
		_statusSubject = GetComponent<StatusSubject>();
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
			string soundId = _factionService.Current.SoundId;
			string text = (((BaseComponent)(object)_child) ? ChildKey : AdultKey);
			string stateKey = GetStateKey();
			string soundName = "UI.Beavers." + soundId + ".Selected." + text + stateKey;
			_soundSystem.PlaySound2D(base.GameObject, soundName, 10);
		}
	}

	private string GetStateKey()
	{
		if (_behaviorManager.IsRunningBehavior<SleepNeedBehavior>())
		{
			if (!_behaviorManager.IsRunningExecutor<ApplyEffectExecutor>())
			{
				return SleepyKey;
			}
			return SleepingKey;
		}
		if (_statusSubject.ActiveStatuses.Count > 0)
		{
			return DiscontentKey;
		}
		return ContentKey;
	}
}
