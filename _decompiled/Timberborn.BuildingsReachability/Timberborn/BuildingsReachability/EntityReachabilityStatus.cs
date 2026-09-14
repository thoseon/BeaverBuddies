using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.SelectionSystem;
using Timberborn.StatusSystem;
using Timberborn.TickSystem;

namespace Timberborn.BuildingsReachability;

internal class EntityReachabilityStatus : TickableComponent, IAwakableComponent, IInitializableEntity, ISelectionListener
{
	private static readonly string UnreachableObjectLocKey = "Status.Object.UnreachableObject";

	private readonly ILoc _loc;

	private readonly List<IUnreachableEntity> _unreachableEntities = new List<IUnreachableEntity>();

	private StatusToggle _unreachableStatus;

	public EntityReachabilityStatus(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		GetComponents(_unreachableEntities);
		_unreachableStatus = StatusToggle.CreateNormalStatus("UnreachableObject", _loc.T(UnreachableObjectLocKey));
		DisableComponent();
	}

	public void InitializeEntity()
	{
		GetComponent<StatusSubject>().RegisterStatus(_unreachableStatus);
	}

	public override void Tick()
	{
		UpdateStatus();
	}

	public void OnSelect()
	{
		Enable();
	}

	public void OnUnselect()
	{
		Disable();
	}

	private void UpdateStatus()
	{
		if (IsAnyUnreachable())
		{
			_unreachableStatus.Activate();
		}
		else
		{
			_unreachableStatus.Deactivate();
		}
	}

	private bool IsAnyUnreachable()
	{
		foreach (IUnreachableEntity unreachableEntity in _unreachableEntities)
		{
			if (unreachableEntity.IsUnreachable())
			{
				return true;
			}
		}
		return false;
	}

	private void Enable()
	{
		EnableComponent();
		UpdateStatus();
	}

	private void Disable()
	{
		DisableComponent();
		_unreachableStatus.Deactivate();
	}
}
