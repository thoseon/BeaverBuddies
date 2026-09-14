using Timberborn.BaseComponentSystem;
using Timberborn.Localization;
using Timberborn.NaturalResourcesContamination;
using Timberborn.NaturalResourcesMoisture;

namespace Timberborn.NaturalResourcesLifecycleUI;

internal class DeadNaturalResourceDescriber
{
	private static readonly string GenericDiedLocKey = "NaturalResources.GenericDied";

	private static readonly string DriedLocKey = "NaturalResources.Dried";

	private static readonly string OverwateredLocKey = "NaturalResources.Overwatered";

	private static readonly string DiedFromNotEnoughWaterLocKey = "NaturalResources.DiedFromNotEnoughWater";

	private static readonly string DiedFromTooMuchWaterLocKey = "NaturalResources.DiedFromTooMuchWater";

	private static readonly string DiedFromContaminationLocKey = "NaturalResources.DiedFromContamination";

	private readonly ILoc _loc;

	public DeadNaturalResourceDescriber(ILoc loc)
	{
		_loc = loc;
	}

	public string Describe(BaseComponent entity)
	{
		WateredNaturalResource component = entity.GetComponent<WateredNaturalResource>();
		LivingWaterNaturalResource component2 = entity.GetComponent<LivingWaterNaturalResource>();
		ContaminatedNaturalResource component3 = entity.GetComponent<ContaminatedNaturalResource>();
		AridNaturalResource component4 = entity.GetComponent<AridNaturalResource>();
		if ((bool)component3 && component3.DyingProgress.Died)
		{
			return _loc.T(DiedFromContaminationLocKey);
		}
		if ((bool)component && component.DyingProgress.Died)
		{
			return _loc.T(DriedLocKey);
		}
		if ((bool)component4 && component4.DyingProgress.Died)
		{
			return _loc.T(OverwateredLocKey);
		}
		if ((bool)component2 && component2.DyingProgress.Died)
		{
			string key = (component2.DeathByFlooding ? DiedFromTooMuchWaterLocKey : DiedFromNotEnoughWaterLocKey);
			return _loc.T(key);
		}
		return _loc.T(GenericDiedLocKey);
	}
}
