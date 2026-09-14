using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.Common;
using Timberborn.InputSystem;
using UnityEngine.UIElements;

namespace Timberborn.SliderToggleSystem;

public class SliderToggle : IInputProcessor
{
	private readonly InputService _inputService;

	private readonly string _toggleBindingKey;

	private readonly ImmutableList<SliderToggleButton> _sliderToggleButtons;

	public VisualElement Root { get; }

	public bool IsBound { get; private set; }

	public SliderToggle(InputService inputService, VisualElement root, string toggleBindingKey, IEnumerable<SliderToggleButton> sliderToggleButtons)
	{
		Root = root;
		_inputService = inputService;
		_toggleBindingKey = toggleBindingKey;
		_sliderToggleButtons = sliderToggleButtons.ToImmutableList();
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyDown(_toggleBindingKey))
		{
			SelectNext();
			return true;
		}
		return false;
	}

	public void Bind()
	{
		if (!string.IsNullOrWhiteSpace(_toggleBindingKey))
		{
			Asserts.IsFalse(this, IsBound, "IsBound");
			IsBound = true;
			_inputService.AddInputProcessor(this);
		}
	}

	public void Unbind()
	{
		if (IsBound)
		{
			IsBound = false;
			_inputService.RemoveInputProcessor(this);
		}
	}

	public void Update()
	{
		for (int i = 0; i < _sliderToggleButtons.Count; i++)
		{
			_sliderToggleButtons[i].Update();
		}
	}

	public void Clear()
	{
		for (int i = 0; i < _sliderToggleButtons.Count; i++)
		{
			_sliderToggleButtons[i].Clear();
		}
	}

	private void SelectNext()
	{
		_sliderToggleButtons[GetNextIndex()].Select();
	}

	private int GetNextIndex()
	{
		for (int i = 0; i < _sliderToggleButtons.Count; i++)
		{
			if (_sliderToggleButtons[i].CurrentState == SliderToggleState.Active)
			{
				return (i + 1) % _sliderToggleButtons.Count;
			}
		}
		return 0;
	}
}
