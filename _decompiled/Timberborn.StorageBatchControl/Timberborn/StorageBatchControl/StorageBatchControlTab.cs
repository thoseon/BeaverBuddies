using System.Collections.Generic;
using System.Linq;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using Timberborn.Stockpiles;

namespace Timberborn.StorageBatchControl;

internal class StorageBatchControlTab : BatchControlTab
{
	private readonly StorageBatchControlRowFactory _storageBatchControlRowFactory;

	private readonly BatchControlRowGroupFactory _batchControlRowGroupFactory;

	public override string TabNameLocKey => "BatchControl.Storage";

	public override string TabImage => "Storage";

	public override string BindingKey => "StorageTab";

	public StorageBatchControlTab(VisualElementLoader visualElementLoader, BatchControlDistrict batchControlDistrict, StorageBatchControlRowFactory storageBatchControlRowFactory, BatchControlRowGroupFactory batchControlRowGroupFactory, EventBus eventBus)
		: base(visualElementLoader, batchControlDistrict, eventBus)
	{
		_storageBatchControlRowFactory = storageBatchControlRowFactory;
		_batchControlRowGroupFactory = batchControlRowGroupFactory;
	}

	protected override IEnumerable<BatchControlRowGroup> GetRowGroups(IEnumerable<EntityComponent> entities)
	{
		IEnumerable<IGrouping<string, EntityComponent>> enumerable = from entity in entities
			where entity.GetComponent<Stockpile>()
			group entity by entity.GetComponent<LabeledEntitySpec>().DisplayNameLocKey;
		foreach (IGrouping<string, EntityComponent> item in enumerable)
		{
			string groupSortingKey = GetGroupSortingKey(item);
			BatchControlRowGroup batchControlRowGroup = _batchControlRowGroupFactory.CreateSortedWithTextHeader(item.Key, groupSortingKey);
			foreach (EntityComponent item2 in item)
			{
				batchControlRowGroup.AddRow(_storageBatchControlRowFactory.Create(item2));
			}
			yield return batchControlRowGroup;
		}
	}

	private static string GetGroupSortingKey(IGrouping<string, EntityComponent> group)
	{
		EntityComponent entityComponent = group.First();
		Stockpile component = entityComponent.GetComponent<Stockpile>();
		if (component != null)
		{
			return $"{component.WhitelistedGoodType}_{component.MaxCapacity:00000}";
		}
		return "_" + entityComponent.GetComponent<LabeledEntitySpec>().DisplayNameLocKey;
	}
}
