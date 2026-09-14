using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Illumination;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.IlluminationUI;

internal class CustomizableIlluminatorFragment : IEntityPanelFragment
{
	private static readonly string SelectedPresetButtonClass = "customizable-illuminator-preset--selected";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly IlluminationService _illuminationService;

	private VisualElement _root;

	private TextField _rgbTextField;

	private CustomizableIlluminator _customizableIlluminator;

	private readonly List<(Color, Button)> _presetColorButtons = new List<(Color, Button)>();

	public CustomizableIlluminatorFragment(VisualElementLoader visualElementLoader, IlluminationService illuminationService)
	{
		_visualElementLoader = visualElementLoader;
		_illuminationService = illuminationService;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/CustomizableIlluminatorFragment");
		_root.ToggleDisplayStyle(visible: false);
		_rgbTextField = _root.Q<TextField>("Rgb");
		_rgbTextField.RegisterValueChangedCallback(OnRgbValueChanged);
		_rgbTextField.isDelayed = true;
		InitializePresets();
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_customizableIlluminator = entity.GetComponent<CustomizableIlluminator>();
		if ((bool)_customizableIlluminator)
		{
			UpdateCustomColor();
			_customizableIlluminator.CustomColorChanged += OnCustomColorChanged;
		}
	}

	public void ClearFragment()
	{
		if (_customizableIlluminator != null)
		{
			_customizableIlluminator.CustomColorChanged -= OnCustomColorChanged;
		}
		_customizableIlluminator = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if ((bool)_customizableIlluminator && _customizableIlluminator.IsCustomized && !_customizableIlluminator.IsLocked)
		{
			_root.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	private void InitializePresets()
	{
		VisualElement visualElement = _root.Q("Presets");
		foreach (Color presetColor in _illuminationService.PresetColors)
		{
			Button button = (Button)_visualElementLoader.LoadVisualElement("Game/EntityPanel/CustomizableIlluminatorPreset");
			button.Q<Image>("Image").style.unityBackgroundImageTintColor = presetColor;
			button.RegisterCallback<ClickEvent>(delegate
			{
				SetCustomColorToPreset(presetColor);
			});
			visualElement.Add(button);
			_presetColorButtons.Add((presetColor, button));
		}
	}

	private void OnRgbValueChanged(ChangeEvent<string> changeEvent)
	{
		if (ColorUtility.TryParseHtmlString("#" + changeEvent.newValue, out var color) || ColorUtility.TryParseHtmlString(changeEvent.newValue, out color))
		{
			_customizableIlluminator.SetCustomColor(color);
		}
		UpdateCustomColor();
	}

	private void OnCustomColorChanged(object sender, EventArgs e)
	{
		UpdateCustomColor();
	}

	private void SetCustomColorToPreset(Color presetColor)
	{
		_customizableIlluminator.SetCustomColor(presetColor);
	}

	private void UpdateCustomColor()
	{
		Color customColor = _customizableIlluminator.CustomColor;
		_rgbTextField.SetValueWithoutNotify(ColorUtility.ToHtmlStringRGB(customColor).ToLowerInvariant());
		foreach (var presetColorButton in _presetColorButtons)
		{
			var (a, _) = presetColorButton;
			presetColorButton.Item2.EnableInClassList(SelectedPresetButtonClass, ColorsAreTheSame(a, customColor));
		}
	}

	private static bool ColorsAreTheSame(Color a, Color b)
	{
		Color32 color = a;
		Color32 other = b;
		return color.Equals(other);
	}
}
