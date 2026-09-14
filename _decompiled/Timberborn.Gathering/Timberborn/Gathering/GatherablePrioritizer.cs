using System;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.DuplicationSystem;
using Timberborn.Persistence;
using Timberborn.TemplateSystem;
using Timberborn.WorldPersistence;
using Timberborn.Yielding;

namespace Timberborn.Gathering;

public class GatherablePrioritizer : BaseComponent, IAwakableComponent, IPersistentEntity, IDuplicable<GatherablePrioritizer>, IDuplicable, IFinishedStateListener
{
	private static readonly ComponentKey GatherablePrioritizerKey = new ComponentKey("GatherablePrioritizer");

	private static readonly PropertyKey<string> PrioritizedGatherableKey = new PropertyKey<string>("PrioritizedGatherable");

	private YieldRemovingBuilding _yieldRemovingBuilding;

	public GatherableSpec PrioritizedGatherable { get; private set; }

	public event EventHandler PrioritizedGatherableChanged;

	public void Awake()
	{
		_yieldRemovingBuilding = GetComponent<YieldRemovingBuilding>();
		DisableComponent();
	}

	public void PrioritizeGatherable(GatherableSpec gatherableSpec)
	{
		PrioritizedGatherable = gatherableSpec;
		PrioritizedGatherableChanged?.Invoke(this, EventArgs.Empty);
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (PrioritizedGatherable != null)
		{
			entitySaver.GetComponent(GatherablePrioritizerKey).Set(value: PrioritizedGatherable.GetSpec<TemplateSpec>().TemplateName, key: PrioritizedGatherableKey);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(GatherablePrioritizerKey, out var objectLoader) && objectLoader.Has(PrioritizedGatherableKey))
		{
			string templateName = objectLoader.Get(PrioritizedGatherableKey);
			PrioritizedGatherable = GetGatherable(templateName);
		}
	}

	public void DuplicateFrom(GatherablePrioritizer source)
	{
		GatherableSpec prioritizedGatherable = source.PrioritizedGatherable;
		if (prioritizedGatherable == null || SupportsGatherable(prioritizedGatherable))
		{
			PrioritizeGatherable(prioritizedGatherable);
		}
	}

	private bool SupportsGatherable(GatherableSpec gatherableSpec)
	{
		return GetGatherable(gatherableSpec.GetSpec<TemplateSpec>().TemplateName) != null;
	}

	private GatherableSpec GetGatherable(string templateName)
	{
		return _yieldRemovingBuilding.GetAllowedYielders().Cast<ComponentSpec>().SingleOrDefault((ComponentSpec yielder) => yielder.GetSpec<TemplateSpec>().IsNamed(templateName))?.GetSpec<GatherableSpec>();
	}
}
