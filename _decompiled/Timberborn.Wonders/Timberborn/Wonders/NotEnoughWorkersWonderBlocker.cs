using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.StatusSystem;
using Timberborn.WorkSystem;

namespace Timberborn.Wonders;

public class NotEnoughWorkersWonderBlocker : BaseComponent, IAwakableComponent, IInitializableEntity, IWonderBlocker
{
	private static readonly string DisallowReasonLocKey = "Status.Wonder.NotEnoughWorkers";

	private readonly ILoc _loc;

	private Wonder _wonder;

	private Workplace _workplace;

	private StatusToggle _statusToggle;

	public NotEnoughWorkersWonderBlocker(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_wonder = GetComponent<Wonder>();
		_workplace = GetComponent<Workplace>();
		_statusToggle = StatusToggle.CreateNormalStatus("NoUnemployed", _loc.T(DisallowReasonLocKey));
		_workplace.WorkerAssigned += delegate
		{
			UpdateStatus();
		};
		_workplace.WorkerUnassigned += delegate
		{
			UpdateStatus();
		};
	}

	public void InitializeEntity()
	{
		GetComponent<StatusSubject>().RegisterStatus(_statusToggle);
	}

	public bool IsWonderBlocked()
	{
		return _workplace.AssignedWorkers.Count != _workplace.MaxWorkers;
	}

	private void UpdateStatus()
	{
		if (!IsWonderBlocked() || _wonder.IsActive)
		{
			_statusToggle.Deactivate();
		}
		else
		{
			_statusToggle.Activate();
		}
	}
}
