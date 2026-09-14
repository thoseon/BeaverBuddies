using System;
using UnityEngine.UIElements;

namespace Timberborn.SliderToggleSystem;

public class SliderToggleButton
{
	private static readonly string ActiveButtonClass = "slider-toggle__element--active";

	private static readonly string LockedButtonClass = "slider-toggle--locked";

	private readonly Button _button;

	private readonly Func<SliderToggleState> _stateGetter;

	private readonly Action _clickAction;

	public SliderToggleState CurrentState => _stateGetter();

	public SliderToggleButton(Button button, Func<SliderToggleState> stateGetter, Action clickAction)
	{
		_button = button;
		_stateGetter = stateGetter;
		_clickAction = clickAction;
	}

	public void Update()
	{
		switch (CurrentState)
		{
		case SliderToggleState.None:
			_button.RemoveFromClassList(ActiveButtonClass);
			_button.RemoveFromClassList(LockedButtonClass);
			break;
		case SliderToggleState.Active:
			_button.AddToClassList(ActiveButtonClass);
			_button.RemoveFromClassList(LockedButtonClass);
			break;
		case SliderToggleState.Locked:
			_button.AddToClassList(LockedButtonClass);
			break;
		case SliderToggleState.Unclickable:
			_button.SetEnabled(value: false);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public void Select()
	{
		_clickAction();
	}

	public void Clear()
	{
		_button.SetEnabled(value: true);
		_button.RemoveFromClassList(LockedButtonClass);
		_button.RemoveFromClassList(ActiveButtonClass);
	}
}
