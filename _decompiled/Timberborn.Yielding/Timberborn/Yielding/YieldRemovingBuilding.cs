using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Goods;
using Timberborn.TemplateSystem;

namespace Timberborn.Yielding;

public class YieldRemovingBuilding : BaseComponent, IAwakableComponent, IAllowedGoodProvider
{
	private readonly TemplateService _templateService;

	private readonly IGoodService _goodService;

	private YieldRemovingBuildingSpec _yieldRemovingBuildingSpec;

	public YieldRemovingBuilding(TemplateService templateService, IGoodService goodService)
	{
		_templateService = templateService;
		_goodService = goodService;
	}

	public void Awake()
	{
		_yieldRemovingBuildingSpec = GetComponent<YieldRemovingBuildingSpec>();
	}

	public IEnumerable<string> GetAllowedGoods()
	{
		return (from yielderDecorable in GetAllowedYielders()
			select yielderDecorable.Yielder.Yield.Id).Distinct();
	}

	public IEnumerable<IYielderDecorable> GetAllowedYielders()
	{
		if (_yieldRemovingBuildingSpec == null)
		{
			_yieldRemovingBuildingSpec = GetComponent<YieldRemovingBuildingSpec>();
		}
		return from yielderDecorable in _templateService.GetAll<IYielderDecorable>()
			where IsAllowed(yielderDecorable.Yielder)
			orderby ((ComponentSpec)yielderDecorable).GetSpec<IOrderableYielder>().Order
			select yielderDecorable;
	}

	public bool IsAllowed(YielderSpec yielderSpec)
	{
		if (_goodService.GetGoodOrNull(yielderSpec.Yield.Id) != null)
		{
			return yielderSpec.ResourceGroup == _yieldRemovingBuildingSpec.ResourceGroup;
		}
		return false;
	}
}
