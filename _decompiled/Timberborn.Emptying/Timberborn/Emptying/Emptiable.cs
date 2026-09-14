using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.InventorySystem;
using Timberborn.Localization;
using Timberborn.Navigation;
using Timberborn.Persistence;
using Timberborn.StatusSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Emptying;

public class Emptiable : BaseComponent, IAwakableComponent, IInitializableEntity, IPersistentEntity, IAccessibleValidator, IFinishedStateListener, IInventoryValidator, IRegisteredComponent
{
	private static readonly string EmptyingInProgressLocKey = "Status.Emptying.EmptyingInProgress";

	private static readonly ComponentKey EmptiableKey = new ComponentKey("Emptiable");

	private static readonly PropertyKey<bool> IsMarkedForEmptyingKey = new PropertyKey<bool>("IsMarkedForEmptying");

	private static readonly PropertyKey<bool> StatusIsActiveKey = new PropertyKey<bool>("StatusIsActive");

	private readonly ILoc _loc;

	private StatusToggle _emptyStatusToggle;

	public bool IsMarkedForEmptying { get; private set; }

	public bool ValidAccessible
	{
		get
		{
			if (base.Enabled)
			{
				return !IsMarkedForEmptying;
			}
			return true;
		}
	}

	public bool ValidInventory
	{
		get
		{
			if (base.Enabled)
			{
				return !IsMarkedForEmptying;
			}
			return true;
		}
	}

	public event EventHandler MarkedForEmptying;

	public event EventHandler UnmarkedForEmptying;

	public Emptiable(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_emptyStatusToggle = StatusToggle.CreatePriorityStatusWithFloatingIcon("Empty", _loc.T(EmptyingInProgressLocKey));
		DisableComponent();
	}

	public void InitializeEntity()
	{
		GetComponent<StatusSubject>().RegisterStatus(_emptyStatusToggle);
		if (IsMarkedForEmptying)
		{
			MarkForEmptying();
		}
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		UnmarkForEmptying();
		DisableComponent();
	}

	public void MarkForEmptyingWithStatus()
	{
		MarkForEmptying();
		_emptyStatusToggle.Activate();
	}

	public void MarkForEmptyingWithoutStatus()
	{
		MarkForEmptying();
	}

	public void UnmarkForEmptying()
	{
		_emptyStatusToggle.Deactivate();
		IsMarkedForEmptying = false;
		UnmarkedForEmptying?.Invoke(this, EventArgs.Empty);
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (IsMarkedForEmptying)
		{
			IObjectSaver component = entitySaver.GetComponent(EmptiableKey);
			component.Set(IsMarkedForEmptyingKey, IsMarkedForEmptying);
			component.Set(StatusIsActiveKey, _emptyStatusToggle.IsActive);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(EmptiableKey, out var objectLoader))
		{
			IsMarkedForEmptying = objectLoader.Get(IsMarkedForEmptyingKey);
			if (objectLoader.Get(StatusIsActiveKey))
			{
				_emptyStatusToggle.Activate();
			}
		}
	}

	private void MarkForEmptying()
	{
		IsMarkedForEmptying = true;
		MarkedForEmptying?.Invoke(this, EventArgs.Empty);
	}
}
