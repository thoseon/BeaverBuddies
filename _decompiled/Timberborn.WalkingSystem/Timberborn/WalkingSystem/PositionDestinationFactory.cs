using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.WalkingSystem;

public class PositionDestinationFactory
{
	private readonly INavigationService _navigationService;

	private readonly WalkerService _walkerService;

	public PositionDestinationFactory(INavigationService navigationService, WalkerService walkerService)
	{
		_navigationService = navigationService;
		_walkerService = walkerService;
	}

	public PositionDestination Create(Vector3 position, float stoppingDistance)
	{
		return new PositionDestination(_navigationService, _walkerService, position, stoppingDistance);
	}
}
