using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.StatusSystem;

internal class StatusSlotOccupier : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private BlockObject _blockObject;

	private IStatusIconOffsetter _statusIconOffsetter;

	private int _topZCoordinate;

	public bool UseUnfinishedConstructionModeModel { get; private set; }

	public bool IsUnfinished => _blockObject.IsUnfinished;

	public byte BaseZ => (byte)_blockObject.CoordinatesAtBaseZ.z;

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_statusIconOffsetter = GetComponent<IStatusIconOffsetter>();
	}

	public void InitializeEntity()
	{
		IBlockObjectModel component = GetComponent<IBlockObjectModel>();
		if (component != null)
		{
			UseUnfinishedConstructionModeModel = component.UnfinishedConstructionModeModel;
		}
		Vector3Int size = GetComponent<BlockObjectSpec>().Size;
		_topZCoordinate = _blockObject.CoordinatesAtBaseZ.z + size.z - 1;
	}

	public float GetNormalModeTopBound()
	{
		if (_blockObject.IsFinished)
		{
			return _statusIconOffsetter?.FinishedTopBound ?? 0f;
		}
		return _statusIconOffsetter?.UnfinishedTopBound ?? 0f;
	}

	public TopBoundForLayer GetTopBound(Vector3Int coordinates)
	{
		Block block = _blockObject.PositionedBlocks.GetBlock(coordinates);
		if ((coordinates.z == _topZCoordinate || block.Occupation.Intersects(SlotBlockOccupation.Default)) && _statusIconOffsetter != null)
		{
			return new TopBoundForLayer(_statusIconOffsetter.FinishedTopBound, GetNormalModeTopBound());
		}
		return new TopBoundForLayer(0f, 0f);
	}

	public bool IntersectsAt(Vector3Int coordinates, BlockOccupations occupations)
	{
		return _blockObject.PositionedBlocks.GetBlock(coordinates).Occupation.Intersects(occupations);
	}
}
