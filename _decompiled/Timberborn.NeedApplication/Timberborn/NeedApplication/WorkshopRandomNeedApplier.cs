using System;
using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Characters;
using Timberborn.Common;
using Timberborn.Effects;
using Timberborn.EnterableSystem;
using Timberborn.NeedSystem;
using Timberborn.Persistence;
using Timberborn.TimeSystem;
using Timberborn.WorkSystem;
using Timberborn.Workshops;
using Timberborn.WorldPersistence;

namespace Timberborn.NeedApplication;

public class WorkshopRandomNeedApplier : BaseComponent, IAwakableComponent, IFinishedStateListener, IPersistentEntity, IProbabilityGroupProvider
{
	private static readonly float CheckIntervalInDays = 1f / 24f;

	private static readonly ComponentKey WorkshopRandomNeedApplierKey = new ComponentKey("WorkshopRandomNeedApplier");

	private static readonly PropertyKey<float> ApplicationTriggerProgressKey = new PropertyKey<float>("ApplicationTriggerProgress");

	private readonly EffectProbabilityService _effectProbabilityService;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly ITimeTriggerFactory _timeTriggerFactory;

	private Workplace _workplace;

	private Workshop _workshop;

	private Enterable _enterable;

	private WorkshopRandomNeedApplierSpec _workshopRandomNeedApplierSpec;

	private ITimeTrigger _applicationTrigger;

	public string ProbabilityGroupId => "WorkshopRandomNeedApplier";

	public event EventHandler<NeedAppliedEventArgs> NeedApplied;

	public WorkshopRandomNeedApplier(EffectProbabilityService effectProbabilityService, IRandomNumberGenerator randomNumberGenerator, ITimeTriggerFactory timeTriggerFactory)
	{
		_effectProbabilityService = effectProbabilityService;
		_randomNumberGenerator = randomNumberGenerator;
		_timeTriggerFactory = timeTriggerFactory;
	}

	public void Awake()
	{
		_workplace = GetComponent<Workplace>();
		_workshop = GetComponent<Workshop>();
		_enterable = GetComponent<Enterable>();
		_workshopRandomNeedApplierSpec = GetComponent<WorkshopRandomNeedApplierSpec>();
		_applicationTrigger = _timeTriggerFactory.Create(TryApplyNeeds, CheckIntervalInDays);
		DisableComponent();
	}

	public void OnEnterFinishedState()
	{
		_applicationTrigger.Resume();
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		_applicationTrigger.Pause();
		DisableComponent();
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(WorkshopRandomNeedApplierKey).Set(ApplicationTriggerProgressKey, _applicationTrigger.Progress);
	}

	public void Load(IEntityLoader entityLoader)
	{
		float progress = entityLoader.GetComponent(WorkshopRandomNeedApplierKey).Get(ApplicationTriggerProgressKey);
		_applicationTrigger.FastForwardProgress(progress);
	}

	private void TryApplyNeeds()
	{
		if (_workshop.CurrentlyWorking)
		{
			for (int i = 0; i < _workplace.AssignedWorkers.Count; i++)
			{
				Worker worker = _workplace.AssignedWorkers[i];
				if (((BaseComponent)(object)worker).GetComponent<Enterer>().CurrentBuilding == _enterable)
				{
					TryApplyRandomNeedToWorker(worker);
				}
			}
		}
		_applicationTrigger.Reset();
		_applicationTrigger.Resume();
	}

	private void TryApplyRandomNeedToWorker(Worker worker)
	{
		ImmutableArray<NeedApplierEffectSpec> effects = _workshopRandomNeedApplierSpec.Effects;
		NeedApplierEffectSpec listElement = _randomNumberGenerator.GetListElement(effects);
		if (_effectProbabilityService.CanApply(this, listElement))
		{
			ApplyNeed(worker, listElement);
		}
	}

	private void ApplyNeed(Worker worker, NeedApplierEffectSpec effectToApply)
	{
		NeedManager component = ((BaseComponent)(object)worker).GetComponent<NeedManager>();
		if ((bool)(BaseComponent)(object)component && component.HasNeed(effectToApply.NeedId))
		{
			InstantEffect effect = effectToApply.ToInstantEffect();
			component.ApplyEffect(in effect);
			NeedApplied?.Invoke(this, new NeedAppliedEventArgs(((BaseComponent)(object)worker).GetComponent<Character>(), effect));
		}
	}
}
