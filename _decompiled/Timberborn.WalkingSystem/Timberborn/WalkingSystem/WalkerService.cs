using System;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.WalkingSystem;

public class WalkerService
{
	private readonly INavigationService _navigationService;

	public WalkerService(INavigationService navigationService)
	{
		_navigationService = navigationService;
	}

	public Vector3 ClosestPositionOnNavMesh(Vector3 position)
	{
		Vector3? vector = _navigationService.ClosestPositionOnNavMesh(position, 50f);
		if (!vector.HasValue)
		{
			throw new InvalidOperationException($"Couldn't find valid position on NavMesh in {50f} radius around {position}");
		}
		return vector.Value;
	}
}
