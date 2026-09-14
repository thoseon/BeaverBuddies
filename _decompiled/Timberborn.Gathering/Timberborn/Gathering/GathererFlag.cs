using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.TemplateSystem;
using Timberborn.Yielding;

namespace Timberborn.Gathering;

public class GathererFlag : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private YieldRemovingBuilding _yieldRemovingBuilding;

	private readonly HashSet<string> _allowedGatherables = new HashSet<string>();

	public ImmutableArray<GatherableSpec> AllowedGatherables { get; private set; }

	public void Awake()
	{
		_yieldRemovingBuilding = GetComponent<YieldRemovingBuilding>();
	}

	public void InitializeEntity()
	{
		AllowedGatherables = GetAllowedGatherables().ToImmutableArray();
		IEnumerable<string> values = AllowedGatherables.Select((GatherableSpec gatherable) => gatherable.GetSpec<TemplateSpec>().TemplateName);
		_allowedGatherables.AddRange(values);
	}

	public bool CanGather(string templateName)
	{
		return _allowedGatherables.Contains(templateName);
	}

	private IEnumerable<GatherableSpec> GetAllowedGatherables()
	{
		return from yielder in _yieldRemovingBuilding.GetAllowedYielders()
			select ((ComponentSpec)yielder).GetSpec<GatherableSpec>();
	}
}
