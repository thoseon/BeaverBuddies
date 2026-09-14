using Timberborn.BatchControl;
using Timberborn.WorkSystem;
using UnityEngine.UIElements;

namespace Timberborn.WorkSystemUI;

internal class WorkplaceBatchControlRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem
{
	private readonly Workplace _workplace;

	private readonly Label _info;

	private readonly Button _increase;

	private readonly Button _decrease;

	public VisualElement Root { get; }

	public WorkplaceBatchControlRowItem(VisualElement root, Workplace workplace, Label info, Button increase, Button decrease)
	{
		Root = root;
		_workplace = workplace;
		_info = info;
		_increase = increase;
		_decrease = decrease;
	}

	public void UpdateRowItem()
	{
		int numberOfAssignedWorkers = _workplace.NumberOfAssignedWorkers;
		int desiredWorkers = _workplace.DesiredWorkers;
		int maxWorkers = _workplace.MaxWorkers;
		_info.text = $"{numberOfAssignedWorkers} / {desiredWorkers}";
		_decrease.SetEnabled(desiredWorkers > 1);
		_increase.SetEnabled(desiredWorkers < maxWorkers);
	}
}
