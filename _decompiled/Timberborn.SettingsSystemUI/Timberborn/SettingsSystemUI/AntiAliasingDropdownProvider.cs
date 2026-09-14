using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.DropdownSystem;
using Timberborn.GraphicsQualitySystem;
using Timberborn.Localization;
using Timberborn.SingletonSystem;

namespace Timberborn.SettingsSystemUI;

internal class AntiAliasingDropdownProvider : IDropdownProvider, ILoadableSingleton
{
	private static readonly ImmutableArray<AntialiasingType> Values = ImmutableArray.Create<AntialiasingType>(AntialiasingType.Off, AntialiasingType.FXAA, AntialiasingType.SMAA, AntialiasingType.MSAAx2, AntialiasingType.MSAAx4, AntialiasingType.MSAAx8);

	private readonly ILoc _loc;

	private readonly GraphicsQualitySettings _graphicsQualitySettings;

	private ImmutableArray<string> _valuesFormatted;

	public IReadOnlyList<string> Items => _valuesFormatted;

	public AntiAliasingDropdownProvider(ILoc loc, GraphicsQualitySettings graphicsQualitySettings)
	{
		_loc = loc;
		_graphicsQualitySettings = graphicsQualitySettings;
	}

	public string GetValue()
	{
		return GetFormattedValue(_graphicsQualitySettings.AntiAliasingType);
	}

	public void SetValue(string value)
	{
		_graphicsQualitySettings.ChangeToCustom();
		_graphicsQualitySettings.AntiAliasingType = Values[_valuesFormatted.IndexOf(value)];
	}

	public void Load()
	{
		_valuesFormatted = Values.Select(GetFormattedValue).ToImmutableArray();
	}

	private string GetFormattedValue(AntialiasingType antialiasingType)
	{
		switch (antialiasingType)
		{
		case AntialiasingType.Off:
			return _loc.T("Settings.Graphics.Quality.Off");
		case AntialiasingType.FXAA:
		case AntialiasingType.SMAA:
		case AntialiasingType.MSAAx2:
		case AntialiasingType.MSAAx4:
		case AntialiasingType.MSAAx8:
			return _loc.T("Settings.Graphics.AntiAliasing." + antialiasingType);
		default:
			throw new ArgumentOutOfRangeException("antialiasingType", antialiasingType, null);
		}
	}
}
