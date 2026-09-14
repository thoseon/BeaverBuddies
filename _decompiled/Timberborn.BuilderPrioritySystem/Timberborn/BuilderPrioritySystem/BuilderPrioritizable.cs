using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.DuplicationSystem;
using Timberborn.Persistence;
using Timberborn.PrioritySystem;
using Timberborn.WorldPersistence;

namespace Timberborn.BuilderPrioritySystem;

public class BuilderPrioritizable : BaseComponent, IAwakableComponent, IPersistentEntity, IDuplicable<BuilderPrioritizable>, IDuplicable, IPrioritizable
{
	private static readonly ComponentKey BuilderPrioritizableKey = new ComponentKey("BuilderPrioritizable");

	private static readonly PropertyKey<Priority> PriorityKey = new PropertyKey<Priority>("Priority");

	private BlockObject _blockObject;

	public Priority Priority { get; private set; } = Priority.Normal;

	public bool IsDuplicable => base.Enabled;

	public event EventHandler<PriorityChangedEventArgs> PriorityChanged;

	public event EventHandler PrioritizableEnabled;

	public event EventHandler PrioritizableDisabled;

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		DisableComponent();
	}

	public void Enable()
	{
		EnableComponent();
		PrioritizableEnabled?.Invoke(this, EventArgs.Empty);
	}

	public void Disable()
	{
		Priority = Priority.Normal;
		DisableComponent();
		PrioritizableDisabled?.Invoke(this, EventArgs.Empty);
	}

	public void SetPriority(Priority priority)
	{
		Priority priority2 = Priority;
		Priority = priority;
		PriorityChanged?.Invoke(this, new PriorityChangedEventArgs(priority2));
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (Priority != Priority.Normal)
		{
			entitySaver.GetComponent(BuilderPrioritizableKey).Set(PriorityKey, Priority);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(BuilderPrioritizableKey, out var objectLoader))
		{
			Priority = objectLoader.Get(PriorityKey);
		}
	}

	public void DuplicateFrom(BuilderPrioritizable source)
	{
		if ((base.Enabled || !_blockObject.IsFinished) && source.Enabled)
		{
			SetPriority(source.Priority);
		}
	}
}
