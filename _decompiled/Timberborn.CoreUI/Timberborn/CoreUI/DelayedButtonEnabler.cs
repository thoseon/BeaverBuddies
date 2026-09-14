using System.Collections.Generic;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

public class DelayedButtonEnabler : IUpdatableSingleton
{
	private readonly struct DelayedButton
	{
		public Button Button { get; }

		public float EnableTime { get; }

		public DelayedButton(Button button, float enableTime)
		{
			Button = button;
			EnableTime = enableTime;
		}
	}

	private static readonly int DelayInSeconds = 2;

	private readonly List<DelayedButton> _delayedButtons = new List<DelayedButton>();

	public void Add(Button button)
	{
		DelayedButton item = new DelayedButton(button, Time.unscaledTime + (float)DelayInSeconds);
		_delayedButtons.Add(item);
		button.SetEnabled(value: false);
	}

	public void UpdateSingleton()
	{
		for (int num = _delayedButtons.Count - 1; num >= 0; num--)
		{
			DelayedButton delayedButton = _delayedButtons[num];
			if (delayedButton.EnableTime - Time.unscaledTime <= 0f)
			{
				delayedButton.Button.SetEnabled(value: true);
				_delayedButtons.RemoveAt(num);
			}
		}
	}
}
