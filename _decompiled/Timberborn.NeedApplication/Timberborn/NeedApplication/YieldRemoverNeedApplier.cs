using System;
using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.NeedSystem;
using Timberborn.Persistence;
using Timberborn.WorkSystem;
using Timberborn.WorldPersistence;
using Timberborn.Yielding;

namespace Timberborn.NeedApplication;

internal class YieldRemoverNeedApplier : BaseComponent, IAwakableComponent, IPersistentEntity, IProbabilityGroupProvider
{
	private static readonly ComponentKey YieldRemoverNeedApplierKey = new ComponentKey("YieldRemoverNeedApplier");

	private static readonly PropertyKey<int> AttemptCounterKey = new PropertyKey<int>("AttemptCounter");

	private readonly EffectProbabilityService _effectProbabilityService;

	private NeedManager _needManager;

	private YielderRemover _yielderRemover;

	private Worker _worker;

	private YieldRemoverWorkplaceEffectsSpec _yieldRemoverWorkplaceEffectsSpec;

	private int _attemptCounter;

	private bool _isEnabled;

	public string ProbabilityGroupId => "YieldRemoverNeedApplier";

	public YieldRemoverNeedApplier(EffectProbabilityService effectProbabilityService)
	{
		_effectProbabilityService = effectProbabilityService;
	}

	public void Awake()
	{
		_needManager = GetComponent<NeedManager>();
		_yielderRemover = GetComponent<YielderRemover>();
		_worker = GetComponent<Worker>();
		_worker.GotEmployed += OnGotEmployed;
		_worker.GotUnemployed += OnGotUnemployed;
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(YieldRemoverNeedApplierKey).Set(AttemptCounterKey, _attemptCounter);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(YieldRemoverNeedApplierKey);
		_attemptCounter = component.Get(AttemptCounterKey);
	}

	private void OnGotEmployed(object sender, EventArgs e)
	{
		YieldRemoverWorkplaceEffectsSpec component = _worker.Workplace.GetComponent<YieldRemoverWorkplaceEffectsSpec>();
		if ((object)component != null)
		{
			Enable(component);
		}
	}

	private void OnGotUnemployed(object sender, EventArgs e)
	{
		Disable();
	}

	private void Enable(YieldRemoverWorkplaceEffectsSpec yieldRemoverWorkplaceEffectsSpec)
	{
		if (!_isEnabled)
		{
			_yieldRemoverWorkplaceEffectsSpec = yieldRemoverWorkplaceEffectsSpec;
			_yielderRemover.YieldReservationCompleted += OnYieldReservationCompleted;
			_isEnabled = true;
		}
	}

	private void Disable()
	{
		if (_isEnabled)
		{
			_yieldRemoverWorkplaceEffectsSpec = null;
			_yielderRemover.YieldReservationCompleted -= OnYieldReservationCompleted;
			_isEnabled = false;
		}
	}

	private void OnYieldReservationCompleted(object sender, YieldReservationCompletedEventArgs e)
	{
		YielderNeedApplierSpec component = e.Yielder.GetComponent<YielderNeedApplierSpec>();
		if ((object)component != null)
		{
			TryApply(component.Effects);
		}
		if (_yieldRemoverWorkplaceEffectsSpec != null && e.Yield.GoodId == _yieldRemoverWorkplaceEffectsSpec.YieldGoodId)
		{
			TryApplyWithCooldown(_yieldRemoverWorkplaceEffectsSpec.Effects);
		}
	}

	private void TryApplyWithCooldown(ImmutableArray<NeedApplierEffectSpec> effects)
	{
		_attemptCounter++;
		if (_attemptCounter > _yieldRemoverWorkplaceEffectsSpec.MinimumAttemptsThreshold && TryApply(effects))
		{
			_attemptCounter = 0;
		}
	}

	private bool TryApply(ImmutableArray<NeedApplierEffectSpec> effects)
	{
		bool result = false;
		foreach (NeedApplierEffectSpec item in effects)
		{
			if (!_needManager.NeedIsActive(item.NeedId) && _effectProbabilityService.CanApply(this, item))
			{
				_needManager.ApplyEffect(item.ToInstantEffect());
				result = true;
			}
		}
		return result;
	}
}
