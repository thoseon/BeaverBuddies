using System;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Wonders;

internal class WonderDeactivationTimer : BaseComponent, IAwakableComponent, IPersistentEntity, IDeletableEntity
{
	private static readonly ComponentKey WonderDeactivationTimerKey = new ComponentKey("WonderDeactivationTimer");

	private static readonly PropertyKey<float> DelayProgressKey = new PropertyKey<float>("DelayProgress");

	private readonly ITimeTriggerFactory _timeTriggerFactory;

	private Wonder _wonder;

	private ITimeTrigger _timeTrigger;

	public WonderDeactivationTimer(ITimeTriggerFactory timeTriggerFactory)
	{
		_timeTriggerFactory = timeTriggerFactory;
	}

	public void Awake()
	{
		_wonder = GetComponent<Wonder>();
		_wonder.WonderActivated += OnWonderActivated;
		float timerDelayInHours = GetComponent<WonderDeactivationTimerSpec>().TimerDelayInHours;
		_timeTrigger = _timeTriggerFactory.Create(DeactivateWonder, timerDelayInHours / 24f);
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (_timeTrigger.InProgress)
		{
			entitySaver.GetComponent(WonderDeactivationTimerKey).Set(DelayProgressKey, _timeTrigger.Progress);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(WonderDeactivationTimerKey, out var objectLoader))
		{
			float progress = objectLoader.Get(DelayProgressKey);
			_timeTrigger.FastForwardProgress(progress);
			_timeTrigger.Resume();
		}
	}

	public void DeleteEntity()
	{
		_timeTrigger.Reset();
	}

	private void OnWonderActivated(object sender, EventArgs e)
	{
		_timeTrigger.Reset();
		_timeTrigger.Resume();
	}

	private void DeactivateWonder()
	{
		_wonder.Deactivate();
	}
}
