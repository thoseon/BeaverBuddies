using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.DropdownSystem;
using Timberborn.GraphicsQualitySystem;
using Timberborn.Localization;
using Timberborn.SingletonSystem;

namespace Timberborn.SettingsSystemUI;

internal class WaterQualityDropdownProvider : IDropdownProvider, ILoadableSingleton
{
	private static readonly string LowQualityLocKey = "Settings.Graphics.Quality.Low";

	private static readonly string HighQualityLocKey = "Settings.Graphics.Quality.High";

	private static readonly ImmutableArray<int> Values = ((IEnumerable<int>)new int[2] { 0, 1 }).ToImmutableArray();

	private readonly ILoc _loc;

	private readonly GraphicsQualitySettings _graphicsQualitySettings;

	private ImmutableArray<string> _valuesFormatted;

	public IReadOnlyList<string> Items => _valuesFormatted;

	public WaterQualityDropdownProvider(ILoc loc, GraphicsQualitySettings graphicsQualitySettings)
	{
		_loc = loc;
		_graphicsQualitySettings = graphicsQualitySettings;
	}

	public string GetValue()
	{
		return GetFormatedValue(_graphicsQualitySettings.WaterQuality);
	}

	public void SetValue(string value)
	{
		_graphicsQualitySettings.ChangeToCustom();
		_graphicsQualitySettings.WaterQuality = Values[_valuesFormatted.IndexOf(value)];
	}

	public void Load()
	{
		_valuesFormatted = Values.Select(GetFormatedValue).ToImmutableArray();
	}

	private string GetFormatedValue(int value)
	{
		return value switch
		{
			0 => _loc.T(LowQualityLocKey), 
			1 => _loc.T(HighQualityLocKey), 
			_ => throw new ArgumentOutOfRangeException("value", value, null), 
		};
	}
}
