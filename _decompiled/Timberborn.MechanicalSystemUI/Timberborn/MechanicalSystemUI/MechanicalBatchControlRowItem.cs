using Timberborn.BatchControl;
using Timberborn.MechanicalSystem;
using UnityEngine.UIElements;

namespace Timberborn.MechanicalSystemUI;

internal class MechanicalBatchControlRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem
{
	private readonly MechanicalNodeTextFormatter _mechanicalNodeTextFormatter;

	private readonly Label _label;

	private readonly MechanicalNode _mechanicalNode;

	public VisualElement Root { get; }

	public MechanicalBatchControlRowItem(MechanicalNodeTextFormatter mechanicalNodeTextFormatter, VisualElement root, Label label, MechanicalNode mechanicalNode)
	{
		Root = root;
		_mechanicalNodeTextFormatter = mechanicalNodeTextFormatter;
		_label = label;
		_mechanicalNode = mechanicalNode;
	}

	public void UpdateRowItem()
	{
		if (_mechanicalNode.IsGenerator)
		{
			_label.text = _mechanicalNodeTextFormatter.FormatGeneratorText(_mechanicalNode);
		}
		else if (_mechanicalNode.IsConsumer)
		{
			_label.text = _mechanicalNodeTextFormatter.FormatConsumerText(_mechanicalNode);
		}
	}
}
