using System;
using Timberborn.AutomationBuildings;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.StatusSystem;

namespace Timberborn.AutomationBuildingsUI;

internal class GateConflictStatus : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private static readonly string ConflictLocKey = "Status.Buildings.GateConflict";

	private static readonly string ConflictShortLocKey = "Status.Buildings.GateConflict.Short";

	private readonly ILoc _loc;

	private Gate _gate;

	private StatusToggle _statusToggle;

	public GateConflictStatus(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_gate = GetComponent<Gate>();
		_statusToggle = StatusToggle.CreateNormalStatusWithAlertAndFloatingIcon("GateConflict", _loc.T(ConflictLocKey), _loc.T(ConflictShortLocKey));
		_gate.StateChanged += OnStateChanged;
	}

	public void InitializeEntity()
	{
		GetComponent<StatusSubject>().RegisterStatus(_statusToggle);
	}

	private void OnStateChanged(object sender, EventArgs e)
	{
		_statusToggle.Toggle(_gate.IsConflict);
	}
}
