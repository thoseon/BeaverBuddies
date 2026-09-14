using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.AutomationBuildings;
using Timberborn.Localization;
using Timberborn.SingletonSystem;

namespace Timberborn.AutomationBuildingsUI;

internal class RelayModeDescriptions : ILoadableSingleton
{
	private static readonly string DescriptionLocKeyPrefix = "Building.Relay.Mode.";

	private static readonly string DescriptionLocKeyPostfix = ".Description";

	private readonly ILoc _loc;

	private readonly Dictionary<RelayMode, string> _dictionary = new Dictionary<RelayMode, string>();

	public RelayModeDescriptions(ILoc loc)
	{
		_loc = loc;
	}

	public void Load()
	{
		foreach (RelayMode item in Enum.GetValues(typeof(RelayMode)).Cast<RelayMode>())
		{
			string key = $"{DescriptionLocKeyPrefix}{item}{DescriptionLocKeyPostfix}";
			_dictionary.Add(item, _loc.T(key));
		}
	}

	public string GetDescription(RelayMode relayMode)
	{
		return _dictionary[relayMode];
	}
}
