using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EnterableSystem;
using Timberborn.Persistence;
using Timberborn.TickSystem;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Attractions;

public class AttractionLoadRate : TickableComponent, IAwakableComponent, IFinishedStateListener, IPersistentEntity
{
	private static readonly ComponentKey AttractionLoadRateKey = new ComponentKey("AttractionLoadRate");

	private static readonly ListKey<int> MaxLoadKey = new ListKey<int>("MaxLoad");

	private static readonly ListKey<int> ActualLoadKey = new ListKey<int>("ActualLoad");

	private readonly IDayNightCycle _dayNightCycle;

	private int[] _maxLoad = new int[24];

	private int[] _actualLoad = new int[24];

	private int _currentHour;

	private Enterable _enterable;

	public AttractionLoadRate(IDayNightCycle dayNightCycle)
	{
		_dayNightCycle = dayNightCycle;
	}

	public void Awake()
	{
		_enterable = GetComponent<Enterable>();
		DisableComponent();
	}

	public override void Tick()
	{
		ResetValuesOnHourChange();
		CollectSample();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	public float GetLoadRate(int hour)
	{
		return (float)_actualLoad[hour] / (float)_maxLoad[hour];
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(AttractionLoadRateKey);
		component.Set(MaxLoadKey, _maxLoad);
		component.Set(ActualLoadKey, _actualLoad);
	}

	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(AttractionLoadRateKey, out var objectLoader))
		{
			_maxLoad = objectLoader.Get(MaxLoadKey).ToArray();
			_actualLoad = objectLoader.Get(ActualLoadKey).ToArray();
		}
	}

	private void ResetValuesOnHourChange()
	{
		int num = (int)_dayNightCycle.HoursPassedToday;
		if (num != _currentHour)
		{
			_currentHour = num;
			_maxLoad[_currentHour] = 0;
			_actualLoad[_currentHour] = 0;
		}
	}

	private void CollectSample()
	{
		_maxLoad[_currentHour] += _enterable.Capacity;
		_actualLoad[_currentHour] += _enterable.NumberOfEnterersInside;
	}
}
