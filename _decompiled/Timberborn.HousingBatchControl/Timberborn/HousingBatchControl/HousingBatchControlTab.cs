using System.Collections.Generic;
using System.Linq;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.DwellingSystem;
using Timberborn.EntitySystem;
using Timberborn.Reproduction;
using Timberborn.SingletonSystem;

namespace Timberborn.HousingBatchControl;

internal class HousingBatchControlTab : BatchControlTab
{
	private readonly HousingBatchControlRowFactory _housingBatchControlRowFactory;

	private readonly BatchControlRowGroupFactory _batchControlRowGroupFactory;

	public override string TabNameLocKey => "BatchControl.Housing";

	public override string TabImage => "Housing";

	public override string BindingKey => "HousingTab";

	public HousingBatchControlTab(VisualElementLoader visualElementLoader, BatchControlDistrict batchControlDistrict, HousingBatchControlRowFactory housingBatchControlRowFactory, BatchControlRowGroupFactory batchControlRowGroupFactory, EventBus eventBus)
		: base(visualElementLoader, batchControlDistrict, eventBus)
	{
		_housingBatchControlRowFactory = housingBatchControlRowFactory;
		_batchControlRowGroupFactory = batchControlRowGroupFactory;
	}

	protected override IEnumerable<BatchControlRowGroup> GetRowGroups(IEnumerable<EntityComponent> entities)
	{
		IEnumerable<IGrouping<string, EntityComponent>> enumerable = from gameObject in entities
			where (bool)gameObject.GetComponent<Dwelling>() || (bool)gameObject.GetComponent<BreedingPod>()
			group gameObject by gameObject.GetComponent<LabeledEntitySpec>().DisplayNameLocKey;
		foreach (IGrouping<string, EntityComponent> item in enumerable)
		{
			BatchControlRowGroup batchControlRowGroup = _batchControlRowGroupFactory.CreateSortedWithTextHeader(item.Key);
			foreach (EntityComponent item2 in item)
			{
				batchControlRowGroup.AddRow(_housingBatchControlRowFactory.Create(item2));
			}
			yield return batchControlRowGroup;
		}
	}
}
