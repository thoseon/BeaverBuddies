using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.BlockingSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.Effects;
using Timberborn.EnterableSystem;
using Timberborn.NeedSystem;
using Timberborn.Persistence;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.NeedBehaviorSystem;

public class ApplyEffectExecutor : BaseComponent, IAwakableComponent, IExecutor
{
	private static readonly ComponentKey ApplyEffectExecutorKey = new ComponentKey("ApplyEffectExecutor");

	private static readonly PropertyKey<float> FinishTimestampKey = new PropertyKey<float>("FinishTimestamp");

	private static readonly PropertyKey<string> AnimationNameKey = new PropertyKey<string>("AnimationName");

	private static readonly PropertyKey<bool> WasInsideAtLaunchKey = new PropertyKey<bool>("WasInsideAtLaunch");

	private static readonly PropertyKey<BlockableObject> EnteredBlockableBuildingKey = new PropertyKey<BlockableObject>("EnteredBlockableBuilding");

	private static readonly ListKey<ContinuousEffect> EffectsKey = new ListKey<ContinuousEffect>("Effects");

	private readonly IDayNightCycle _dayNightCycle;

	private readonly ContinuousEffectValueSerializer _continuousEffectValueSerializer;

	private readonly ReferenceSerializer _referenceSerializer;

	private NeedManager _needManager;

	private Enterer _enterer;

	private CharacterAnimator _characterAnimator;

	private readonly List<ContinuousEffect> _effects = new List<ContinuousEffect>();

	private float _finishTimestamp;

	private string _animationName;

	private bool _wasInsideAtLaunch;

	private BlockableObject _enteredBlockableBuilding;

	public ApplyEffectExecutor(IDayNightCycle dayNightCycle, ContinuousEffectValueSerializer continuousEffectValueSerializer, ReferenceSerializer referenceSerializer)
	{
		_dayNightCycle = dayNightCycle;
		_continuousEffectValueSerializer = continuousEffectValueSerializer;
		_referenceSerializer = referenceSerializer;
	}

	public void Awake()
	{
		_needManager = GetComponent<NeedManager>();
		_enterer = GetComponent<Enterer>();
		_characterAnimator = GetComponent<CharacterAnimator>();
	}

	public void LaunchToTimestamp(IEnumerable<ContinuousEffect> effects, float timestamp, string animationName = null)
	{
		Launch(effects, timestamp, animationName);
	}

	public ExecutorStatus Tick(float deltaTimeInHours)
	{
		if (_wasInsideAtLaunch && (!_enterer.IsInside || !_enteredBlockableBuilding || !_enteredBlockableBuilding.IsUnblocked))
		{
			TurnOffAnimation();
			return ExecutorStatus.Failure;
		}
		if (_dayNightCycle.PartialDayNumber > _finishTimestamp)
		{
			TurnOffAnimation();
			return ExecutorStatus.Success;
		}
		_needManager.ApplyEffects(_effects, deltaTimeInHours);
		return ExecutorStatus.Running;
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(ApplyEffectExecutorKey);
		component.Set(EffectsKey, _effects, _continuousEffectValueSerializer);
		component.Set(FinishTimestampKey, _finishTimestamp);
		if (_animationName != null)
		{
			component.Set(AnimationNameKey, _animationName);
		}
		component.Set(WasInsideAtLaunchKey, _wasInsideAtLaunch);
		if ((bool)_enteredBlockableBuilding)
		{
			component.Set(EnteredBlockableBuildingKey, _enteredBlockableBuilding, _referenceSerializer.Of<BlockableObject>());
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(ApplyEffectExecutorKey);
		SetEffects(component.Get(EffectsKey, _continuousEffectValueSerializer));
		_finishTimestamp = component.Get(FinishTimestampKey);
		if (component.Has(AnimationNameKey))
		{
			TurnOnAnimation(component.Get(AnimationNameKey));
		}
		_wasInsideAtLaunch = component.Get(WasInsideAtLaunchKey);
		if (component.Has(EnteredBlockableBuildingKey))
		{
			component.GetObsoletable(EnteredBlockableBuildingKey, _referenceSerializer.Of<BlockableObject>(), out _enteredBlockableBuilding);
		}
	}

	private void Launch(IEnumerable<ContinuousEffect> effects, float finishTimestamp, string animationName = null)
	{
		_finishTimestamp = finishTimestamp;
		TurnOnAnimation(animationName);
		SetEffects(effects);
		_wasInsideAtLaunch = _enterer.IsInside;
		if (_wasInsideAtLaunch)
		{
			_enteredBlockableBuilding = _enterer.CurrentBuilding.GetComponent<BlockableObject>();
		}
	}

	private void SetEffects(IEnumerable<ContinuousEffect> effects)
	{
		_effects.Clear();
		_effects.AddRange(effects);
	}

	private void TurnOnAnimation(string animationName)
	{
		if (animationName != null)
		{
			_characterAnimator.SetBool(animationName, value: true);
			_animationName = animationName;
		}
	}

	private void TurnOffAnimation()
	{
		if (_animationName != null)
		{
			_characterAnimator.SetBool(_animationName, value: false);
			_animationName = null;
		}
	}
}
