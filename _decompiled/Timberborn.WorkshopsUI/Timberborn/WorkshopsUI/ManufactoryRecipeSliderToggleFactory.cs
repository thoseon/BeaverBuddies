using System.Collections.Generic;
using System.Linq;
using Timberborn.Localization;
using Timberborn.SliderToggleSystem;
using Timberborn.Workshops;
using UnityEngine.UIElements;

namespace Timberborn.WorkshopsUI;

public class ManufactoryRecipeSliderToggleFactory
{
	private readonly SliderToggleFactory _sliderToggleFactory;

	private readonly ILoc _loc;

	public ManufactoryRecipeSliderToggleFactory(SliderToggleFactory sliderToggleFactory, ILoc loc)
	{
		_sliderToggleFactory = sliderToggleFactory;
		_loc = loc;
	}

	public SliderToggle Create(VisualElement parent, Manufactory manufactory)
	{
		return _sliderToggleFactory.Create(parent, CreateItems(manufactory).ToArray());
	}

	private IEnumerable<SliderToggleItem> CreateItems(Manufactory manufactory)
	{
		foreach (RecipeSpec productionRecipe in manufactory.ProductionRecipes)
		{
			yield return SliderToggleItem.Create(() => _loc.T(productionRecipe.DisplayLocKey), productionRecipe.UIIcon.Value, delegate
			{
				manufactory.SetRecipe(productionRecipe);
			}, () => manufactory.CurrentRecipe == productionRecipe);
		}
	}
}
