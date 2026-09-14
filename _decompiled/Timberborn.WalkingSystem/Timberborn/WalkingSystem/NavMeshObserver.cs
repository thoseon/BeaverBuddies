using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.MortalComponents;
using Timberborn.Navigation;
using Timberborn.TickSystem;
using UnityEngine;

namespace Timberborn.WalkingSystem;

public class NavMeshObserver : TickableComponent, IAwakableComponent, IInitializableEntity, IPostLoadableEntity, IDeletableEntity, INavMeshListener, IDeadNeededComponent
{
	private readonly WalkerService _walkerService;

	private readonly INavMeshListenerEntityRegistry _navMeshListenerEntityRegistry;

	private Walker _walker;

	private CharacterModel _characterModel;

	private BoundingBox? _navMeshUpdateBounds;

	private readonly List<INavMeshProximityValidator> _navMeshProximityValidators = new List<INavMeshProximityValidator>();

	public event EventHandler PlacedOnNavMesh;

	public NavMeshObserver(WalkerService walkerService, INavMeshListenerEntityRegistry navMeshListenerEntityRegistry)
	{
		_walkerService = walkerService;
		_navMeshListenerEntityRegistry = navMeshListenerEntityRegistry;
	}

	public void Awake()
	{
		_walker = GetComponent<Walker>();
		_characterModel = GetComponent<CharacterModel>();
		GetComponents(_navMeshProximityValidators);
	}

	public void PostLoadEntity()
	{
		ReactToNewNavMesh();
		_navMeshUpdateBounds = null;
	}

	public override void Tick()
	{
		BoundingBox? navMeshUpdateBounds = _navMeshUpdateBounds;
		if (navMeshUpdateBounds.HasValue)
		{
			BoundingBox valueOrDefault = navMeshUpdateBounds.GetValueOrDefault();
			if (valueOrDefault.Intersects(_walker.CurrentPathBounds) || valueOrDefault.Contains(NavigationCoordinateSystem.WorldToGridInt(base.Transform.position)))
			{
				ReactToNewNavMesh();
			}
			_navMeshUpdateBounds = null;
		}
	}

	public void InitializeEntity()
	{
		_navMeshListenerEntityRegistry.RegisterNavMeshListener(this);
	}

	public void DeleteEntity()
	{
		_navMeshListenerEntityRegistry.UnregisterNavMeshListener(this);
	}

	public void OnNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		_navMeshUpdateBounds = navMeshUpdate.Bounds;
	}

	public void Disable()
	{
		DisableComponent();
	}

	private void ReactToNewNavMesh()
	{
		if (base.Enabled)
		{
			PlaceOnNavMesh();
			_walker.RefreshPath();
		}
	}

	private void PlaceOnNavMesh()
	{
		if (!IsOnNavMesh())
		{
			Vector3 position = _walkerService.ClosestPositionOnNavMesh(base.Transform.position);
			base.Transform.position = position;
			_characterModel.Position = position;
			PlacedOnNavMesh?.Invoke(this, EventArgs.Empty);
		}
	}

	private bool IsOnNavMesh()
	{
		foreach (INavMeshProximityValidator navMeshProximityValidator in _navMeshProximityValidators)
		{
			if (navMeshProximityValidator.IsOnNavMesh())
			{
				return true;
			}
		}
		return false;
	}
}
