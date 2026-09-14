using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.DropdownSystem;
using Timberborn.GraphicsQualitySystem;
using Timberborn.Localization;
using Timberborn.SingletonSystem;

namespace Timberborn.SettingsSystemUI;

internal class GraphicsQualityDropdownProvider : IDropdownProvider, ILoadableSingleton
{
	private static readonly ImmutableArray<string> Values = ((IEnumerable<string>)new string[5] { "Low", "Medium", "High", "Ultra", "Custom" }).ToImmutableArray();

	private readonly ILoc _loc;

	private readonly GraphicsQualitySettings _graphicsQualitySettings;

	private ImmutableArray<string> _valuesFormatted;

	public IReadOnlyList<string> Items => _valuesFormatted;

	public GraphicsQualityDropdownProvider(ILoc loc, GraphicsQualitySettings graphicsQualitySettings)
	{
		_loc = loc;
		_graphicsQualitySettings = graphicsQualitySettings;
	}

	public string GetValue()
	{
		return GetFormattedValue(_graphicsQualitySettings.OverallGraphicsQuality);
	}

	public void SetValue(string value)
	{
		_graphicsQualitySettings.OverallGraphicsQuality = Values[_valuesFormatted.IndexOf(value)];
	}

	public void Load()
	{
		_valuesFormatted = Values.Select(GetFormattedValue).ToImmutableArray();
	}

	private string GetFormattedValue(string value)
	{
		return _loc.T("Settings.Graphics.Quality." + value);
	}
}
