using Timberborn.BaseComponentSystem;
using Timberborn.Navigation;

namespace Timberborn.WalkingSystem;

public class NavMeshProximityValidator : BaseComponent, INavMeshProximityValidator
{
	private readonly INavigationService _navigationService;

	public NavMeshProximityValidator(INavigationService navigationService)
	{
		_navigationService = navigationService;
	}

	public bool IsOnNavMesh()
	{
		return _navigationService.IsOnNavMesh(base.Transform.position);
	}
}
