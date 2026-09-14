using JetBrains.Annotations;
using Timberborn.BlueprintSystem;
using Timberborn.Planting;

namespace Timberborn.Cutting;

[UsedImplicitly]
internal record CuttableYieldGoodIdProviderSpec : ComponentSpec, IPlantableGoodIdProvider
{
	public string GetGoodId()
	{
		return GetSpec<CuttableSpec>().Yielder.Yield.Id;
	}
}
