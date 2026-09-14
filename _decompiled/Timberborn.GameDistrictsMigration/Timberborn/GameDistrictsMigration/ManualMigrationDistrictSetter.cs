using Timberborn.Common;
using Timberborn.GameDistricts;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.GameDistrictsMigration;

public class ManualMigrationDistrictSetter : ILoadableSingleton, ISaveableSingleton, IPostLoadableSingleton
{
	private static readonly SingletonKey ManualMigrationDistrictSetterKey = new SingletonKey("ManualMigrationDistrictSetter");

	private static readonly PropertyKey<int> LeftDistrictLastIndexKey = new PropertyKey<int>("LeftDistrictLastIndex");

	private static readonly PropertyKey<int> RightDistrictLastIndexKey = new PropertyKey<int>("RightDistrictLastIndex");

	private readonly DistrictCenterRegistry _districtCenterRegistry;

	private readonly DistrictConnections _districtConnections;

	private readonly EventBus _eventBus;

	private readonly ISingletonLoader _singletonLoader;

	private int _leftDistrictLastIndex = -1;

	private int _rightDistrictLastIndex = -1;

	private bool _wasRightDistrictChanged;

	public DistrictCenter LeftDistrict { get; private set; }

	public DistrictCenter RightDistrict { get; private set; }

	public bool AreDistrictsSet
	{
		get
		{
			if (IsLeftDistrictSet)
			{
				return IsRightDistrictSet;
			}
			return false;
		}
	}

	private ReadOnlyList<DistrictCenter> DistrictCenters => _districtCenterRegistry.FinishedDistrictCenters;

	private bool IsLeftDistrictSet
	{
		get
		{
			if ((bool)LeftDistrict)
			{
				return DistrictCenters.Contains(LeftDistrict);
			}
			return false;
		}
	}

	private bool IsRightDistrictSet
	{
		get
		{
			if ((bool)RightDistrict)
			{
				return DistrictCenters.Contains(RightDistrict);
			}
			return false;
		}
	}

	public ManualMigrationDistrictSetter(DistrictCenterRegistry districtCenterRegistry, DistrictConnections districtConnections, EventBus eventBus, ISingletonLoader singletonLoader)
	{
		_districtCenterRegistry = districtCenterRegistry;
		_districtConnections = districtConnections;
		_eventBus = eventBus;
		_singletonLoader = singletonLoader;
	}

	public void Save(ISingletonSaver singletonSaver)
	{
		IObjectSaver singleton = singletonSaver.GetSingleton(ManualMigrationDistrictSetterKey);
		singleton.Set(LeftDistrictLastIndexKey, DistrictCenters.IndexOf(LeftDistrict));
		singleton.Set(RightDistrictLastIndexKey, DistrictCenters.IndexOf(RightDistrict));
	}

	public void Load()
	{
		_eventBus.Register(this);
		if (_singletonLoader.TryGetSingleton(ManualMigrationDistrictSetterKey, out var objectLoader) && objectLoader.Has(LeftDistrictLastIndexKey))
		{
			_leftDistrictLastIndex = objectLoader.Get(LeftDistrictLastIndexKey);
			_rightDistrictLastIndex = objectLoader.Get(RightDistrictLastIndexKey);
		}
	}

	public void PostLoad()
	{
		if (_leftDistrictLastIndex >= 0 && _leftDistrictLastIndex < DistrictCenters.Count)
		{
			LeftDistrict = DistrictCenters[_leftDistrictLastIndex];
		}
		if (_rightDistrictLastIndex >= 0 && _rightDistrictLastIndex < DistrictCenters.Count)
		{
			RightDistrict = DistrictCenters[_rightDistrictLastIndex];
		}
	}

	public void SetLeftDistrict(DistrictCenter districtCenter)
	{
		SetLeftDistrict(districtCenter, MigrationDistrictChangedEvent.Create());
	}

	public void SetRightDistrict(DistrictCenter districtCenter)
	{
		SetRightDistrict(districtCenter, MigrationDistrictChangedEvent.Create());
	}

	public void SetLeftDistrictWithHighlight(DistrictCenter districtCenter)
	{
		SetLeftDistrict(districtCenter, MigrationDistrictChangedEvent.CreateWithLeftHighlight());
	}

	public void SetRightDistrictWithHighlight(DistrictCenter districtCenter)
	{
		SetRightDistrict(districtCenter, MigrationDistrictChangedEvent.CreateWithRightHighlight());
	}

	public void ResetRightDistrictChangeCheck()
	{
		_wasRightDistrictChanged = false;
	}

	public void DifferentiateDistricts()
	{
		if (LeftDistrict == RightDistrict)
		{
			if (_wasRightDistrictChanged)
			{
				LeftDistrict = null;
			}
			else
			{
				RightDistrict = null;
			}
			CheckDistrictsAndPostEvent(MigrationDistrictChangedEvent.Create());
		}
	}

	[OnEvent]
	public void OnDistrictCenterRegistryChanged(DistrictCenterRegistryChangedEvent districtCenterRegistryChangedEvent)
	{
		if (!DistrictCenters.Contains(LeftDistrict))
		{
			LeftDistrict = null;
		}
		if (!DistrictCenters.Contains(RightDistrict))
		{
			RightDistrict = null;
		}
		CheckDistrictsAndPostEvent(MigrationDistrictChangedEvent.Create());
	}

	private void SetLeftDistrict(DistrictCenter districtCenter, MigrationDistrictChangedEvent migrationDistrictChangedEvent)
	{
		LeftDistrict = districtCenter;
		_wasRightDistrictChanged = false;
		CheckDistrictsAndPostEvent(migrationDistrictChangedEvent);
	}

	private void SetRightDistrict(DistrictCenter districtCenter, MigrationDistrictChangedEvent migrationDistrictChangedEvent)
	{
		RightDistrict = districtCenter;
		_wasRightDistrictChanged = true;
		CheckDistrictsAndPostEvent(migrationDistrictChangedEvent);
	}

	private void CheckDistrictsAndPostEvent(MigrationDistrictChangedEvent migrationDistrictChangedEvent)
	{
		UpdateDistricts();
		_eventBus.Post(migrationDistrictChangedEvent);
	}

	private void UpdateDistricts()
	{
		if (DistrictCenters.Count == 0)
		{
			LeftDistrict = null;
			RightDistrict = null;
		}
		else if (!RightDistrict)
		{
			if (!LeftDistrict)
			{
				LeftDistrict = DistrictCenters[0];
			}
			RightDistrict = _districtConnections.GetFirstConnectedOrAny(LeftDistrict);
		}
		else if (!LeftDistrict)
		{
			LeftDistrict = _districtConnections.GetFirstConnectedOrAny(RightDistrict);
		}
	}
}
