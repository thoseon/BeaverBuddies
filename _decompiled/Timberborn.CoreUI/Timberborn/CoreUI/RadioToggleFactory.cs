using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Localization;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

public class RadioToggleFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	internal RadioToggleFactory(VisualElementLoader visualElementLoader, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
	}

	public RadioToggle CreateLocalizable(IEnumerable<string> optionLocKeys, VisualElement parent)
	{
		return Create(optionLocKeys.Select((string optionLocKey) => _loc.T(optionLocKey)), parent);
	}

	public RadioToggle CreateLocalizable<T>(string optionLocKeyPrefix, VisualElement parent) where T : Enum
	{
		IEnumerable<string> optionLocKeys = Enum.GetValues(typeof(T)).Cast<T>().Select(delegate(T value)
		{
			string text = optionLocKeyPrefix;
			T val = value;
			return text + val;
		});
		return CreateLocalizable(optionLocKeys, parent);
	}

	private RadioToggle Create(IEnumerable<string> options, VisualElement parent)
	{
		List<VisualElement> list = new List<VisualElement>();
		foreach (string option in options)
		{
			VisualElement visualElement = CreateRadioButton(option);
			parent.Add(visualElement);
			list.Add(visualElement);
		}
		return RadioToggle.Create(list);
	}

	private VisualElement CreateRadioButton(string option)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Core/RadioButton");
		visualElement.Q<Label>("Text").text = option;
		return visualElement;
	}
}
