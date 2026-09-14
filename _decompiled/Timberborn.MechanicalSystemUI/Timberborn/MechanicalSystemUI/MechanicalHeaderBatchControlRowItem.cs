using Timberborn.BatchControl;
using Timberborn.Localization;
using Timberborn.MechanicalSystem;
using Timberborn.UIFormatters;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.MechanicalSystemUI;

internal class MechanicalHeaderBatchControlRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem
{
	private static readonly string EfficiencyLocKey = "Mechanical.Efficiency";

	private readonly ILoc _loc;

	private readonly Label _label;

	private readonly MechanicalGraph _mechanicalGraph;

	private readonly Phrase _networkPowerPhrase = Phrase.New("Mechanical.NetworkPower").Format((int value) => value.ToString()).FormatPower<int>();

	public VisualElement Root { get; }

	public MechanicalHeaderBatchControlRowItem(ILoc loc, VisualElement root, Label label, MechanicalGraph mechanicalGraph)
	{
		Root = root;
		_loc = loc;
		_label = label;
		_mechanicalGraph = mechanicalGraph;
	}

	public void UpdateRowItem()
	{
		string text = _loc.T(_networkPowerPhrase, _mechanicalGraph.PowerSupply, _mechanicalGraph.PowerDemand);
		int param = Mathf.RoundToInt(_mechanicalGraph.PowerEfficiency * 100f);
		string text2 = _loc.T(EfficiencyLocKey, param) ?? "";
		_label.text = text + " " + text2;
	}
}
