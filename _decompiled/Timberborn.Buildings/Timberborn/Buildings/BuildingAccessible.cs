using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.Navigation;
using UnityEngine;

namespace Timberborn.Buildings;

public class BuildingAccessible : BaseComponent, IAwakableComponent, IFinishedStateListener, IPreviewSelectionListener, IPostPlacementChangeListener, IRegisteredComponent, IAccessibleNeeder
{
	private BlockObject _blockObject;

	private BuildingAccessibleSpec _buildingAccessibleSpec;

	private Vector3 _access;

	public Accessible Accessible { get; private set; }

	public string AccessibleComponentName => "Building";

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_buildingAccessibleSpec = GetComponent<BuildingAccessibleSpec>();
		if (!GetComponent<BlockObjectSpec>().Entrance.HasEntrance)
		{
			throw new InvalidOperationException(base.Name + " is BuildingAccessible but doesn't have a BlockObjectSpec entrance");
		}
	}

	public void SetAccessible(Accessible accessible)
	{
		Accessible = accessible;
	}

	public void OnEnterFinishedState()
	{
		UpdateAccess();
		EnableAccesses();
	}

	public void OnExitFinishedState()
	{
		Accessible.ClearAccesses();
	}

	public void OnPreviewSelect()
	{
		EnableAccesses();
	}

	public void OnPreviewUnselect()
	{
	}

	public void OnPostPlacementChanged()
	{
		UpdateAccess();
	}

	public Vector3 CalculateAccess()
	{
		if (!_buildingAccessibleSpec.ForceOneFinalAccess)
		{
			return CalculateAccessAtEntrance();
		}
		return CalculateAccessFromLocalAccess();
	}

	public Vector3 CalculateAccessFromLocalAccess()
	{
		return _buildingAccessibleSpec.CalculateAccessFromLocalAccess(_blockObject);
	}

	private void EnableAccesses()
	{
		if (_buildingAccessibleSpec.ForceOneFinalAccess)
		{
			Accessible.SetAccesses(Enumerables.One(_access));
		}
		else
		{
			Accessible.SetAccesses(Enumerables.One(_access), CalculateAccessFromLocalAccess());
		}
	}

	private void UpdateAccess()
	{
		_access = CalculateAccess();
	}

	private Vector3 CalculateAccessAtEntrance()
	{
		return CoordinateSystem.GridToWorldCentered(_blockObject.PositionedEntrance.Coordinates);
	}
}
