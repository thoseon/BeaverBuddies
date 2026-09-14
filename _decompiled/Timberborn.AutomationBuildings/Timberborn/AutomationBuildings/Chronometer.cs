using System;
using Timberborn.Automation;
using Timberborn.BaseComponentSystem;
using Timberborn.DuplicationSystem;
using Timberborn.Persistence;
using Timberborn.TimeSystem;
using Timberborn.WorkSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.AutomationBuildings;

public class Chronometer : BaseComponent, IAwakableComponent, IPersistentEntity, IDuplicable<Chronometer>, IDuplicable, ISamplingTransmitter, ITransmitter
{
	private static readonly ComponentKey ChronometerKey = new ComponentKey("Chronometer");

	private static readonly PropertyKey<float> StartTimeKey = new PropertyKey<float>("StartTime");

	private static readonly PropertyKey<float> EndTimeKey = new PropertyKey<float>("EndTime");

	private static readonly PropertyKey<ChronometerMode> ModeKey = new PropertyKey<ChronometerMode>("Mode");

	private readonly IDayNightCycle _dayNightCycle;

	private readonly WorkingHoursManager _workingHoursManager;

	private Automator _automator;

	private float _sampledWorkEndHours;

	public float StartTime { get; private set; }

	public float EndTime { get; private set; } = 16f;

	public ChronometerMode Mode { get; private set; }

	public float SampledTime { get; private set; }

	public Chronometer(IDayNightCycle dayNightCycle, WorkingHoursManager workingHoursManager)
	{
		_dayNightCycle = dayNightCycle;
		_workingHoursManager = workingHoursManager;
	}

	public void Awake()
	{
		_automator = GetComponent<Automator>();
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(ChronometerKey);
		component.Set(StartTimeKey, StartTime);
		component.Set(EndTimeKey, EndTime);
		component.Set(ModeKey, Mode);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(ChronometerKey);
		StartTime = component.Get(StartTimeKey);
		EndTime = component.Get(EndTimeKey);
		Mode = component.Get(ModeKey);
	}

	public void DuplicateFrom(Chronometer source)
	{
		StartTime = source.StartTime;
		EndTime = source.EndTime;
		Mode = source.Mode;
		UpdateOutputState();
	}

	public void SetStartTime(float startTime)
	{
		StartTime = startTime;
		UpdateOutputState();
	}

	public void SetEndTime(float endTime)
	{
		EndTime = endTime;
		UpdateOutputState();
	}

	public void SetMode(ChronometerMode mode)
	{
		Mode = mode;
		UpdateOutputState();
	}

	public void Sample()
	{
		SampledTime = _dayNightCycle.HoursPassedToday;
		_sampledWorkEndHours = _workingHoursManager.EndHours;
		UpdateOutputState();
	}

	private void UpdateOutputState()
	{
		var (startTime, endTime) = GetStartAndEndTime();
		_automator.SetState(IsOn(startTime, endTime));
	}

	private (float, float) GetStartAndEndTime()
	{
		return Mode switch
		{
			ChronometerMode.TimeRange => (StartTime, EndTime), 
			ChronometerMode.WorkingHours => (0f, _sampledWorkEndHours), 
			ChronometerMode.NonWorkingHours => (_sampledWorkEndHours, 24f), 
			_ => throw new ArgumentOutOfRangeException("ChronometerMode", Mode, null), 
		};
	}

	private bool IsOn(float startTime, float endTime)
	{
		if (!(startTime <= endTime))
		{
			if (!(SampledTime >= startTime))
			{
				return SampledTime < endTime;
			}
			return true;
		}
		if (SampledTime >= startTime)
		{
			return SampledTime < endTime;
		}
		return false;
	}
}
