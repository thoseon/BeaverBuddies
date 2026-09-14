using System;
using Timberborn.InputSystem;
using UnityEngine;

namespace Timberborn.LevelVisibilitySystemUI;

internal class LevelVisibilitySelector : IInputProcessor
{
	private static readonly float SelectionSpeed = 0.1f;

	private readonly InputService _inputService;

	private Action<int> _changeCallback;

	private Action _endCallback;

	private float _accumulatedChange;

	private bool _midSelection;

	public LevelVisibilitySelector(InputService inputService)
	{
		_inputService = inputService;
	}

	public void StartLevelSelection(Action<int> changeCallback, Action endCallback)
	{
		if (!_midSelection)
		{
			_changeCallback = changeCallback;
			_endCallback = endCallback;
			_accumulatedChange = 0f;
			_midSelection = true;
			_inputService.AddInputProcessor(this);
		}
	}

	public bool ProcessInput()
	{
		if (_inputService.MainMouseButtonUp)
		{
			EndLevelSelection();
		}
		else
		{
			int num = ProcessMouseMovement();
			if (num != 0)
			{
				_changeCallback(num);
			}
		}
		return true;
	}

	private void EndLevelSelection()
	{
		_inputService.RemoveInputProcessor(this);
		_midSelection = false;
		_endCallback();
	}

	private int ProcessMouseMovement()
	{
		_accumulatedChange += (_inputService.MouseXYAxes * SelectionSpeed).y;
		int num = Mathf.RoundToInt(_accumulatedChange);
		if (num != 0)
		{
			_accumulatedChange -= num;
			return num;
		}
		return 0;
	}
}
