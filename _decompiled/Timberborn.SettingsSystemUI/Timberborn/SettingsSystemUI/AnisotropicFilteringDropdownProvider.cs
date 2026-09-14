using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.DropdownSystem;
using Timberborn.GraphicsQualitySystem;
using Timberborn.Localization;
using Timberborn.SingletonSystem;

namespace Timberborn.SettingsSystemUI;

internal class AnisotropicFilteringDropdownProvider : IDropdownProvider, ILoadableSingleton
{
	private static readonly string OffLocKey = "Settings.Graphics.Quality.Off";

	private static readonly string OnLocKey = "Settings.Graphics.Quality.On";

	private readonly ILoc _loc;

	private readonly GraphicsQualitySettings _graphicsQualitySettings;

	private ImmutableArray<string> _valuesFormatted;

	public IReadOnlyList<string> Items => _valuesFormatted;

	public AnisotropicFilteringDropdownProvider(ILoc loc, GraphicsQualitySettings graphicsQualitySettings)
	{
		_loc = loc;
		_graphicsQualitySettings = graphicsQualitySettings;
	}

	public string GetValue()
	{
		return _valuesFormatted[_graphicsQualitySettings.AnisotropicFilteringEnabled ? 1 : 0];
	}

	public void SetValue(string value)
	{
		_graphicsQualitySettings.ChangeToCustom();
		_graphicsQualitySettings.AnisotropicFilteringEnabled = value == _valuesFormatted[1];
	}

	public void Load()
	{
		_valuesFormatted = ((IEnumerable<string>)new string[2]
		{
			_loc.T(OffLocKey),
			_loc.T(OnLocKey)
		}).ToImmutableArray();
	}
}
