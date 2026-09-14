using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.GameDistricts;
using Timberborn.GameDistrictsMigration;
using UnityEngine.UIElements;

namespace Timberborn.GameDistrictsMigrationBatchControl;

public class DistrictMigrationSetterRowItemFactory
{
	private readonly ManualMigrationDistrictSetter _manualMigrationDistrictSetter;

	private readonly VisualElementLoader _visualElementLoader;

	public DistrictMigrationSetterRowItemFactory(ManualMigrationDistrictSetter manualMigrationDistrictSetter, VisualElementLoader visualElementLoader)
	{
		_manualMigrationDistrictSetter = manualMigrationDistrictSetter;
		_visualElementLoader = visualElementLoader;
	}

	public IBatchControlRowItem Create(DistrictCenter districtCenter)
	{
		string elementName = "Game/BatchControl/DistrictMigrationSetterRowItem";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		visualElement.Q<Button>("MigrateButtonLeft").RegisterCallback<ClickEvent>(delegate
		{
			_manualMigrationDistrictSetter.SetLeftDistrictWithHighlight(districtCenter);
		});
		visualElement.Q<Button>("MigrateButtonRight").RegisterCallback<ClickEvent>(delegate
		{
			_manualMigrationDistrictSetter.SetRightDistrictWithHighlight(districtCenter);
		});
		return new EmptyBatchControlRowItem(visualElement);
	}
}
