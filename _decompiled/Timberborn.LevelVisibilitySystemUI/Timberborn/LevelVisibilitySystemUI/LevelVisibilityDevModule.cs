using Timberborn.Debugging;
using Timberborn.LevelVisibilitySystem;
using Timberborn.QuickNotificationSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.LevelVisibilitySystemUI;

internal class LevelVisibilityDevModule : IDevModule, IUpdatableSingleton
{
	private enum AutoMoveState
	{
		Disabled,
		Up,
		Down
	}

	private static readonly float WarmupTimeInSeconds = 5f;

	private static readonly float ChangeIntervalInSeconds = 0.5f;

	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly QuickNotificationService _quickNotificationService;

	private AutoMoveState _state;

	private float _timeToNextChange;

	public LevelVisibilityDevModule(ILevelVisibilityService levelVisibilityService, QuickNotificationService quickNotificationService)
	{
		_levelVisibilityService = levelVisibilityService;
		_quickNotificationService = quickNotificationService;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Auto move levels up", delegate
		{
			ToggleState(AutoMoveState.Up);
		})).AddMethod(DevMethod.Create("Auto move levels down", delegate
		{
			ToggleState(AutoMoveState.Down);
		})).Build();
	}

	public void UpdateSingleton()
	{
		if (_state != AutoMoveState.Disabled)
		{
			Update();
		}
	}

	private void ToggleState(AutoMoveState state)
	{
		Reset();
		_state = state;
		string text = ((_state == AutoMoveState.Disabled) ? "Auto move levels disabled" : $"Auto move levels enabled\nWarmup time: {WarmupTimeInSeconds} seconds");
		_quickNotificationService.SendNotification(text);
	}

	private void Update()
	{
		if ((_state == AutoMoveState.Up && !_levelVisibilityService.LevelIsAtMax) || (_state == AutoMoveState.Down && !_levelVisibilityService.LevelIsAtMin))
		{
			UpdateLevels();
		}
		else
		{
			Reset();
		}
	}

	private void UpdateLevels()
	{
		if (_timeToNextChange < 0f)
		{
			int num = ((_state == AutoMoveState.Up) ? 1 : (-1));
			_levelVisibilityService.SetMaxVisibleLevel(_levelVisibilityService.MaxVisibleLevel + num);
			_timeToNextChange = ChangeIntervalInSeconds;
		}
		else
		{
			_timeToNextChange -= Time.unscaledDeltaTime;
		}
	}

	private void Reset()
	{
		_state = AutoMoveState.Disabled;
		_timeToNextChange = WarmupTimeInSeconds;
	}
}
