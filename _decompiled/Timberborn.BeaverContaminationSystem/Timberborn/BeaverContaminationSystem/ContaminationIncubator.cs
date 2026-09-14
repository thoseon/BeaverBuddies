using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.BeaverContaminationSystem;

public class ContaminationIncubator : BaseComponent, IAwakableComponent, IPersistentEntity, IPostInitializableEntity, IChildhoodInfluenced, IDeletableEntity
{
	private static readonly ComponentKey ContaminationIncubatorKey = new ComponentKey("ContaminationIncubator");

	private static readonly PropertyKey<float> IncubationProgressKey = new PropertyKey<float>("IncubationProgress");

	private static readonly float IncubationTimeInDays = 3f;

	private readonly ITimeTriggerFactory _timeTriggerFactory;

	private Contaminable _contaminable;

	private ITimeTrigger _timeTrigger;

	public bool IsIncubating => _timeTrigger.InProgress;

	public bool IncubationFinished => _timeTrigger.Finished;

	private float IncubationProgress => _timeTrigger.Progress;

	public event EventHandler IncubationStateChanged;

	public ContaminationIncubator(ITimeTriggerFactory timeTriggerFactory)
	{
		_timeTriggerFactory = timeTriggerFactory;
	}

	public void Awake()
	{
		_contaminable = GetComponent<Contaminable>();
		_timeTrigger = _timeTriggerFactory.Create(FinishIncubation, IncubationTimeInDays);
	}

	public void Save(IEntitySaver entitySaver)
	{
		if (_timeTrigger.Progress != 0f)
		{
			entitySaver.GetComponent(ContaminationIncubatorKey).Set(IncubationProgressKey, _timeTrigger.Progress);
		}
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(ContaminationIncubatorKey, out var objectLoader))
		{
			float incubationProgress = objectLoader.Get(IncubationProgressKey);
			FastForwardIncubation(incubationProgress);
		}
	}

	public void PostInitializeEntity()
	{
		NotifyContaminationIncubationChanged();
	}

	public void InfluenceByChildhood(Character child)
	{
		FastForwardIncubation(child.GetComponent<ContaminationIncubator>().IncubationProgress);
		NotifyContaminationIncubationChanged();
	}

	public void DeleteEntity()
	{
		_timeTrigger.Pause();
	}

	public void StartIncubation()
	{
		if (!_contaminable.IsContaminated && !IsIncubating)
		{
			_timeTrigger.Resume();
			NotifyContaminationIncubationChanged();
		}
	}

	public void ResetIncubation()
	{
		_timeTrigger.Reset();
		NotifyContaminationIncubationChanged();
	}

	private void FinishIncubation()
	{
		NotifyContaminationIncubationChanged();
	}

	private void NotifyContaminationIncubationChanged()
	{
		IncubationStateChanged?.Invoke(this, EventArgs.Empty);
	}

	private void FastForwardIncubation(float incubationProgress)
	{
		if (incubationProgress > 0f)
		{
			_timeTrigger.FastForwardProgress(incubationProgress);
			_timeTrigger.Resume();
		}
	}
}
