using Timberborn.TimeSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class SetPauseStep : ITutorialStep
{
	private readonly SpeedManager _speedManager;

	private readonly string _description;

	private readonly bool _pause;

	private readonly bool _onlyOnce;

	private bool _wasAchieved;

	private bool _wasNotAchieved;

	public SetPauseStep(SpeedManager speedManager, string description, bool pause, bool onlyOnce)
	{
		_speedManager = speedManager;
		_description = description;
		_pause = pause;
		_onlyOnce = onlyOnce;
	}

	public string Description()
	{
		return _description;
	}

	public bool Achieved()
	{
		bool flag = (_pause ? (_speedManager.CurrentSpeed == 0f) : (_speedManager.CurrentSpeed > 0f));
		_wasNotAchieved = _wasNotAchieved || !flag;
		_wasAchieved = flag || _wasAchieved;
		if (!_onlyOnce)
		{
			return _wasNotAchieved & flag;
		}
		return _wasAchieved;
	}
}
