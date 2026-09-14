using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.GoodsUI;
using Timberborn.Localization;
using Timberborn.Yielding;

namespace Timberborn.YieldingUI;

public class YieldRemovingBuildingDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private static readonly string GatheringLocKey = "Gathering.Action";

	private readonly ILoc _loc;

	private readonly GoodDescriber _goodDescriber;

	private YieldRemovingBuilding _yieldRemovingBuilding;

	public YieldRemovingBuildingDescriber(ILoc loc, GoodDescriber goodDescriber)
	{
		_loc = loc;
		_goodDescriber = goodDescriber;
	}

	public void Awake()
	{
		_yieldRemovingBuilding = GetComponent<YieldRemovingBuilding>();
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		if (!base.GameObject.activeInHierarchy)
		{
			yield return EntityDescription.CreateTextSection(Describe().TrimEnd(), 100);
		}
	}

	private string Describe()
	{
		StringBuilder stringBuilder = new StringBuilder();
		StringListBuilder stringListBuilder = new StringListBuilder(stringBuilder, ", ");
		stringBuilder.Append(SpecialStrings.RowStarter + _loc.T(GatheringLocKey) + " ");
		foreach (string allowedGoodName in GetAllowedGoodNames())
		{
			stringListBuilder.BeginItem();
			stringBuilder.Append(allowedGoodName);
		}
		return stringBuilder.ToString();
	}

	private IEnumerable<string> GetAllowedGoodNames()
	{
		return (from yielderDecorable in _yieldRemovingBuilding.GetAllowedYielders()
			select GetPluralDisplayName(yielderDecorable.Yielder)).Distinct();
	}

	private string GetPluralDisplayName(YielderSpec spec)
	{
		return _goodDescriber.Describe(spec.Yield.Id);
	}
}
