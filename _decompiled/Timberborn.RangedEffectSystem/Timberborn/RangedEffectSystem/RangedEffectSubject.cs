using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.EnterableSystem;
using Timberborn.NeedSystem;
using Timberborn.TickSystem;
using Timberborn.TimeSystem;
using UnityEngine;

namespace Timberborn.RangedEffectSystem;

internal class RangedEffectSubject : TickableComponent, IAwakableComponent
{
	private readonly RangedEffectService _rangedEffectService;

	private readonly IDayNightCycle _dayNightCycle;

	private NeedManager _needManager;

	private Enterer _enterer;

	public RangedEffectSubject(RangedEffectService rangedEffectService, IDayNightCycle dayNightCycle)
	{
		_rangedEffectService = rangedEffectService;
		_dayNightCycle = dayNightCycle;
	}

	public void Awake()
	{
		_needManager = GetComponent<NeedManager>();
		_enterer = GetComponent<Enterer>();
	}

	public override void Tick()
	{
		ApplyEffects();
	}

	private void ApplyEffects()
	{
		ReadOnlyList<RangedEffect> affectingEffects = GetAffectingEffects();
		float fixedDeltaTimeInHours = _dayNightCycle.FixedDeltaTimeInHours;
		foreach (RangedEffect item in affectingEffects)
		{
			_needManager.ApplyEffect(item.ToContinuousEffect(), fixedDeltaTimeInHours);
		}
	}

	private ReadOnlyList<RangedEffect> GetAffectingEffects()
	{
		if (_enterer.IsInside)
		{
			return _enterer.CurrentBuilding.GetComponent<RangedEffectsAffectingEnterable>().ActiveEffects;
		}
		Vector3Int value = CoordinateSystem.WorldToGridInt(base.Transform.position);
		return _rangedEffectService.GetEffectsAffectingCoordinates(value.XY());
	}
}
