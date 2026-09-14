using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.Common;
using Timberborn.InputSystem;
using Timberborn.PrioritySystem;

namespace Timberborn.PrioritySystemUI;

public class PriorityToggleGroup : IInputProcessor
{
	private readonly InputService _inputService;

	private readonly ImmutableArray<PriorityToggle> _toggles;

	private readonly string _decreasePriorityKey;

	private readonly string _increasePriorityKey;

	private IPrioritizable _prioritizable;

	public PriorityToggleGroup(InputService inputService, IEnumerable<PriorityToggle> toggles, string decreasePriorityKey, string increasePriorityKey)
	{
		_inputService = inputService;
		_toggles = toggles.ToImmutableArray();
		_decreasePriorityKey = decreasePriorityKey;
		_increasePriorityKey = increasePriorityKey;
	}

	public void UpdateGroup()
	{
		for (int i = 0; i < _toggles.Length; i++)
		{
			_toggles[i].UpdateState();
		}
	}

	public void Enable(IPrioritizable prioritizable)
	{
		_prioritizable = prioritizable;
		for (int i = 0; i < _toggles.Length; i++)
		{
			_toggles[i].Enable(prioritizable);
		}
		_inputService.AddInputProcessor(this);
	}

	public void Disable()
	{
		_prioritizable = null;
		for (int i = 0; i < _toggles.Length; i++)
		{
			_toggles[i].Disable();
		}
		_inputService.RemoveInputProcessor(this);
	}

	public bool ProcessInput()
	{
		if (IsDefinedAndPressed(_decreasePriorityKey))
		{
			DecreasePriorityIfPossible();
			return true;
		}
		if (IsDefinedAndPressed(_increasePriorityKey))
		{
			IncreasePriorityIfPossible();
			return true;
		}
		return false;
	}

	private bool IsDefinedAndPressed(string key)
	{
		if (!string.IsNullOrEmpty(key))
		{
			return _inputService.IsKeyDown(key);
		}
		return false;
	}

	private void DecreasePriorityIfPossible()
	{
		Priority priority = _prioritizable.Priority.Previous();
		if (priority != _prioritizable.Priority)
		{
			_prioritizable.SetPriority(priority);
		}
	}

	private void IncreasePriorityIfPossible()
	{
		Priority priority = _prioritizable.Priority.Next();
		if (priority != _prioritizable.Priority)
		{
			_prioritizable.SetPriority(priority);
		}
	}
}
