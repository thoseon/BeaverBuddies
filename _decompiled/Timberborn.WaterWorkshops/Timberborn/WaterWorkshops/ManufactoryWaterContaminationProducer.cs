using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.WaterBuildings;
using Timberborn.Workshops;

namespace Timberborn.WaterWorkshops;

internal class ManufactoryWaterContaminationProducer : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private static readonly string WaterContaminationId = "FlowingBadwater";

	private Manufactory _manufactory;

	private WaterOutput _waterOutput;

	private float _producedWaterContamination;

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
		_producedWaterContamination = 0f;
		RecipeSpec currentRecipe = _manufactory.CurrentRecipe;
		if (currentRecipe != null && currentRecipe.Id == WaterContaminationId)
		{
			_manufactory.ProductionProgressed += OnProductionProgressed;
			_producedWaterContamination = WaterContaminationGoodToWaterContaminationAmountConverter.GetWaterContaminationAmount(currentRecipe.Ingredients);
		}
	}

	private void OnProductionProgressed(object sender, ProductionProgressedEventArgs e)
	{
		_waterOutput.AddContaminatedWater(_producedWaterContamination * e.ProductionProgressChange);
	}
}
