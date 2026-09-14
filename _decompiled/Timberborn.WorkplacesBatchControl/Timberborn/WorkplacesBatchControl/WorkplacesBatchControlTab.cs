using System.Collections.Generic;
using System.Linq;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using Timberborn.WorkSystem;

namespace Timberborn.WorkplacesBatchControl;

internal class WorkplacesBatchControlTab : BatchControlTab
{
	private readonly WorkplacesBatchControlRowFactory _workplacesBatchControlRowFactory;

	private readonly BatchControlRowGroupFactory _batchControlRowGroupFactory;

	public override string TabNameLocKey => "BatchControl.Workplaces";

	public override string TabImage => "Workplaces";

	public override string BindingKey => "WorkplacesTab";

	public WorkplacesBatchControlTab(VisualElementLoader visualElementLoader, BatchControlDistrict batchControlDistrict, WorkplacesBatchControlRowFactory workplacesBatchControlRowFactory, BatchControlRowGroupFactory batchControlRowGroupFactory, EventBus eventBus)
		: base(visualElementLoader, batchControlDistrict, eventBus)
	{
		_workplacesBatchControlRowFactory = workplacesBatchControlRowFactory;
		_batchControlRowGroupFactory = batchControlRowGroupFactory;
	}

	protected override IEnumerable<BatchControlRowGroup> GetRowGroups(IEnumerable<EntityComponent> entities)
	{
		IEnumerable<IGrouping<string, EntityComponent>> enumerable = from workplace in entities
			where workplace.GetComponent<Workplace>()
			where workplace
			group workplace by workplace.GetComponent<LabeledEntitySpec>().DisplayNameLocKey;
		foreach (IGrouping<string, EntityComponent> item in enumerable)
		{
			BatchControlRowGroup batchControlRowGroup = _batchControlRowGroupFactory.CreateSortedWithTextHeader(item.Key);
			foreach (EntityComponent item2 in item)
			{
				batchControlRowGroup.AddRow(_workplacesBatchControlRowFactory.Create(item2));
			}
			yield return batchControlRowGroup;
		}
	}
}
