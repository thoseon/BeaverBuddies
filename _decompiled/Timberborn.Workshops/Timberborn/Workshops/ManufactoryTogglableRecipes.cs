using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;

namespace Timberborn.Workshops;

public class ManufactoryTogglableRecipes : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private ManufactoryTogglableRecipesSpec _manufactoryTogglableRecipesSpec;

	public string LabelLocKey => _manufactoryTogglableRecipesSpec.LabelLocKey;

	public void Awake()
	{
		_manufactoryTogglableRecipesSpec = GetComponent<ManufactoryTogglableRecipesSpec>();
	}

	public void InitializeEntity()
	{
		Manufactory component = GetComponent<Manufactory>();
		if (component.CurrentRecipe == null)
		{
			component.SetRecipe(component.ProductionRecipes[0]);
		}
	}
}
