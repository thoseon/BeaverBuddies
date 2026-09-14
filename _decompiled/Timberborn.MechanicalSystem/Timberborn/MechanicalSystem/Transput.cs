using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.MechanicalSystem;

public class Transput
{
	private readonly BlockObject _blockObject;

	private readonly Vector3Int _coordinates;

	private readonly Direction3D _direction;

	private readonly bool _initialRotation;

	public MechanicalNode ParentNode { get; }

	public bool ReversedRotation { get; private set; }

	public Transput ConnectedTransput { get; private set; }

	public Vector3Int Coordinates => _blockObject.TransformCoordinates(_coordinates);

	public Direction3D Direction => _blockObject.TransformDirection(_direction);

	public Direction3D BaseDirection => _direction;

	public Vector3Int Target => Coordinates + Direction.ToOffset();

	public Vector3Int Offset => _coordinates - new Vector3Int(0, 0, _blockObject.BaseZ);

	public MechanicalNode ConnectedNode
	{
		get
		{
			if (!Connected)
			{
				return null;
			}
			return ConnectedTransput.ParentNode;
		}
	}

	public bool IsFinished => _blockObject.IsFinished;

	public bool Connected => ConnectedTransput != null;

	public Transput(MechanicalNode parentNode, TransputSpec transputSpec, Direction3D direction, BlockObject blockObject)
	{
		ParentNode = parentNode;
		ReversedRotation = (_initialRotation = (blockObject.FlipMode.IsFlipped ? (!transputSpec.ReverseRotation) : transputSpec.ReverseRotation));
		_coordinates = transputSpec.Coordinates;
		_direction = direction;
		_blockObject = blockObject;
	}

	public void Connect(Transput other)
	{
		ConnectedTransput = other;
	}

	public void Disconnect()
	{
		ConnectedTransput = null;
	}

	public void ReverseRotation()
	{
		ReversedRotation = !ReversedRotation;
	}

	public void ResetRotation()
	{
		ReversedRotation = _initialRotation;
	}

	public bool RotationMatches(Transput other)
	{
		return ReversedRotation == other.ReversedRotation;
	}

	public bool Faces(Transput other)
	{
		if (other.Target == Coordinates && Target == other.Coordinates)
		{
			return other.Direction == Direction.Across();
		}
		return false;
	}
}
