using Timberborn.CoreUI;
using Timberborn.MechanicalSystem;
using UnityEngine.UIElements;

namespace Timberborn.MechanicalSystemUI;

internal class GeneratorFragmentService
{
	private readonly MechanicalNodeTextFormatter _mechanicalNodeTextFormatter;

	private Label _label;

	public GeneratorFragmentService(MechanicalNodeTextFormatter mechanicalNodeTextFormatter)
	{
		_mechanicalNodeTextFormatter = mechanicalNodeTextFormatter;
	}

	public void Initialize(Label label)
	{
		_label = label;
	}

	public bool Update(MechanicalNode mechanicalNode)
	{
		if (mechanicalNode.IsGenerator)
		{
			_label.text = _mechanicalNodeTextFormatter.FormatGeneratorText(mechanicalNode);
		}
		_label.ToggleDisplayStyle(mechanicalNode.IsGenerator);
		return mechanicalNode.IsGenerator;
	}

	public void Hide()
	{
		_label.ToggleDisplayStyle(visible: false);
	}
}
