using Timberborn.BatchControl;
using UnityEngine.UIElements;

namespace Timberborn.WorkSystemUI;

internal class WorkplaceWorkerTypeBatchControlRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem
{
	private readonly WorkerTypeToggle _workerTypeToggle;

	public VisualElement Root { get; }

	public WorkplaceWorkerTypeBatchControlRowItem(VisualElement root, WorkerTypeToggle workerTypeToggle)
	{
		Root = root;
		_workerTypeToggle = workerTypeToggle;
	}

	public void UpdateRowItem()
	{
		_workerTypeToggle.Update();
	}
}
