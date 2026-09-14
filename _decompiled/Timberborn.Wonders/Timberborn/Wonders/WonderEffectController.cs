using System;
using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.NeedSpecs;
using Timberborn.RangedEffectSystem;

namespace Timberborn.Wonders;

internal class WonderEffectController : BaseComponent, IAwakableComponent
{
	private RangedEffectBuilding _rangedEffectBuilding;

	private Wonder _wonder;

	private WonderEffectControllerSpec _wonderEffectControllerSpec;

	public ImmutableArray<ContinuousEffectSpec> Effects => _wonderEffectControllerSpec.Effects;

	public void Awake()
	{
		_rangedEffectBuilding = GetComponent<RangedEffectBuilding>();
		_wonder = GetComponent<Wonder>();
		_wonderEffectControllerSpec = GetComponent<WonderEffectControllerSpec>();
		_wonder.WonderActivated += OnWonderActivated;
		_wonder.WonderDeactivated += OnWonderDeactivated;
	}

	private void EnableEffects()
	{
		foreach (ContinuousEffectSpec effect in Effects)
		{
			_rangedEffectBuilding.AddEffect(effect);
		}
	}

	private void DisableEffects()
	{
		foreach (ContinuousEffectSpec effect in Effects)
		{
			_rangedEffectBuilding.RemoveEffect(effect);
		}
	}

	private void OnWonderActivated(object sender, EventArgs e)
	{
		EnableEffects();
	}

	private void OnWonderDeactivated(object sender, EventArgs e)
	{
		DisableEffects();
	}
}
