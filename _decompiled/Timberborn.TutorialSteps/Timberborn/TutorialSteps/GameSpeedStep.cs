using System;
using Timberborn.TimeSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class GameSpeedStep : ITutorialStep
{
	private readonly SpeedManager _speedManager;

	private readonly string _description;

	private readonly int _speed;

	private readonly bool _onlyOnce;

	private bool _wasAchieved;

	private bool _wasNotAchieved;

	public GameSpeedStep(SpeedManager speedManager, string description, int speed, bool onlyOnce)
	{
		_speedManager = speedManager;
		_description = description;
		_speed = speed;
		_onlyOnce = onlyOnce;
	}

	public string Description()
	{
		return _description;
	}

	public bool Achieved()
	{
		bool flag = (double)Math.Abs((float)_speed - _speedManager.CurrentSpeed) < 0.0001;
		_wasNotAchieved = _wasNotAchieved || !flag;
		_wasAchieved = flag || _wasAchieved;
		if (!_onlyOnce)
		{
			return _wasNotAchieved & flag;
		}
		return _wasAchieved;
	}
}
