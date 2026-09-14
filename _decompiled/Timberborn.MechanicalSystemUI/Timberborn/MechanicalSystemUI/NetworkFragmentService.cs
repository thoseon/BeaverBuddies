using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.MechanicalSystem;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.MechanicalSystemUI;

internal class NetworkFragmentService
{
	private readonly ILoc _loc;

	private Label _label;

	private readonly Phrase _powerDemandPhrase = Phrase.New("Mechanical.NetworkPower").Format((int value) => value.ToString()).FormatPower<int>();

	public NetworkFragmentService(ILoc loc)
	{
		_loc = loc;
	}

	public void Initialize(Label label)
	{
		_label = label;
	}

	public bool Update(MechanicalNode mechanicalNode)
	{
		MechanicalGraph graph = mechanicalNode.Graph;
		bool flag = graph != null && (graph.NumberOfGenerators > 0 || graph.Batteries.Count > 0);
		if (flag)
		{
			_label.text = _loc.T(_powerDemandPhrase, graph.PowerSupply, graph.PowerDemand);
		}
		_label.ToggleDisplayStyle(flag);
		return flag;
	}

	public void Hide()
	{
		_label.ToggleDisplayStyle(visible: false);
	}
}
