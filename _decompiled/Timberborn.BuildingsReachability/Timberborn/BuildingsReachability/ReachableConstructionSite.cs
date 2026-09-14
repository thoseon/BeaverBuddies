using Timberborn.BaseComponentSystem;
using Timberborn.Buildings;
using Timberborn.BuildingsNavigation;
using Timberborn.ConstructionSites;
using Timberborn.Navigation;

namespace Timberborn.BuildingsReachability;

public class ReachableConstructionSite : BaseComponent, IAwakableComponent, IUnreachableEntity
{
	private readonly IDistrictService _districtService;

	private ConstructionSiteAccessible _constructionSiteAccessible;

	private ConstructionSite _constructionSite;

	private BuildingSpec _buildingSpec;

	private IExpandedConstructionSiteReachability _expandedConstructionSiteReachability;

	public ReachableConstructionSite(IDistrictService districtService)
	{
		_districtService = districtService;
	}

	public void Awake()
	{
		_constructionSiteAccessible = GetComponent<ConstructionSiteAccessible>();
		_constructionSite = GetComponent<ConstructionSite>();
		_buildingSpec = GetComponent<BuildingSpec>();
		_expandedConstructionSiteReachability = GetComponent<IExpandedConstructionSiteReachability>();
	}

	public bool IsUnreachable()
	{
		if (!_buildingSpec.PlaceFinished && _constructionSite.IsOn)
		{
			return !IsReachableByBuilders();
		}
		return false;
	}

	public bool IsReachableByBuilders()
	{
		if (_constructionSiteAccessible.Accessible.Enabled && !IsReachableFromBuilderHub())
		{
			return IsReachableByExpandedConstructionSite();
		}
		return true;
	}

	private bool IsReachableFromBuilderHub()
	{
		return _districtService.IsOnInstantDistrictRoadSpill(_constructionSiteAccessible.Accessible);
	}

	private bool IsReachableByExpandedConstructionSite()
	{
		if (_expandedConstructionSiteReachability != null)
		{
			return _expandedConstructionSiteReachability.IsReachable();
		}
		return false;
	}
}
