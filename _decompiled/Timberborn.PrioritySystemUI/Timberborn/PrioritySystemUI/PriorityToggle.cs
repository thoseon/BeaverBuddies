using Timberborn.PrioritySystem;
using UnityEngine.UIElements;

namespace Timberborn.PrioritySystemUI;

public class PriorityToggle
{
	private static readonly string CheckedClass = "priority-toggle--checked";

	private readonly Priority _priority;

	private readonly Toggle _toggle;

	private IPrioritizable _prioritizable;

	public PriorityToggle(Priority priority, Toggle toggle)
	{
		_priority = priority;
		_toggle = toggle;
	}

	public void Initialize()
	{
		_toggle.RegisterValueChangedCallback(OnValueChanged);
	}

	public void UpdateState()
	{
		if (_prioritizable != null)
		{
			bool flag = _priority == _prioritizable.Priority;
			_toggle.SetValueWithoutNotify(flag);
			UpdateImage(flag);
		}
	}

	public void Enable(IPrioritizable prioritizable)
	{
		_prioritizable = prioritizable;
	}

	public void Disable()
	{
		_prioritizable = null;
	}

	private void OnValueChanged(ChangeEvent<bool> changeEvent)
	{
		if (changeEvent.newValue)
		{
			_prioritizable.SetPriority(_priority);
		}
		UpdateImage(changeEvent.newValue);
	}

	private void UpdateImage(bool state)
	{
		_toggle.EnableInClassList(CheckedClass, state);
	}
}
