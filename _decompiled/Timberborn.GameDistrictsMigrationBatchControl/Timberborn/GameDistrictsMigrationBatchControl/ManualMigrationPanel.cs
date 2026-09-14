using Timberborn.CoreUI;
using Timberborn.EntityNaming;
using Timberborn.GameDistricts;
using Timberborn.GameDistrictsMigration;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.GameDistrictsMigrationBatchControl;

internal class ManualMigrationPanel
{
	private readonly EventBus _eventBus;

	private readonly ManualMigrationDistrictSetter _manualMigrationDistrictSetter;

	private readonly ManualMigrationDistrictColumn _manualMigrationDistrictColumnLeft;

	private readonly ManualMigrationDistrictColumn _manualMigrationDistrictColumnRight;

	public VisualElement Root { get; }

	public ManualMigrationPanel(EventBus eventBus, ManualMigrationDistrictSetter manualMigrationDistrictSetter, VisualElement root, ManualMigrationDistrictColumn manualMigrationDistrictColumnLeft, ManualMigrationDistrictColumn manualMigrationDistrictColumnRight)
	{
		_eventBus = eventBus;
		_manualMigrationDistrictSetter = manualMigrationDistrictSetter;
		Root = root;
		_manualMigrationDistrictColumnLeft = manualMigrationDistrictColumnLeft;
		_manualMigrationDistrictColumnRight = manualMigrationDistrictColumnRight;
	}

	public void Show()
	{
		_manualMigrationDistrictSetter.DifferentiateDistricts();
		SetDistricts();
		_eventBus.Register(this);
		_manualMigrationDistrictColumnLeft.Show();
		_manualMigrationDistrictColumnRight.Show();
		_eventBus.Post(new ManualMigrationPanelOpenedEvent());
	}

	public void Update()
	{
		if (_manualMigrationDistrictSetter.AreDistrictsSet)
		{
			_manualMigrationDistrictColumnLeft.Update();
			_manualMigrationDistrictColumnRight.Update();
			Root.ToggleDisplayStyle(visible: true);
		}
		else
		{
			Root.ToggleDisplayStyle(visible: false);
		}
	}

	public void Hide()
	{
		_eventBus.Post(new ManualMigrationPanelClosedEvent());
		_eventBus.Unregister(this);
		_manualMigrationDistrictSetter.ResetRightDistrictChangeCheck();
	}

	[OnEvent]
	public void OnMigrationDistrictChangedEvent(MigrationDistrictChangedEvent migrationDistrictChangedEvent)
	{
		SetDistricts();
		if (migrationDistrictChangedEvent.HighlightLeftDistrict)
		{
			_manualMigrationDistrictColumnLeft.Highlight();
		}
		if (migrationDistrictChangedEvent.HighlightRightDistrict)
		{
			_manualMigrationDistrictColumnRight.Highlight();
		}
	}

	[OnEvent]
	public void OnEntityNameChanged(EntityNameChangedEvent entityNameChangedEvent)
	{
		if ((bool)entityNameChangedEvent.Entity.GetComponent<DistrictCenter>())
		{
			SetDistricts();
		}
	}

	private void SetDistricts()
	{
		if (_manualMigrationDistrictSetter.AreDistrictsSet)
		{
			DistrictCenter leftDistrict = _manualMigrationDistrictSetter.LeftDistrict;
			DistrictCenter rightDistrict = _manualMigrationDistrictSetter.RightDistrict;
			_manualMigrationDistrictColumnLeft.SetDistricts(leftDistrict, rightDistrict);
			_manualMigrationDistrictColumnRight.SetDistricts(rightDistrict, leftDistrict);
		}
	}
}
