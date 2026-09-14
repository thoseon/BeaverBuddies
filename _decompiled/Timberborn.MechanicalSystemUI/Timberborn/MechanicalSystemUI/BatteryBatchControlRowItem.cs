using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.MechanicalSystem;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.MechanicalSystemUI;

internal class BatteryBatchControlRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem
{
	private readonly ILoc _loc;

	private readonly Timberborn.CoreUI.ProgressBar _progressBar;

	private readonly Label _chargeLabel;

	private readonly MechanicalNode _mechanicalNode;

	private readonly Phrase _chargePhrase = Phrase.New("Mechanical.BatteryCharge").Format((int value) => value.ToString()).FormatPowerCapacity<int>();

	public VisualElement Root { get; }

	public BatteryBatchControlRowItem(ILoc loc, VisualElement root, Timberborn.CoreUI.ProgressBar progressBar, Label chargeLabel, MechanicalNode mechanicalNode)
	{
		_loc = loc;
		Root = root;
		_progressBar = progressBar;
		_chargeLabel = chargeLabel;
		_mechanicalNode = mechanicalNode;
	}

	public void UpdateRowItem()
	{
		_chargeLabel.text = _loc.T(_chargePhrase, _mechanicalNode.NominalBatteryCharge, _mechanicalNode.NominalBatteryCapacity);
		_progressBar.SetProgress(_mechanicalNode.NominalBatteryChargeLevel);
	}
}
