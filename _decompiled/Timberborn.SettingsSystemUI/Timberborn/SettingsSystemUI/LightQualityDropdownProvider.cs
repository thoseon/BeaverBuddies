using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.DropdownSystem;
using Timberborn.GraphicsQualitySystem;
using Timberborn.Localization;
using Timberborn.SingletonSystem;

namespace Timberborn.SettingsSystemUI;

internal class LightQualityDropdownProvider : IDropdownProvider, ILoadableSingleton
{
	private static readonly ImmutableArray<int> Values = ((IEnumerable<int>)new int[4] { 0, 4, 6, 8 }).ToImmutableArray();

	private readonly ILoc _loc;

	private readonly GraphicsQualitySettings _graphicsQualitySettings;

	private ImmutableArray<string> _valuesFormatted;

	public IReadOnlyList<string> Items => _valuesFormatted;

	public LightQualityDropdownProvider(ILoc loc, GraphicsQualitySettings graphicsQualitySettings)
	{
		_loc = loc;
		_graphicsQualitySettings = graphicsQualitySettings;
	}

	public string GetValue()
	{
		return GetFormatedValue(_graphicsQualitySettings.LightQuality);
	}

	public void SetValue(string value)
	{
		_graphicsQualitySettings.ChangeToCustom();
		_graphicsQualitySettings.LightQuality = Values[_valuesFormatted.IndexOf(value)];
	}

	public void Load()
	{
		_valuesFormatted = Values.Select(GetFormatedValue).ToImmutableArray();
	}

	private string GetFormatedValue(int value)
	{
		return value switch
		{
			0 => _loc.T("Settings.Graphics.Quality.Off"), 
			4 => _loc.T("Settings.Graphics.Quality.Low"), 
			6 => _loc.T("Settings.Graphics.Quality.Medium"), 
			8 => _loc.T("Settings.Graphics.Quality.High"), 
			_ => value.ToString(), 
		};
	}
}
