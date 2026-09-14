using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.MortalComponents;
using Timberborn.StatusSystem;

namespace Timberborn.MortalSystem;

internal class DeadStatus : BaseComponent, IAwakableComponent, IInitializableEntity, IDeadNeededComponent
{
	private readonly ILoc _loc;

	private StatusToggle _oldAgeDeathStatus;

	private StatusToggle _tragicalDeathStatus;

	public DeadStatus(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		DeadStatusSpec component = GetComponent<DeadStatusSpec>();
		string description = _loc.T(component.DiedOldAgeStatusLocKey);
		string statusDescription = _loc.T(component.DiedTragicallyStatusLocKey);
		string alertDescription = _loc.T(component.DiedTragicallyAlertLocKey);
		string spriteName = "Death";
		_oldAgeDeathStatus = StatusToggle.CreatePriorityStatus(spriteName, description);
		_tragicalDeathStatus = StatusToggle.CreatePriorityStatusWithAlertAndFloatingIcon(spriteName, statusDescription, alertDescription).AsNotifying();
	}

	public void InitializeEntity()
	{
		StatusSubject component = GetComponent<StatusSubject>();
		component.RegisterDynamicStatus(_tragicalDeathStatus, () => float.MinValue, () => StatusWarningType.Short, null);
		component.RegisterStatus(_oldAgeDeathStatus);
	}

	public void Activate(bool isTragicalDeath)
	{
		if (isTragicalDeath)
		{
			_tragicalDeathStatus.Activate();
		}
		else
		{
			_oldAgeDeathStatus.Activate();
		}
	}
}
