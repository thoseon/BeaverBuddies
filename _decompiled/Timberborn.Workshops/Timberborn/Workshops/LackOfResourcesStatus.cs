using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.Localization;
using Timberborn.StatusSystem;
using Timberborn.TickSystem;

namespace Timberborn.Workshops;

public class LackOfResourcesStatus : TickableComponent, IAwakableComponent
{
	private static readonly string LackOfResourcesLocKey = "Status.Work.LackOfResources";

	private static readonly string LackOfResourcesShortLocKey = "Status.Work.LackOfResources.Short";

	private readonly ILoc _loc;

	private StatusToggle _lackOfResourcesStatusToggle;

	private Func<bool> _activePredicate;

	public LackOfResourcesStatus(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_lackOfResourcesStatusToggle = StatusToggle.CreateNormalStatusWithAlert("LackOfResources", _loc.T(LackOfResourcesLocKey), _loc.T(LackOfResourcesShortLocKey), 3f);
		DisableComponent();
	}

	public override void Tick()
	{
		UpdateStatusToggle();
	}

	public void Initialize(Func<bool> activePredicate)
	{
		Asserts.FieldIsNull(this, _activePredicate, "_activePredicate");
		GetComponent<StatusSubject>().RegisterStatus(_lackOfResourcesStatusToggle);
		_activePredicate = activePredicate;
		UpdateStatusToggle();
		EnableComponent();
	}

	public void Disable()
	{
		DisableComponent();
	}

	private void UpdateStatusToggle()
	{
		if (_activePredicate())
		{
			_lackOfResourcesStatusToggle.Activate();
		}
		else
		{
			_lackOfResourcesStatusToggle.Deactivate();
		}
	}
}
