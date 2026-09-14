using System;
using Timberborn.InputSystemUI;
using UnityEngine.UIElements;

namespace Timberborn.TimeSpeedButtonSystem;

public class TimeSpeedButtonFactory
{
	private static readonly string TimeSpeedKeyFormat = "Speed{0}";

	private readonly BindableButtonFactory _bindableButtonFactory;

	public TimeSpeedButtonFactory(BindableButtonFactory bindableButtonFactory)
	{
		_bindableButtonFactory = bindableButtonFactory;
	}

	public TimeSpeedButton Create(Button button, int index, Action<int> clickCallback)
	{
		if (!int.TryParse(button.name.Replace("Speed", ""), out var timeSpeed))
		{
			throw new ArgumentException("Unable to parse speed value for " + button.name);
		}
		_bindableButtonFactory.CreateAndBind(button, string.Format(TimeSpeedKeyFormat, index), delegate
		{
			clickCallback(timeSpeed);
		});
		return new TimeSpeedButton(button, timeSpeed);
	}
}
