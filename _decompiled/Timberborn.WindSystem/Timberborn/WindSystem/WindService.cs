using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.WindSystem;

public class WindService : ITickableSingleton, IUpdatableSingleton, ISaveableSingleton, ILoadableSingleton
{
	private static readonly int WindStrengthProperty = Shader.PropertyToID("_WindStrength");

	private static readonly SingletonKey WindServiceKey = new SingletonKey("WindService");

	private static readonly PropertyKey<float> WindStrengthKey = new PropertyKey<float>("WindStrength");

	private static readonly PropertyKey<Vector2> WindDirectionKey = new PropertyKey<Vector2>("WindDirection");

	private static readonly PropertyKey<float> NextWindChangeTimeKey = new PropertyKey<float>("NextWindChangeTime");

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly ISingletonLoader _singletonLoader;

	private readonly IDayNightCycle _dayNightCycle;

	private readonly EventBus _eventBus;

	private readonly ISpecService _specService;

	private WindServiceSpec _windServiceSpec;

	private float _nextWindChangeTime;

	private float _shaderWindStrength;

	public float WindStrength { get; private set; }

	public Vector2 WindDirection { get; private set; }

	public bool IsForcedWind { get; private set; }

	public WindService(ISingletonLoader singletonLoader, IRandomNumberGenerator randomNumberGenerator, IDayNightCycle dayNightCycle, EventBus eventBus, ISpecService specService)
	{
		_singletonLoader = singletonLoader;
		_randomNumberGenerator = randomNumberGenerator;
		_dayNightCycle = dayNightCycle;
		_eventBus = eventBus;
		_specService = specService;
	}

	public void Load()
	{
		_windServiceSpec = _specService.GetSingleSpec<WindServiceSpec>();
		if (_singletonLoader.TryGetSingleton(WindServiceKey, out var objectLoader))
		{
			WindStrength = objectLoader.Get(WindStrengthKey);
			WindDirection = objectLoader.Get(WindDirectionKey);
			_nextWindChangeTime = objectLoader.Get(NextWindChangeTimeKey);
		}
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		IObjectSaver singleton = singletonSaver.GetSingleton(WindServiceKey);
		singleton.Set(WindStrengthKey, WindStrength);
		singleton.Set(WindDirectionKey, WindDirection);
		singleton.Set(NextWindChangeTimeKey, _nextWindChangeTime);
	}

	public void UpdateSingleton()
	{
		UpdateShaderProperties();
	}

	public void Tick()
	{
		if (!IsForcedWind && _dayNightCycle.PartialDayNumber >= _nextWindChangeTime)
		{
			ChangeWind();
		}
	}

	public void ToggleForcedWind()
	{
		IsForcedWind = !IsForcedWind;
		ChangeWind();
	}

	private void ChangeWind()
	{
		_shaderWindStrength = WindStrength;
		float hours = _randomNumberGenerator.Range(_windServiceSpec.MinWindTimeInHours, _windServiceSpec.MaxWindTimeInHours);
		_nextWindChangeTime = _dayNightCycle.DayNumberHoursFromNow(hours);
		WindDirection = _randomNumberGenerator.InsideUnitCircle();
		WindStrength = _randomNumberGenerator.Range(_windServiceSpec.MinWindStrength, _windServiceSpec.MaxWindStrength);
		WindDirection.Normalize();
		UpdateShaderProperties();
		_eventBus.Post(new WindChangedEvent());
	}

	private void UpdateShaderProperties()
	{
		if (!Mathf.Approximately(_shaderWindStrength, WindStrength))
		{
			_shaderWindStrength = Mathf.MoveTowards(_shaderWindStrength, WindStrength, Time.deltaTime);
			Shader.SetGlobalFloat(WindStrengthProperty, _shaderWindStrength);
		}
	}
}
