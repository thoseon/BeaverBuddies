using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.BlockObjectAccesses;

public class BlockObjectAccessible : BaseComponent, IAwakableComponent, IInitializableEntity, INavMeshListener, IDeletableEntity, IAccessibleNeeder
{
	private readonly INavMeshListenerEntityRegistry _navMeshListenerEntityRegistry;

	private BlockObject _blockObject;

	private BlockObjectAccessGenerator _blockObjectAccessGenerator;

	private BoundingBox _bounds;

	private int _accessLevelsAboveGround;

	public Accessible Accessible { get; private set; }

	public string AccessibleComponentName => "BlockObjectAccessible";

	private int MinZ => _blockObject.Coordinates.z;

	private int MaxZ => _blockObject.Coordinates.z + _accessLevelsAboveGround;

	public BlockObjectAccessible(INavMeshListenerEntityRegistry navMeshListenerEntityRegistry)
	{
		_navMeshListenerEntityRegistry = navMeshListenerEntityRegistry;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_blockObjectAccessGenerator = GetComponent<BlockObjectAccessGenerator>();
	}

	public void InitializeEntity()
	{
		if (!_blockObject.IsPreview)
		{
			UpdateBounds();
			UpdateAccesses();
			_navMeshListenerEntityRegistry.RegisterNavMeshListener(this);
		}
	}

	public void DeleteEntity()
	{
		_navMeshListenerEntityRegistry.UnregisterNavMeshListener(this);
	}

	public void SetAccessible(Accessible accessible)
	{
		Accessible = accessible;
	}

	public void OnNavMeshUpdated(NavMeshUpdate navMeshUpdate)
	{
		if (_bounds.Intersects(navMeshUpdate.Bounds))
		{
			UpdateAccesses();
		}
	}

	public void SetNumberOfAccessLevelsAboveGround(int accessLevelsAboveGround)
	{
		_accessLevelsAboveGround = accessLevelsAboveGround;
		if (Accessible.Enabled)
		{
			UpdateBounds();
			UpdateAccesses();
		}
	}

	private void UpdateBounds()
	{
		_bounds = _blockObjectAccessGenerator.GenerateAccessBounds(MinZ, MaxZ);
	}

	private void UpdateAccesses()
	{
		Accessible.SetAccesses(GenerateAccesses());
	}

	private IEnumerable<Vector3> GenerateAccesses()
	{
		return _blockObjectAccessGenerator.GenerateAccesses(MinZ, MaxZ);
	}
}
