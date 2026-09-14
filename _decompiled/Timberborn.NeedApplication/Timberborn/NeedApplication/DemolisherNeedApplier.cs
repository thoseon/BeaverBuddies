using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.Demolishing;
using Timberborn.NeedSystem;
using Timberborn.Persistence;
using Timberborn.ReservableSystem;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.NeedApplication;

internal class DemolisherNeedApplier : BaseComponent, IAwakableComponent, IPersistentEntity, IProbabilityGroupProvider
{
	private static readonly float CheckIntervalInDays = 1f / 24f;

	private static readonly ComponentKey DemolisherNeedApplierKey = new ComponentKey("DemolisherNeedApplier");

	private static readonly PropertyKey<float> ApplicationTriggerProgressKey = new PropertyKey<float>("ApplicationTriggerProgress");

	private readonly EffectProbabilityService _effectProbabilityService;

	private readonly ITimeTriggerFactory _timeTriggerFactory;

	private NeedManager _needManager;

	private Demolisher _demolisher;

	private DemolishExecutor _demolishExecutor;

	private DemolishableEffectsSpec _demolishableEffectsSpec;

	private ITimeTrigger _applicationTrigger;

	private bool _isEnabled;

	public string ProbabilityGroupId => "DemolisherNeedApplier";

	public DemolisherNeedApplier(EffectProbabilityService effectProbabilityService, ITimeTriggerFactory timeTriggerFactory)
	{
		_effectProbabilityService = effectProbabilityService;
		_timeTriggerFactory = timeTriggerFactory;
	}

	public void Awake()
	{
		_needManager = GetComponent<NeedManager>();
		_demolisher = GetComponent<Demolisher>();
		_demolisher.ReservedDemolishableChanged += OnReservedDemolishableChanged;
		_demolishExecutor = GetComponent<DemolishExecutor>();
		_applicationTrigger = _timeTriggerFactory.Create(ApplyNeeds, CheckIntervalInDays);
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(DemolisherNeedApplierKey).Set(ApplicationTriggerProgressKey, _applicationTrigger.Progress);
	}

	[BackwardCompatible(2025, 8, 21, Compatibility.Save)]
	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(DemolisherNeedApplierKey, out var objectLoader))
		{
			float progress = objectLoader.Get(ApplicationTriggerProgressKey);
			_applicationTrigger.FastForwardProgress(progress);
		}
	}

	private void OnReservedDemolishableChanged(object sender, Demolishable demolishable)
	{
		if ((bool)demolishable)
		{
			DemolishableEffectsSpec component = demolishable.GetComponent<DemolishableEffectsSpec>();
			if ((object)component != null)
			{
				Enable(component);
				return;
			}
		}
		Disable();
	}

	private void Enable(DemolishableEffectsSpec demolishableEffectsSpec)
	{
		if (!_isEnabled)
		{
			_demolishableEffectsSpec = demolishableEffectsSpec;
			_demolishExecutor.WorkStarted += OnWorkStarted;
			_demolishExecutor.WorkFinished += OnWorkFinished;
			_isEnabled = true;
		}
	}

	private void Disable()
	{
		if (_isEnabled)
		{
			_demolishableEffectsSpec = null;
			_demolishExecutor.WorkStarted -= OnWorkStarted;
			_demolishExecutor.WorkFinished -= OnWorkFinished;
			_applicationTrigger.Pause();
			_isEnabled = false;
		}
	}

	private void OnWorkStarted(object sender, EventArgs e)
	{
		_applicationTrigger.Resume();
	}

	private void OnWorkFinished(object sender, WorkFinishedEventArgs e)
	{
		_applicationTrigger.Pause();
	}

	private void ApplyNeeds()
	{
		if (!(_demolishableEffectsSpec != null))
		{
			return;
		}
		foreach (NeedApplierEffectSpec effect in _demolishableEffectsSpec.Effects)
		{
			if (!_needManager.NeedIsActive(effect.NeedId) && _effectProbabilityService.CanApply(this, effect))
			{
				_needManager.ApplyEffect(effect.ToInstantEffect());
			}
		}
		_applicationTrigger.Reset();
		_applicationTrigger.Resume();
	}
}
