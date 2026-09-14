using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.WorkSystem;

namespace Timberborn.WorkshopsEffects;

internal class WorkshopWorker : BaseComponent, IAwakableComponent
{
	private CharacterModel _characterModel;

	private BehaviorManager _behaviorManager;

	public void Awake()
	{
		_characterModel = GetComponent<CharacterModel>();
		_behaviorManager = GetComponent<BehaviorManager>();
	}

	public void UpdateVisibility()
	{
		if (_behaviorManager.IsRunningBehavior<WaitInsideIdlyWorkplaceBehavior>())
		{
			_characterModel.Show();
		}
		else
		{
			_characterModel.Hide();
		}
	}
}
