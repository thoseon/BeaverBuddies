using System;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.NaturalResourcesLifecycle;
using Timberborn.NaturalResourcesMoisture;
using Timberborn.StatusSystem;

namespace Timberborn.NaturalResourcesMoistureUI;

internal class LivingWaterNaturalResourceStatus : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private static readonly string TooMuchWaterLocKey = "Status.NaturalResources.TooMuchWater";

	private static readonly string NotEnoughWaterLocKey = "Status.NaturalResources.NotEnoughWater";

	private readonly ILoc _loc;

	private LivingNaturalResource _livingNaturalResource;

	private LivingWaterNaturalResource _livingWaterNaturalResource;

	private StatusToggle _tooMuchWaterStatusToggle;

	private StatusToggle _notEnoughWaterStatusToggle;

	public LivingWaterNaturalResourceStatus(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_livingNaturalResource = GetComponent<LivingNaturalResource>();
		_livingWaterNaturalResource = GetComponent<LivingWaterNaturalResource>();
		_tooMuchWaterStatusToggle = StatusToggle.CreateNormalStatus("TooMuchWater", _loc.T(TooMuchWaterLocKey));
		_notEnoughWaterStatusToggle = StatusToggle.CreateNormalStatus("NotEnoughWater", _loc.T(NotEnoughWaterLocKey));
	}

	public void InitializeEntity()
	{
		StatusSubject component = GetComponent<StatusSubject>();
		component.RegisterStatus(_tooMuchWaterStatusToggle);
		component.RegisterStatus(_notEnoughWaterStatusToggle);
		_livingNaturalResource.Died += OnDied;
		_livingWaterNaturalResource.StartedDying += OnStartedDying;
		_livingWaterNaturalResource.StoppedDying += OnStoppedDying;
		if (!_livingNaturalResource.IsDead && _livingWaterNaturalResource.DyingProgress.IsDying)
		{
			ActivateStatus();
		}
	}

	private void OnDied(object sender, EventArgs e)
	{
		DeactivateStatuses();
	}

	private void OnStartedDying(object sender, EventArgs e)
	{
		ActivateStatus();
	}

	private void OnStoppedDying(object sender, EventArgs e)
	{
		DeactivateStatuses();
	}

	private void DeactivateStatuses()
	{
		_tooMuchWaterStatusToggle.Deactivate();
		_notEnoughWaterStatusToggle.Deactivate();
	}

	private void ActivateStatus()
	{
		if (_livingWaterNaturalResource.DeathByFlooding)
		{
			_tooMuchWaterStatusToggle.Activate();
		}
		else
		{
			_notEnoughWaterStatusToggle.Activate();
		}
	}
}
