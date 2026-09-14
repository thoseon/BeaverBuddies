using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.StatusSystem;

namespace Timberborn.AutomationUI;

public class AutomationLoopStatus : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private static readonly string AutomationLoopLocKey = "Status.Automation.AutomationLoop";

	private static readonly string AutomationLoopShortLocKey = "Status.Automation.AutomationLoop.Short";

	private readonly ILoc _loc;

	private Automator _automator;

	private StatusToggle _statusToggle;

	public AutomationLoopStatus(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_automator = GetComponent<Automator>();
		_statusToggle = StatusToggle.CreateNormalStatusWithAlertAndFloatingIcon("AutomationLoop", _loc.T(AutomationLoopLocKey), _loc.T(AutomationLoopShortLocKey));
	}

	public void InitializeEntity()
	{
		GetComponent<StatusSubject>().RegisterStatus(_statusToggle);
		_automator.IsCyclicOrBlockedChanged += delegate
		{
			UpdateStatus();
		};
		UpdateStatus();
	}

	private void UpdateStatus()
	{
		_statusToggle.Toggle(_automator.IsCyclicOrBlocked);
	}
}
