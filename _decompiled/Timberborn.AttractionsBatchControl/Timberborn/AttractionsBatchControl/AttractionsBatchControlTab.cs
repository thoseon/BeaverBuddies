using System.Collections.Generic;
using System.Linq;
using Timberborn.Attractions;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;

namespace Timberborn.AttractionsBatchControl;

internal class AttractionsBatchControlTab : BatchControlTab
{
	private readonly BatchControlRowGroupFactory _batchControlRowGroupFactory;

	private readonly AttractionsBatchControlRowFactory _attractionsBatchControlRowFactory;

	public override string TabNameLocKey => "Wellbeing.DisplayName";

	public override string TabImage => "Attractions";

	public override string BindingKey => "AttractionsTab";

	public AttractionsBatchControlTab(VisualElementLoader visualElementLoader, BatchControlDistrict batchControlDistrict, BatchControlRowGroupFactory batchControlRowGroupFactory, AttractionsBatchControlRowFactory attractionsBatchControlRowFactory, EventBus eventBus)
		: base(visualElementLoader, batchControlDistrict, eventBus)
	{
		_batchControlRowGroupFactory = batchControlRowGroupFactory;
		_attractionsBatchControlRowFactory = attractionsBatchControlRowFactory;
	}

	protected override IEnumerable<BatchControlRowGroup> GetRowGroups(IEnumerable<EntityComponent> entities)
	{
		IEnumerable<IGrouping<string, EntityComponent>> enumerable = from entity in entities
			where entity.GetComponent<Attraction>()
			group entity by entity.GetComponent<LabeledEntitySpec>().DisplayNameLocKey;
		foreach (IGrouping<string, EntityComponent> item in enumerable)
		{
			BatchControlRowGroup batchControlRowGroup = _batchControlRowGroupFactory.CreateSortedWithTextHeader(item.Key);
			foreach (EntityComponent item2 in item)
			{
				batchControlRowGroup.AddRow(_attractionsBatchControlRowFactory.Create(item2));
			}
			yield return batchControlRowGroup;
		}
	}
}
