using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.Workshops;
using UnityEngine.UIElements;

namespace Timberborn.WorkshopsUI;

internal class ProductivityBatchControlRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem, IFinishableBatchControlRowItem
{
	private static readonly string VeryLowClass = "productivity-batch-control-row-item__icon--very-low";

	private static readonly string LowClass = "productivity-batch-control-row-item__icon--low";

	private static readonly string MediumClass = "productivity-batch-control-row-item__icon--medium";

	private static readonly string HighClass = "productivity-batch-control-row-item__icon--high";

	private static readonly string VeryHighClass = "productivity-batch-control-row-item__icon--very-high";

	private readonly Image _productivity;

	private readonly WorkshopProductivityCounter _workshopProductivityCounter;

	public VisualElement Root { get; }

	public ProductivityBatchControlRowItem(VisualElement root, Image productivity, WorkshopProductivityCounter workshopProductivityCounter)
	{
		Root = root;
		_productivity = productivity;
		_workshopProductivityCounter = workshopProductivityCounter;
	}

	public void UpdateRowItem()
	{
		DisableAllProductivityMarkers();
		SetProductivityMarker();
	}

	public void SetFinishedState(bool isFinished)
	{
		Root.ToggleDisplayStyle(isFinished);
	}

	private void DisableAllProductivityMarkers()
	{
		_productivity.RemoveFromClassList(VeryLowClass);
		_productivity.RemoveFromClassList(LowClass);
		_productivity.RemoveFromClassList(MediumClass);
		_productivity.RemoveFromClassList(HighClass);
		_productivity.RemoveFromClassList(VeryHighClass);
	}

	private void SetProductivityMarker()
	{
		float productivity = _workshopProductivityCounter.CalculateProductivity();
		_productivity.AddToClassList(GetImageClass(productivity));
	}

	private static string GetImageClass(float productivity)
	{
		if (!(productivity > 0.8f))
		{
			if (!(productivity > 0.6f))
			{
				if (!(productivity > 0.4f))
				{
					if (productivity > 0.2f)
					{
						return LowClass;
					}
					return VeryLowClass;
				}
				return MediumClass;
			}
			return HighClass;
		}
		return VeryHighClass;
	}
}
