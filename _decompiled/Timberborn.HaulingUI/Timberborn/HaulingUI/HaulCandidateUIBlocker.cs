using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.Workshops;

namespace Timberborn.HaulingUI;

internal class HaulCandidateUIBlocker : BaseComponent, IAwakableComponent
{
	private Manufactory _manufactory;

	public void Awake()
	{
		_manufactory = GetComponent<Manufactory>();
	}

	public bool ShouldShowUI()
	{
		if ((bool)_manufactory)
		{
			return !_manufactory.ProductionRecipes.FastAll((RecipeSpec recipe) => !recipe.ConsumesIngredients && !recipe.ProducesProducts);
		}
		return true;
	}
}
