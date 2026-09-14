using Timberborn.Fields;
using Timberborn.Localization;
using Timberborn.SliderToggleSystem;
using UnityEngine.UIElements;

namespace Timberborn.FieldsUI;

public class FarmHouseToggle
{
	private static readonly string HarvestingClass = "farmhouse-toggle__icon--harvesting";

	private static readonly string PlantingClass = "farmhouse-toggle__icon--planting";

	private static readonly string HarvestingLocKey = "Fields.Harvesting";

	private static readonly string PlantingLocKey = "Fields.Planting";

	private readonly SliderToggleFactory _sliderToggleFactory;

	private readonly ILoc _loc;

	private FarmHouse _farmHouse;

	private SliderToggle _sliderToggle;

	public FarmHouseToggle(SliderToggleFactory sliderToggleFactory, ILoc loc)
	{
		_sliderToggleFactory = sliderToggleFactory;
		_loc = loc;
	}

	public void Initialize(VisualElement parent)
	{
		SliderToggleItem sliderToggleItem = SliderToggleItem.Create(() => _loc.T(PlantingLocKey), PlantingClass, delegate
		{
			_farmHouse.PrioritizePlanting();
		}, () => _farmHouse.PlantingPrioritized);
		SliderToggleItem sliderToggleItem2 = SliderToggleItem.Create(() => _loc.T(HarvestingLocKey), HarvestingClass, delegate
		{
			_farmHouse.UnprioritizePlanting();
		}, () => !_farmHouse.PlantingPrioritized);
		_sliderToggle = _sliderToggleFactory.Create(parent, sliderToggleItem, sliderToggleItem2);
	}

	public void Show(FarmHouse farmHouse)
	{
		_farmHouse = farmHouse;
	}

	public void Update()
	{
		if ((bool)_farmHouse)
		{
			_sliderToggle.Update();
		}
	}

	public void Clear()
	{
		_farmHouse = null;
	}
}
