using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.NeedSpecs;

namespace Timberborn.RangedEffectSystem;

internal class ContinuousEffectBuilding : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private ContinuousEffectBuildingSpec _continuousEffectBuildingSpec;

	private RangedEffectBuilding _rangedEffectBuilding;

	public ImmutableArray<ContinuousEffectSpec> Effects => _continuousEffectBuildingSpec.Effects;

	public void Awake()
	{
		_continuousEffectBuildingSpec = GetComponent<ContinuousEffectBuildingSpec>();
		_rangedEffectBuilding = GetComponent<RangedEffectBuilding>();
	}

	public void OnEnterFinishedState()
	{
		foreach (ContinuousEffectSpec effect in Effects)
		{
			_rangedEffectBuilding.AddEffect(effect);
		}
	}

	public void OnExitFinishedState()
	{
		foreach (ContinuousEffectSpec effect in Effects)
		{
			_rangedEffectBuilding.RemoveEffect(effect);
		}
	}
}
