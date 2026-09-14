using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.WaterBuildings;
using Timberborn.Workshops;

namespace Timberborn.WaterWorkshops;

internal class ManufactoryWaterProducer : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private static readonly string WaterId = "FlowingWater";

	private Manufactory _manufactory;

	private WaterOutput _waterOutput;

	private float _producedWater;

	public void Awake()
	{
		_manufactory = GetComponent<Manufactory>();
		_waterOutput = GetComponent<WaterOutput>();
	}

	public void InitializeEntity()
	{
		UpdateRecipe();
		_manufactory.RecipeChanged += delegate
		{
			UpdateRecipe();
		};
	}

	private void UpdateRecipe()
	{
		_manufactory.ProductionProgressed -= OnProductionProgressed;
		_producedWater = 0f;
		RecipeSpec currentRecipe = _manufactory.CurrentRecipe;
		if (currentRecipe != null && currentRecipe.Id == WaterId)
		{
			_manufactory.ProductionProgressed += OnProductionProgressed;
			_producedWater = WaterGoodToWaterAmountConverter.GetWaterAmount(currentRecipe.Ingredients);
		}
	}

	private void OnProductionProgressed(object sender, ProductionProgressedEventArgs e)
	{
		_waterOutput.AddCleanWater(_producedWater * e.ProductionProgressChange);
	}
}
