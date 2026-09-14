using System;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.NaturalResourcesContamination;
using Timberborn.NaturalResourcesLifecycle;
using Timberborn.StatusSystem;

namespace Timberborn.NaturalResourcesContaminationUI;

internal class ContaminatedNaturalResourceStatus : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private static readonly string ContaminatedLocKey = "Status.NaturalResources.Contaminated";

	private readonly ILoc _loc;

	private LivingNaturalResource _livingNaturalResource;

	private ContaminatedNaturalResource _contaminatedNaturalResource;

	private StatusToggle _contaminatedStatusToggle;

	public ContaminatedNaturalResourceStatus(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_livingNaturalResource = GetComponent<LivingNaturalResource>();
		_contaminatedNaturalResource = GetComponent<ContaminatedNaturalResource>();
		_contaminatedStatusToggle = StatusToggle.CreateNormalStatus("ContaminatedNaturalResource", _loc.T(ContaminatedLocKey));
	}

	public void InitializeEntity()
	{
		GetComponent<StatusSubject>().RegisterStatus(_contaminatedStatusToggle);
		_livingNaturalResource.Died += OnDied;
		_contaminatedNaturalResource.StartedDying += OnStartedDying;
		_contaminatedNaturalResource.StoppedDying += OnStoppedDying;
		if (!_livingNaturalResource.IsDead && _contaminatedNaturalResource.DyingProgress.IsDying)
		{
			_contaminatedStatusToggle.Activate();
		}
	}

	private void OnDied(object sender, EventArgs e)
	{
		_contaminatedStatusToggle.Deactivate();
	}

	private void OnStartedDying(object sender, EventArgs e)
	{
		_contaminatedStatusToggle.Activate();
	}

	private void OnStoppedDying(object sender, EventArgs e)
	{
		_contaminatedStatusToggle.Deactivate();
	}
}
