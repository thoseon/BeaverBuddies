using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.DropdownSystem;
using Timberborn.GraphicsQualitySystem;
using Timberborn.Localization;
using Timberborn.SingletonSystem;

namespace Timberborn.SettingsSystemUI;

internal class TextureQualityDropdownProvider : IDropdownProvider, ILoadableSingleton
{
	private static readonly ImmutableArray<int> Values = ImmutableArray.Create(2, 1, 0);

	private readonly ILoc _loc;

	private readonly GraphicsQualitySettings _graphicsQualitySettings;

	private ImmutableArray<string> _valuesFormatted;

	public IReadOnlyList<string> Items => _valuesFormatted;

	public TextureQualityDropdownProvider(ILoc loc, GraphicsQualitySettings graphicsQualitySettings)
	{
		_loc = loc;
		_graphicsQualitySettings = graphicsQualitySettings;
	}

	public string GetValue()
	{
		return GetFormattedValue(_graphicsQualitySettings.TextureQuality);
	}

	public void SetValue(string value)
	{
		_graphicsQualitySettings.ChangeToCustom();
		_graphicsQualitySettings.TextureQuality = Values[_valuesFormatted.IndexOf(value)];
	}

	public void Load()
	{
		_valuesFormatted = Values.Select(GetFormattedValue).ToImmutableArray();
	}

	private string GetFormattedValue(int value)
	{
		return value switch
		{
			0 => _loc.T("Settings.Graphics.Quality.High"), 
			1 => _loc.T("Settings.Graphics.Quality.Medium"), 
			2 => _loc.T("Settings.Graphics.Quality.Low"), 
			_ => value.ToString(), 
		};
	}
}
