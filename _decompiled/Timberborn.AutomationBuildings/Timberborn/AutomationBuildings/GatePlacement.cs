using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.PathSystem;
using UnityEngine;

namespace Timberborn.AutomationBuildings;

internal class GatePlacement : BaseComponent, IAwakableComponent, IPreInitializableEntity, IPathConnectionEnforcer
{
	private readonly IBlockService _blockService;

	private GateSpec _spec;

	private BlockObject _blockObject;

	public Vector3Int Start { get; private set; }

	public Vector3Int End { get; private set; }

	public Vector3Int Center { get; private set; }

	public GatePlacement(IBlockService blockService)
	{
		_blockService = blockService;
	}

	public void Awake()
	{
		_spec = GetComponent<GateSpec>();
		_blockObject = GetComponent<BlockObject>();
	}

	public void PreInitializeEntity()
	{
		Start = _blockObject.TransformCoordinates(_spec.Start);
		End = _blockObject.TransformCoordinates(_spec.End);
		Center = _blockObject.Coordinates;
	}

	public bool CanConnectPath(Vector3Int origin, Vector3Int target)
	{
		if (_blockObject.IsFinished)
		{
			if (target == Center && (origin == Start || origin == End))
			{
				return true;
			}
			BlockObject pathObjectAt = _blockService.GetPathObjectAt(target);
			if (pathObjectAt != null && pathObjectAt.IsFinished && origin == Center)
			{
				if (!(target == Start))
				{
					return target == End;
				}
				return true;
			}
			return false;
		}
		return false;
	}
}
