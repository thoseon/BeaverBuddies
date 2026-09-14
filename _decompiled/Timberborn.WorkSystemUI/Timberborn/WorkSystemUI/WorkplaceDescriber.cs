using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.UIFormatters;
using Timberborn.WorkSystem;
using Timberborn.WorkerTypesUI;
using UnityEngine.UIElements;

namespace Timberborn.WorkSystemUI;

public class WorkplaceDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private static readonly string WorkersClass = "described-amount--workers";

	private static readonly string WorkersLocKey = "Work.Workers";

	private static readonly string CurrentWorkersLocKey = "Work.CurrentWorkers";

	private static readonly string MaximumWorkersLocKey = "Work.MaximumWorkers";

	private readonly DescribedAmountFactory _describedAmountFactory;

	private readonly ILoc _loc;

	private readonly WorkerTypeHelper _workerTypeHelper;

	private Workplace _workplace;

	private WorkplaceSpec _workplaceSpec;

	public WorkplaceDescriber(DescribedAmountFactory describedAmountFactory, ILoc loc, WorkerTypeHelper workerTypeHelper)
	{
		_describedAmountFactory = describedAmountFactory;
		_loc = loc;
		_workerTypeHelper = workerTypeHelper;
	}

	public void Awake()
	{
		_workplace = GetComponent<Workplace>();
		_workplaceSpec = GetComponent<WorkplaceSpec>();
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		if (!base.GameObject.activeInHierarchy)
		{
			int maxWorkers = _workplace.MaxWorkers;
			string tooltip = _loc.T(WorkersLocKey, maxWorkers);
			VisualElement content = _describedAmountFactory.CreatePlain(WorkersClass, $"{maxWorkers}", tooltip);
			yield return EntityDescription.CreateMiddleSection(content, 2);
			if (_workplaceSpec.DisallowOtherWorkerTypes)
			{
				string disallowedWorkerText = _workerTypeHelper.GetDisallowedWorkerText(_workplaceSpec.DefaultWorkerType);
				string content2 = SpecialStrings.RowStarter + disallowedWorkerText;
				yield return EntityDescription.CreateTextSection(content2, 2020);
			}
		}
	}

	public string GetWorkersTooltip()
	{
		int numberOfAssignedWorkers = _workplace.NumberOfAssignedWorkers;
		int desiredWorkers = _workplace.DesiredWorkers;
		int maxWorkers = _workplace.MaxWorkers;
		string text = _loc.T(CurrentWorkersLocKey, numberOfAssignedWorkers, desiredWorkers);
		string text2 = _loc.T(MaximumWorkersLocKey, maxWorkers);
		return text + "\n" + text2;
	}
}
