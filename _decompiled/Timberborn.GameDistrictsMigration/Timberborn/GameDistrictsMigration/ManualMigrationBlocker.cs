using Timberborn.GameDistricts;
using Timberborn.Localization;
using Timberborn.SingletonSystem;

namespace Timberborn.GameDistrictsMigration;

public class ManualMigrationBlocker : ILoadableSingleton
{
	private static readonly string SameDistrictLocKey = "Migration.SameDistrict";

	private static readonly string NotConnectedLocKey = "Migration.DistrictsNotConnected";

	private readonly DistrictConnections _districtConnections;

	private readonly EventBus _eventBus;

	private readonly ILoc _loc;

	private readonly ManualMigrationDistrictSetter _manualMigrationDistrictSetter;

	public string TooltipText { get; private set; }

	public bool IsEnabled => string.IsNullOrEmpty(TooltipText);

	public ManualMigrationBlocker(DistrictConnections districtConnections, EventBus eventBus, ILoc loc, ManualMigrationDistrictSetter manualMigrationDistrictSetter)
	{
		_districtConnections = districtConnections;
		_eventBus = eventBus;
		_loc = loc;
		_manualMigrationDistrictSetter = manualMigrationDistrictSetter;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnDistrictConnectionsChanged(DistrictConnectionsChangedEvent districtConnectionsChangedEvent)
	{
		SetCurrentState();
	}

	[OnEvent]
	public void OnMigrationDistrictChangedEvent(MigrationDistrictChangedEvent migrationDistrictChangedEvent)
	{
		SetCurrentState();
	}

	private void SetCurrentState()
	{
		TooltipText = GetCurrentState();
		_eventBus.Post(new ManualMigrationBlockingStateChangedEvent(IsEnabled));
	}

	private string GetCurrentState()
	{
		if (_manualMigrationDistrictSetter.AreDistrictsSet)
		{
			if (_manualMigrationDistrictSetter.LeftDistrict == _manualMigrationDistrictSetter.RightDistrict)
			{
				return _loc.T(SameDistrictLocKey);
			}
			if (!_districtConnections.AreDistrictsConnected(_manualMigrationDistrictSetter.LeftDistrict, _manualMigrationDistrictSetter.RightDistrict))
			{
				return _loc.T(NotConnectedLocKey);
			}
		}
		return string.Empty;
	}
}
