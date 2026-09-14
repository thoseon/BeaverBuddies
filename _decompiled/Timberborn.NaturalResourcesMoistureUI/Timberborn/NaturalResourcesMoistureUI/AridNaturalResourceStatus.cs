using System;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.NaturalResourcesLifecycle;
using Timberborn.NaturalResourcesMoisture;
using Timberborn.StatusSystem;

namespace Timberborn.NaturalResourcesMoistureUI;

internal class AridNaturalResourceStatus : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private static readonly string OverwateredLocKey = "Status.NaturalResources.Overwatered";

	private readonly ILoc _loc;

	private LivingNaturalResource _livingNaturalResource;

	private AridNaturalResource _aridNaturalResource;

	private StatusToggle _overwateredStatusToggle;

	public AridNaturalResourceStatus(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_livingNaturalResource = GetComponent<LivingNaturalResource>();
		_aridNaturalResource = GetComponent<AridNaturalResource>();
		_overwateredStatusToggle = StatusToggle.CreateNormalStatus("OverwateredNaturalResource", _loc.T(OverwateredLocKey));
	}

	public void InitializeEntity()
	{
		GetComponent<StatusSubject>().RegisterStatus(_overwateredStatusToggle);
		_livingNaturalResource.Died += OnDied;
		_aridNaturalResource.StartedDying += OnStartedDying;
		_aridNaturalResource.StoppedDying += OnStoppedDying;
		if (!_livingNaturalResource.IsDead && _aridNaturalResource.DyingProgress.IsDying)
		{
			_overwateredStatusToggle.Activate();
		}
	}

	private void OnDied(object sender, EventArgs e)
	{
		_overwateredStatusToggle.Deactivate();
	}

	private void OnStartedDying(object sender, EventArgs e)
	{
		_overwateredStatusToggle.Activate();
	}

	private void OnStoppedDying(object sender, EventArgs e)
	{
		_overwateredStatusToggle.Deactivate();
	}
}
