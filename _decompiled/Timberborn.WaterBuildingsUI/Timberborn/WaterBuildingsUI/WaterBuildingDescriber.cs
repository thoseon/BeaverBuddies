using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.WaterBuildings;

namespace Timberborn.WaterBuildingsUI;

internal class WaterBuildingDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private static readonly string WaterAccessLocKey = "Buildings.WaterAccess";

	private static readonly string BadwaterAccessLocKey = "Buildings.BadwaterAccess";

	private readonly ILoc _loc;

	private BlockObject _blockObject;

	private IContaminatedWaterNeedingBuilding _contaminatedWaterNeedingBuilding;

	public WaterBuildingDescriber(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_contaminatedWaterNeedingBuilding = GetComponent<IContaminatedWaterNeedingBuilding>();
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		if (_blockObject.IsPreview)
		{
			string key = ((_contaminatedWaterNeedingBuilding != null) ? BadwaterAccessLocKey : WaterAccessLocKey);
			string content = SpecialStrings.RowStarter + _loc.T(key);
			yield return EntityDescription.CreateTextSection(content, 2010);
		}
	}
}
