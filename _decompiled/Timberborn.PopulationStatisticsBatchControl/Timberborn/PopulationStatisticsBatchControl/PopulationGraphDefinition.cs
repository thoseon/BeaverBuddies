using Timberborn.CoreUI;
using UnityEngine;

namespace Timberborn.PopulationStatisticsBatchControl;

internal class PopulationGraphDefinition
{
	private readonly string _displayName;

	public string Id { get; }

	public GraphDefinition GraphDefinition { get; }

	public PopulationGraphDefinition(string id, string displayName, Color color, bool isVisible = true)
	{
		Id = id;
		GraphDefinition = new GraphDefinition(color, isVisible);
		_displayName = displayName;
	}

	public string GetDisplayName()
	{
		string text = ColorUtility.ToHtmlStringRGB(GraphDefinition.Color);
		return "<b><color=#" + text + ">—</color></b> " + _displayName;
	}
}
