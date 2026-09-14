using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.DuplicationSystem;
using Timberborn.Persistence;
using Timberborn.PrioritySystem;
using Timberborn.TemplateSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.WorkSystem;

public class WorkplacePriority : BaseComponent, IAwakableComponent, IPersistentEntity, IDuplicable<WorkplacePriority>, IDuplicable, IFinishedStateListener, IPrioritizable
{
	private static readonly ComponentKey WorkplacePriorityKey = new ComponentKey("WorkplacePriority");

	private static readonly PropertyKey<Priority> PriorityKey = new PropertyKey<Priority>("Priority");

	private InstantiatedTemplate _instantiatedTemplate;

	public Priority Priority { get; private set; } = Priority.Normal;

	public Workplace Workplace { get; private set; }

	public int InstantiationOrder => _instantiatedTemplate.InstantiationOrder;

	public event EventHandler<PriorityChangedEventArgs> PriorityChanged;

	public void Awake()
	{
		Workplace = GetComponent<Workplace>();
		_instantiatedTemplate = GetComponent<InstantiatedTemplate>();
		DisableComponent();
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
		if (Priority != Priority.Normal)
		{
			entitySaver.GetComponent(WorkplacePriorityKey).Set(PriorityKey, Priority);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(WorkplacePriorityKey, out var objectLoader))
		{
			Priority = objectLoader.Get(PriorityKey);
		}
	}

	public void DuplicateFrom(WorkplacePriority source)
	{
		SetPriority(source.Priority);
	}

	public void SetPriority(Priority priority)
	{
		if (priority != Priority)
		{
			Priority priority2 = Priority;
			Priority = priority;
			PriorityChanged?.Invoke(this, new PriorityChangedEventArgs(priority2));
		}
	}
}
