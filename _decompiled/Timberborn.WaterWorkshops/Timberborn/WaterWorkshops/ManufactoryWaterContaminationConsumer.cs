using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.WaterBuildings;
using Timberborn.Workshops;

namespace Timberborn.WaterWorkshops;

internal class ManufactoryWaterContaminationConsumer : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private Manufactory _manufactory;

	private WaterInput _waterInput;

	public float ConsumedWaterContamination { get; private set; }

	public void Awake()
	{
		_manufactory = GetComponent<Manufactory>();
		_waterInput = GetComponent<WaterInput>();
	}

	public void InitializeEntity()
	{
		UpdateRecipe();
		_manufactory.RecipeChanged += delegate
		{
			UpdateRecipe();
		};
		_manufactory.ProductionProgressed += OnProductionProgressed;
	}

	private void UpdateRecipe()
	{
		ConsumedWaterContamination = WaterContaminationGoodToWaterContaminationAmountConverter.GetWaterContaminationAmount(_manufactory.CurrentRecipe.Products);
	}

	private void OnProductionProgressed(object sender, ProductionProgressedEventArgs e)
	{
		if (ConsumedWaterContamination > 0f)
		{
			_waterInput.RemoveContaminatedWater(ConsumedWaterContamination * e.ProductionProgressChange);
		}
	}
}
