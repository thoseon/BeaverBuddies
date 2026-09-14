using Timberborn.BaseComponentSystem;
using Timberborn.Navigation;

namespace Timberborn.CharacterMovementSystem;

public class PathFollowerFactory
{
	private readonly INavigationService _navigationService;

	public PathFollowerFactory(INavigationService navigationService)
	{
		_navigationService = navigationService;
	}

	public PathFollower Create(BaseComponent owner)
	{
		return new PathFollower(_navigationService, owner.GetComponent<MovementAnimator>(), owner.Transform);
	}
}
