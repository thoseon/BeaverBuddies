using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BeaverContaminationSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.StatusSystem;

namespace Timberborn.BeaverContaminationSystemUI;

internal class ContaminationIncubatorStatus : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private static readonly string IncubationLocKey = "Status.BadwaterContamination.Incubation";

	private static readonly string IncubationShortLocKey = "Status.BadwaterContamination.Incubation.Short";

	private readonly ILoc _loc;

	private StatusToggle _statusToggle;

	private ContaminationIncubator _contaminationIncubator;

	public ContaminationIncubatorStatus(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_statusToggle = StatusToggle.CreateNormalStatusWithAlert("Incubation", _loc.T(IncubationLocKey), _loc.T(IncubationShortLocKey));
		_contaminationIncubator = GetComponent<ContaminationIncubator>();
		_contaminationIncubator.IncubationStateChanged += OnIncubationStateChanged;
	}

	public void InitializeEntity()
	{
		GetComponent<StatusSubject>().RegisterStatus(_statusToggle);
	}

	private void OnIncubationStateChanged(object sender, EventArgs e)
	{
		if (_contaminationIncubator.IsIncubating || _contaminationIncubator.IncubationFinished)
		{
			_statusToggle.Activate();
		}
		else
		{
			_statusToggle.Deactivate();
		}
	}
}
