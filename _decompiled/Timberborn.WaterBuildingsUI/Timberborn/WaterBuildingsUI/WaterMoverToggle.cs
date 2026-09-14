using Timberborn.Goods;
using Timberborn.Localization;
using Timberborn.SliderToggleSystem;
using Timberborn.WaterBuildings;
using UnityEngine.UIElements;

namespace Timberborn.WaterBuildingsUI;

internal class WaterMoverToggle
{
	private static readonly string UnfilteredWaterLocKey = "WaterMover.Unfiltered";

	private static readonly string UnfilteredWaterClass = "water-mover-toggle__icon--unfiltered";

	private static readonly string CleanWaterGoodId = "Water";

	private static readonly string ContaminatedWaterGoodId = "Badwater";

	private readonly SliderToggleFactory _sliderToggleFactory;

	private readonly IGoodService _goodService;

	private readonly ILoc _loc;

	private WaterMover _waterMover;

	private SliderToggle _sliderToggle;

	public WaterMoverToggle(SliderToggleFactory sliderToggleFactory, IGoodService goodService, ILoc loc)
	{
		_sliderToggleFactory = sliderToggleFactory;
		_goodService = goodService;
		_loc = loc;
	}

	public void Initialize(VisualElement parent)
	{
		GoodSpec cleanWater = _goodService.GetGood(CleanWaterGoodId);
		GoodSpec contaminatedWater = _goodService.GetGood(ContaminatedWaterGoodId);
		SliderToggleItem sliderToggleItem = SliderToggleItem.Create(() => _loc.T(UnfilteredWaterLocKey), UnfilteredWaterClass, delegate
		{
			SetWaterMovement(moveCleanWater: true, moveContaminatedWater: true);
		}, () => _waterMover.CleanWaterMovement && _waterMover.ContaminatedWaterMovement);
		SliderToggleItem sliderToggleItem2 = SliderToggleItem.Create(() => cleanWater.DisplayName.Value, cleanWater.IconSmall.Value, delegate
		{
			SetWaterMovement(moveCleanWater: true, moveContaminatedWater: false);
		}, () => _waterMover.CleanWaterMovement && !_waterMover.ContaminatedWaterMovement);
		SliderToggleItem sliderToggleItem3 = SliderToggleItem.Create(() => contaminatedWater.DisplayName.Value, contaminatedWater.IconSmall.Value, delegate
		{
			SetWaterMovement(moveCleanWater: false, moveContaminatedWater: true);
		}, () => !_waterMover.CleanWaterMovement && _waterMover.ContaminatedWaterMovement);
		_sliderToggle = _sliderToggleFactory.Create(parent, sliderToggleItem, sliderToggleItem2, sliderToggleItem3);
	}

	public void Show(WaterMover waterMover)
	{
		_waterMover = waterMover;
	}

	public void Update()
	{
		if ((bool)_waterMover)
		{
			_sliderToggle.Update();
		}
	}

	public void Clear()
	{
		_waterMover = null;
	}

	private void SetWaterMovement(bool moveCleanWater, bool moveContaminatedWater)
	{
		_waterMover.CleanWaterMovement = moveCleanWater;
		_waterMover.ContaminatedWaterMovement = moveContaminatedWater;
	}
}
