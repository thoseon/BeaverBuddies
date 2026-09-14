using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.DropdownSystem;
using Timberborn.GraphicsQualitySystem;
using Timberborn.Localization;
using Timberborn.SingletonSystem;

namespace Timberborn.SettingsSystemUI;

internal class ShadowQualityGraphicsDropdownProvider : IDropdownProvider, ILoadableSingleton
{
	private static readonly ImmutableArray<int> Values = ((IEnumerable<int>)new int[5] { 0, 1, 2, 3, 4 }).ToImmutableArray();

	private readonly ILoc _loc;

	private readonly GraphicsQualitySettings _graphicsQualitySettings;

	private ImmutableArray<string> _valuesFormatted;

	public IReadOnlyList<string> Items => _valuesFormatted;

	public ShadowQualityGraphicsDropdownProvider(ILoc loc, GraphicsQualitySettings graphicsQualitySettings)
	{
		_loc = loc;
		_graphicsQualitySettings = graphicsQualitySettings;
	}

	public string GetValue()
	{
		return GetFormattedValue(_graphicsQualitySettings.ShadowQuality);
	}

	public void SetValue(string value)
	{
		_graphicsQualitySettings.ChangeToCustom();
		_graphicsQualitySettings.ShadowQuality = Values[_valuesFormatted.IndexOf(value)];
	}

	public void Load()
	{
		_valuesFormatted = Values.Select(GetFormattedValue).ToImmutableArray();
	}

	private string GetFormattedValue(int value)
	{
		return value switch
		{
			0 => _loc.T("Settings.Graphics.Quality.Off"), 
			1 => _loc.T("Settings.Graphics.Quality.Low"), 
			2 => _loc.T("Settings.Graphics.Quality.Medium"), 
			3 => _loc.T("Settings.Graphics.Quality.High"), 
			4 => _loc.T("Settings.Graphics.Quality.Ultra"), 
			_ => value.ToString(), 
		};
	}
}
