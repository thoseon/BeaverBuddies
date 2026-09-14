using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectModelSystem;
using Timberborn.EntitySystem;
using Timberborn.UnderstructureSystem;

namespace Timberborn.WaterSourceSystem;

public class UnderlyingWaterSource : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private UnderstructureConstraint _understructureConstraint;

	public WaterSource WaterSource { get; private set; }

	public void Awake()
	{
		_understructureConstraint = GetComponent<UnderstructureConstraint>();
	}

	public void InitializeEntity()
	{
		WaterSource = _understructureConstraint.UnderstructureEntity?.GetComponent<WaterSource>();
	}

	public void AddWaterStrengthModifier(IWaterStrengthModifier waterStrengthModifier)
	{
		if ((bool)WaterSource)
		{
			WaterSource.AddWaterStrengthModifier(waterStrengthModifier);
		}
	}

	public void RemoveWaterStrengthModifier(IWaterStrengthModifier waterStrengthModifier)
	{
		if ((bool)WaterSource)
		{
			WaterSource.RemoveWaterStrengthModifier(waterStrengthModifier);
		}
	}

	public void EnableDroughtInfluence()
	{
		if ((bool)WaterSource)
		{
			WaterSource.GetComponent<DroughtWaterStrengthModifier>().Enable();
			WaterSource.GetComponent<BlockObjectModel>().UnhideFullModelPermanently();
		}
	}

	public void DisableDroughtInfluence()
	{
		if ((bool)WaterSource)
		{
			WaterSource.GetComponent<DroughtWaterStrengthModifier>().Disable();
			WaterSource.GetComponent<BlockObjectModel>().HideFullModelPermanently();
		}
	}
}
