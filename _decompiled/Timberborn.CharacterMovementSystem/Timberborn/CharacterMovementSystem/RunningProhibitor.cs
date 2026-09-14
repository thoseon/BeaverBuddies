using Timberborn.BaseComponentSystem;
using Timberborn.CharacterModelSystem;

namespace Timberborn.CharacterMovementSystem;

public class RunningProhibitor : BaseComponent, IAwakableComponent
{
	private static readonly string RunningProhibitedParameterName = "RunningProhibited";

	private CharacterAnimator _characterAnimator;

	private bool _runningProhibited;

	public bool RunningProhibited
	{
		get
		{
			return _runningProhibited;
		}
		set
		{
			if (_runningProhibited != value)
			{
				_runningProhibited = value;
				_characterAnimator.SetBool(RunningProhibitedParameterName, value);
			}
		}
	}

	public void Awake()
	{
		_characterAnimator = GetComponent<CharacterAnimator>();
	}
}
