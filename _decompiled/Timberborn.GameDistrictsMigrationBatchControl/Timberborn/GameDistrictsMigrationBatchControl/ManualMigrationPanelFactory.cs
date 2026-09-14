using Timberborn.CoreUI;
using Timberborn.GameDistrictsMigration;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.GameDistrictsMigrationBatchControl;

internal class ManualMigrationPanelFactory
{
	private readonly EventBus _eventBus;

	private readonly ManualMigrationDistrictColumnFactory _manualMigrationDistrictColumnFactory;

	private readonly ManualMigrationDistrictSetter _manualMigrationDistrictSetter;

	private readonly VisualElementLoader _visualElementLoader;

	public ManualMigrationPanelFactory(EventBus eventBus, ManualMigrationDistrictColumnFactory manualMigrationDistrictColumnFactory, ManualMigrationDistrictSetter manualMigrationDistrictSetter, VisualElementLoader visualElementLoader)
	{
		_eventBus = eventBus;
		_manualMigrationDistrictColumnFactory = manualMigrationDistrictColumnFactory;
		_manualMigrationDistrictSetter = manualMigrationDistrictSetter;
		_visualElementLoader = visualElementLoader;
	}

	public ManualMigrationPanel Create()
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/BatchControl/ManualMigrationPanel");
		VisualElement parent = visualElement.Q<VisualElement>("LeftDistrictContent");
		ManualMigrationDistrictColumn manualMigrationDistrictColumnLeft = _manualMigrationDistrictColumnFactory.CreateLeftColumn(parent);
		VisualElement parent2 = visualElement.Q<VisualElement>("RightDistrictContent");
		ManualMigrationDistrictColumn manualMigrationDistrictColumnRight = _manualMigrationDistrictColumnFactory.CreateRightColumn(parent2);
		return new ManualMigrationPanel(_eventBus, _manualMigrationDistrictSetter, visualElement, manualMigrationDistrictColumnLeft, manualMigrationDistrictColumnRight);
	}
}
