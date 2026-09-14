using System.Collections.Generic;
using System.Linq;
using Timberborn.BatchControl;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.MechanicalSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.PowerBatchControl;

internal class MechanicalBatchControlTab : BatchControlTab
{
	private readonly MechanicalBatchControlRowFactory _mechanicalBatchControlRowFactory;

	private readonly BatchControlRowGroupFactory _batchControlRowGroupFactory;

	private readonly Dictionary<MechanicalGraph, List<MechanicalNode>> _graphs = new Dictionary<MechanicalGraph, List<MechanicalNode>>();

	public override string TabNameLocKey => "BatchControl.Mechanical";

	public override string TabImage => "Mechanical";

	public override string BindingKey => "MechanicalTab";

	public override bool IgnoreDistrictSelection => true;

	public MechanicalBatchControlTab(VisualElementLoader visualElementLoader, BatchControlDistrict batchControlDistrict, MechanicalBatchControlRowFactory mechanicalBatchControlRowFactory, EventBus eventBus, BatchControlRowGroupFactory batchControlRowGroupFactory)
		: base(visualElementLoader, batchControlDistrict, eventBus)
	{
		_mechanicalBatchControlRowFactory = mechanicalBatchControlRowFactory;
		_batchControlRowGroupFactory = batchControlRowGroupFactory;
	}

	[OnEvent]
	public void OnMechanicalGraphCreated(MechanicalGraphCreatedEvent mechanicalGraphCreatedEvent)
	{
		HideAndMarkForRefresh();
	}

	[OnEvent]
	public void OnMechanicalGraphRemoved(MechanicalGraphRemovedEvent mechanicalGraphRemovedEvent)
	{
		HideAndMarkForRefresh();
	}

	protected override IEnumerable<BatchControlRowGroup> GetRowGroups(IEnumerable<EntityComponent> entities)
	{
		IEnumerable<MechanicalNode> nodes = from entity in entities
			where entity.GetComponent<MechanicalBuilding>()?.Enabled ?? false
			select entity.GetComponent<MechanicalNode>();
		GatherGraphs(nodes);
		return GetRows();
	}

	private void HideAndMarkForRefresh()
	{
		HideContent();
		base.IsDirty = true;
	}

	private void GatherGraphs(IEnumerable<MechanicalNode> nodes)
	{
		foreach (MechanicalNode node in nodes)
		{
			MechanicalGraph graph = node.Graph;
			if (graph != null)
			{
				_graphs.GetOrAdd(graph).Add(node);
			}
		}
	}

	private IEnumerable<BatchControlRowGroup> GetRows()
	{
		foreach (MechanicalGraph key in _graphs.Keys)
		{
			BatchControlRow header = _mechanicalBatchControlRowFactory.Create(key);
			BatchControlRowGroup batchControlRowGroup = _batchControlRowGroupFactory.CreateUnsorted(header);
			foreach (MechanicalNode item in _graphs[key])
			{
				EntityComponent component = item.GetComponent<EntityComponent>();
				batchControlRowGroup.AddRow(_mechanicalBatchControlRowFactory.Create(component));
			}
			yield return batchControlRowGroup;
		}
		_graphs.Clear();
	}
}
