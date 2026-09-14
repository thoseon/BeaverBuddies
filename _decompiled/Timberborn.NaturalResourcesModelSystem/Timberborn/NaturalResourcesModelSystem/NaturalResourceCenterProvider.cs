using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.NaturalResources;
using UnityEngine;

namespace Timberborn.NaturalResourcesModelSystem;

public class NaturalResourceCenterProvider : BaseComponent, IAwakableComponent
{
	private BlockObjectCenter _blockObjectCenter;

	private CoordinatesOffsetter _coordinatesOffsetter;

	public void Awake()
	{
		_blockObjectCenter = GetComponent<BlockObjectCenter>();
		_coordinatesOffsetter = GetComponent<CoordinatesOffsetter>();
	}

	public Vector3 GetWorldCenter()
	{
		Vector3 worldCenterGrounded = _blockObjectCenter.WorldCenterGrounded;
		Vector3 vector = CoordinateSystem.GridToWorld(_coordinatesOffsetter.CoordinatesOffset.XYZ());
		return worldCenterGrounded + vector;
	}
}
