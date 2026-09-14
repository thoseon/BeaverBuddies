using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;

namespace Timberborn.BuildingsNavigation;

internal class DistrictPathNavRangeDrawerRegistrar : BaseComponent, IAwakableComponent, IInitializableEntity, IDeletableEntity
{
	private readonly PathNavRangeDrawerInvalidator _pathNavRangeDrawerInvalidator;

	private DistrictPathNavRangeDrawer _districtPathNavRangeDrawer;

	public DistrictPathNavRangeDrawerRegistrar(PathNavRangeDrawerInvalidator pathNavRangeDrawerInvalidator)
	{
		_pathNavRangeDrawerInvalidator = pathNavRangeDrawerInvalidator;
	}

	public void Awake()
	{
		_districtPathNavRangeDrawer = GetComponent<DistrictPathNavRangeDrawer>();
	}

	public void InitializeEntity()
	{
		_pathNavRangeDrawerInvalidator.AddDistrictDrawer(_districtPathNavRangeDrawer);
	}

	public void DeleteEntity()
	{
		_pathNavRangeDrawerInvalidator.RemoveDistrictDrawer(_districtPathNavRangeDrawer);
	}
}
