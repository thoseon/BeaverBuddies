using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.GraphicsQualitySystem;
using Timberborn.PlatformUtilities;
using UnityEngine.UIElements;

namespace Timberborn.SettingsSystemUI;

internal class GraphicsSettingsController
{
	private readonly GraphicsQualitySettings _graphicsQualitySettings;

	private readonly AntiAliasingDropdownProvider _antiAliasingDropdownProvider;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly GraphicsQualityDropdownProvider _graphicsQualityDropdownProvider;

	private readonly LightQualityDropdownProvider _lightQualityDropdownProvider;

	private readonly ShadowQualityGraphicsDropdownProvider _shadowQualityGraphicsDropdownProvider;

	private readonly TextureQualityDropdownProvider _textureQualityDropdownProvider;

	private readonly AnisotropicFilteringDropdownProvider _anisotropicFilteringDropdownProvider;

	private readonly WaterQualityDropdownProvider _waterQualityDropdownProvider;

	private readonly BloomDropdownProvider _bloomDropdownProvider;

	private Dropdown _anisotropicFilteringDropdown;

	private Dropdown _antiAliasingDropdown;

	private Dropdown _graphicsQualityDropdown;

	private Dropdown _lightQualityDropdown;

	private Dropdown _shadowQualityDropdown;

	private Dropdown _textureQualityDropdown;

	private Dropdown _waterQualityDropdown;

	private Dropdown _bloomDropdown;

	private VisualElement _macMSAAWarning;

	public GraphicsSettingsController(GraphicsQualitySettings graphicsQualitySettings, AntiAliasingDropdownProvider antiAliasingDropdownProvider, DropdownItemsSetter dropdownItemsSetter, GraphicsQualityDropdownProvider graphicsQualityDropdownProvider, LightQualityDropdownProvider lightQualityDropdownProvider, ShadowQualityGraphicsDropdownProvider shadowQualityGraphicsDropdownProvider, TextureQualityDropdownProvider textureQualityDropdownProvider, AnisotropicFilteringDropdownProvider anisotropicFilteringDropdownProvider, WaterQualityDropdownProvider waterQualityDropdownProvider, BloomDropdownProvider bloomDropdownProvider)
	{
		_graphicsQualitySettings = graphicsQualitySettings;
		_antiAliasingDropdownProvider = antiAliasingDropdownProvider;
		_dropdownItemsSetter = dropdownItemsSetter;
		_graphicsQualityDropdownProvider = graphicsQualityDropdownProvider;
		_lightQualityDropdownProvider = lightQualityDropdownProvider;
		_shadowQualityGraphicsDropdownProvider = shadowQualityGraphicsDropdownProvider;
		_textureQualityDropdownProvider = textureQualityDropdownProvider;
		_anisotropicFilteringDropdownProvider = anisotropicFilteringDropdownProvider;
		_waterQualityDropdownProvider = waterQualityDropdownProvider;
		_bloomDropdownProvider = bloomDropdownProvider;
	}

	public void Initialize(VisualElement root)
	{
		_anisotropicFilteringDropdown = root.Q<Dropdown>("AnisotropicFiltering");
		_anisotropicFilteringDropdown.ValueChanged += delegate
		{
			UpdateSettings();
		};
		_antiAliasingDropdown = root.Q<Dropdown>("AntiAliasing");
		_antiAliasingDropdown.ValueChanged += delegate
		{
			UpdateSettings();
		};
		_graphicsQualityDropdown = root.Q<Dropdown>("GraphicsQuality");
		_graphicsQualityDropdown.ValueChanged += delegate
		{
			UpdateSettings();
		};
		_lightQualityDropdown = root.Q<Dropdown>("LightQuality");
		_lightQualityDropdown.ValueChanged += delegate
		{
			UpdateSettings();
		};
		_shadowQualityDropdown = root.Q<Dropdown>("ShadowQuality");
		_shadowQualityDropdown.ValueChanged += delegate
		{
			UpdateSettings();
		};
		_textureQualityDropdown = root.Q<Dropdown>("TextureQuality");
		_textureQualityDropdown.ValueChanged += delegate
		{
			UpdateSettings();
		};
		_waterQualityDropdown = root.Q<Dropdown>("WaterQuality");
		_waterQualityDropdown.ValueChanged += delegate
		{
			UpdateSettings();
		};
		_bloomDropdown = root.Q<Dropdown>("Bloom");
		_bloomDropdown.ValueChanged += delegate
		{
			UpdateSettings();
		};
		_macMSAAWarning = root.Q<VisualElement>("MacMSAAWarning");
		UpdateSettings();
	}

	private void UpdateSettings()
	{
		_dropdownItemsSetter.SetItems(_anisotropicFilteringDropdown, _anisotropicFilteringDropdownProvider);
		_dropdownItemsSetter.SetItems(_antiAliasingDropdown, _antiAliasingDropdownProvider);
		_dropdownItemsSetter.SetItems(_graphicsQualityDropdown, _graphicsQualityDropdownProvider);
		_dropdownItemsSetter.SetItems(_lightQualityDropdown, _lightQualityDropdownProvider);
		_dropdownItemsSetter.SetItems(_shadowQualityDropdown, _shadowQualityGraphicsDropdownProvider);
		_dropdownItemsSetter.SetItems(_textureQualityDropdown, _textureQualityDropdownProvider);
		_dropdownItemsSetter.SetItems(_waterQualityDropdown, _waterQualityDropdownProvider);
		_dropdownItemsSetter.SetItems(_bloomDropdown, _bloomDropdownProvider);
		UpdateMacMSAAWarning();
	}

	private void UpdateMacMSAAWarning()
	{
		AntialiasingType antiAliasingType = _graphicsQualitySettings.AntiAliasingType;
		bool flag = (uint)(antiAliasingType - 3) <= 2u;
		bool flag2 = flag;
		_macMSAAWarning.ToggleDisplayStyle(flag2 && ApplicationPlatform.IsMacOS());
	}
}
