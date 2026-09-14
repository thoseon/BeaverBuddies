using Timberborn.CoreUI;
using Timberborn.MechanicalSystem;
using UnityEngine.UIElements;

namespace Timberborn.MechanicalSystemUI;

internal class ConsumerFragmentService
{
	private readonly MechanicalNodeTextFormatter _mechanicalNodeTextFormatter;

	private Label _label;

	public ConsumerFragmentService(MechanicalNodeTextFormatter mechanicalNodeTextFormatter)
	{
		_mechanicalNodeTextFormatter = mechanicalNodeTextFormatter;
	}

	public void Initialize(Label label)
	{
		_label = label;
	}

	public bool Update(MechanicalNode mechanicalNode)
	{
		if (mechanicalNode.IsConsumer)
		{
			UpdateText(_label, mechanicalNode);
		}
		_label.ToggleDisplayStyle(mechanicalNode.IsConsumer);
		return mechanicalNode.IsConsumer;
	}

	public void Hide()
	{
		_label.ToggleDisplayStyle(visible: false);
	}

	private void UpdateText(Label label, MechanicalNode mechanicalNode)
	{
		label.text = _mechanicalNodeTextFormatter.FormatConsumerText(mechanicalNode);
	}
}
