using System;
using Timberborn.Localization;
using Timberborn.MechanicalSystem;
using Timberborn.UIFormatters;
using UnityEngine;

namespace Timberborn.MechanicalSystemUI;

internal class MechanicalNodeTextFormatter
{
	private readonly ILoc _loc;

	private readonly Phrase _powerOutputPhrase = Phrase.New("Mechanical.PowerOutput").FormatPower<int>();

	private readonly Phrase _powerInputPhrase = Phrase.New("Mechanical.PowerInputMaximum").Format((int value) => value.ToString()).FormatPower<int>()
		.Format((int value) => value.ToString());

	public MechanicalNodeTextFormatter(ILoc loc)
	{
		_loc = loc;
	}

	public string FormatGeneratorText(MechanicalNode mechanicalNode)
	{
		return _loc.T(_powerOutputPhrase, mechanicalNode.Actuals.PowerOutput);
	}

	public string FormatConsumerText(MechanicalNode mechanicalNode)
	{
		float num = (mechanicalNode.Active ? mechanicalNode.PowerEfficiency : 0f);
		int param = Mathf.RoundToInt(num * 100f);
		int val = Mathf.RoundToInt(num * (float)mechanicalNode.Actuals.PowerInput);
		int param2 = Math.Min(mechanicalNode.Actuals.PowerInput, val);
		int powerInput = mechanicalNode.Actuals.PowerInput;
		return _loc.T(_powerInputPhrase, param2, powerInput, param);
	}
}
