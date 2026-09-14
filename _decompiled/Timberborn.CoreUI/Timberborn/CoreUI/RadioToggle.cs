using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

public class RadioToggle
{
	private static readonly string SelectedClass = "selected";

	private readonly ImmutableArray<VisualElement> _radioButtons;

	public event EventHandler<int> RadioButtonSelected;

	private RadioToggle(IEnumerable<VisualElement> radioButtons)
	{
		_radioButtons = radioButtons.ToImmutableArray();
	}

	public static RadioToggle Create(IEnumerable<VisualElement> radioButtons)
	{
		RadioToggle radioButtonToggle = new RadioToggle(radioButtons);
		for (int i = 0; i < radioButtonToggle._radioButtons.Length; i++)
		{
			int capturedIndex = i;
			radioButtonToggle._radioButtons[i].RegisterCallback<ClickEvent>(delegate
			{
				radioButtonToggle.RadioButtonSelected?.Invoke(radioButtonToggle, capturedIndex);
			});
		}
		return radioButtonToggle;
	}

	public void Update(int selectedIndex)
	{
		for (int i = 0; i < _radioButtons.Length; i++)
		{
			_radioButtons[i].EnableInClassList(SelectedClass, i == selectedIndex);
		}
	}
}
