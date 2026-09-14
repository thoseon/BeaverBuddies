using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.MechanicalSystem;
using UnityEngine;

namespace Timberborn.ModularShafts;

internal class ModularShaftVariantFinder : BaseComponent, IAwakableComponent
{
	private static readonly ShaftVariant BottomTopStraightVariant = new ShaftVariant(0, 0, 0, 0, 1, 2);

	private static readonly ShaftVariant UpDownStraightVariant = new ShaftVariant(1, 0, 2, 0, 0, 0);

	private static readonly ShaftVariant LeftRightStraightVariant = new ShaftVariant(0, 1, 0, 2, 0, 0);

	private readonly IBlockService _blockService;

	private readonly PreviewBlockService _previewBlockService;

	private BlockObject _blockObject;

	private MechanicalNode _mechanicalNode;

	private ImmutableHashSet<Direction3D> _supportedDirections;

	private readonly Dictionary<Direction3D, Transput> _directedTransputs = new Dictionary<Direction3D, Transput>();

	public ModularShaftVariantFinder(IBlockService blockService, PreviewBlockService previewBlockService)
	{
		_blockService = blockService;
		_previewBlockService = previewBlockService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_mechanicalNode = GetComponent<MechanicalNode>();
		_mechanicalNode.TransputsInitialized += OnTransputsInitialized;
		_supportedDirections = GetSupportedDirections();
	}

	public ShaftVariant FindBestVariant()
	{
		TransputRotation rotation = GetRotation(Direction3D.Down);
		TransputRotation rotation2 = GetRotation(Direction3D.Left);
		TransputRotation rotation3 = GetRotation(Direction3D.Up);
		TransputRotation rotation4 = GetRotation(Direction3D.Right);
		TransputRotation rotation5 = GetRotation(Direction3D.Bottom);
		TransputRotation rotation6 = GetRotation(Direction3D.Top);
		return OptimizeVariant(rotation, rotation2, rotation3, rotation4, rotation5, rotation6);
	}

	private ImmutableHashSet<Direction3D> GetSupportedDirections()
	{
		HashSet<Direction3D> hashSet = new HashSet<Direction3D>();
		foreach (TransputSpec transput in GetComponent<TransputProviderSpec>().Transputs)
		{
			foreach (Direction3D item in transput.Directions.GetEnumerator())
			{
				hashSet.Add(item);
			}
		}
		return hashSet.ToImmutableHashSet();
	}

	private void OnTransputsInitialized(object sender, EventArgs e)
	{
		foreach (Transput transput in _mechanicalNode.Transputs)
		{
			if (!_supportedDirections.Contains(transput.BaseDirection))
			{
				throw new NotSupportedException($"Unexpected value: {transput.BaseDirection}.");
			}
			_directedTransputs[transput.BaseDirection] = transput;
		}
	}

	private TransputRotation GetRotation(Direction3D direction)
	{
		if (_supportedDirections.Contains(direction))
		{
			TransputRotation rotationFromTransputs = GetRotationFromTransputs(direction);
			if (rotationFromTransputs == TransputRotation.None)
			{
				if (!CanBeConnectedToMechanicalNode(direction))
				{
					return TransputRotation.None;
				}
				return TransputRotation.Ignored;
			}
			return rotationFromTransputs;
		}
		return TransputRotation.None;
	}

	private TransputRotation GetRotationFromTransputs(Direction3D direction)
	{
		if (_directedTransputs.TryGetValue(direction, out var value) && value.Connected)
		{
			MechanicalNode parentNode = value.ConnectedTransput.ParentNode;
			if (parentNode.IgnoreRotation || (!parentNode.IsGenerator && !parentNode.IsShaft))
			{
				return TransputRotation.Ignored;
			}
			if (!value.ReversedRotation)
			{
				return TransputRotation.Normal;
			}
			return TransputRotation.Reversed;
		}
		return TransputRotation.None;
	}

	private bool CanBeConnectedToMechanicalNode(Direction3D direction)
	{
		Direction3D direction3D = _blockObject.TransformDirection(direction);
		Vector3Int coordinates = _blockObject.Coordinates + direction3D.ToOffset();
		TransputProvider objectAt = GetObjectAt(coordinates);
		if (objectAt != null)
		{
			return CanBeConnectedToAnyTransput(direction3D, objectAt.GetComponent<BlockObject>(), objectAt.TransputSpecs);
		}
		return false;
	}

	private TransputProvider GetObjectAt(Vector3Int coordinates)
	{
		TransputProvider firstObjectWithComponentAt = _blockService.GetFirstObjectWithComponentAt<TransputProvider>(coordinates);
		if (firstObjectWithComponentAt != null)
		{
			return firstObjectWithComponentAt;
		}
		TransputProvider firstObjectWithComponentAt2 = _previewBlockService.GetFirstObjectWithComponentAt<TransputProvider>(coordinates);
		if (firstObjectWithComponentAt2 != null)
		{
			return firstObjectWithComponentAt2;
		}
		return null;
	}

	private bool CanBeConnectedToAnyTransput(Direction3D direction, BlockObject transputOwner, ImmutableArray<TransputSpec> transputSpecs)
	{
		foreach (TransputSpec item in transputSpecs)
		{
			Vector3Int vector3Int = transputOwner.TransformCoordinates(item.Coordinates);
			foreach (Direction3D item2 in item.Directions.GetEnumerator())
			{
				Direction3D direction3D = transputOwner.TransformDirection(item2);
				if (vector3Int + direction3D.ToOffset() == _blockObject.Coordinates && direction3D == direction.Across())
				{
					return true;
				}
			}
		}
		return false;
	}

	private ShaftVariant OptimizeVariant(TransputRotation down, TransputRotation left, TransputRotation up, TransputRotation right, TransputRotation bottom, TransputRotation top)
	{
		if (down == TransputRotation.None && left == TransputRotation.None && up == TransputRotation.None && right == TransputRotation.None && bottom == TransputRotation.None && top == TransputRotation.None)
		{
			return new ShaftVariant(1, 1, 1, 1, 2, (byte)(_supportedDirections.Contains(Direction3D.Top) ? 2u : 0u));
		}
		OptimizeForIgnoredRotation(ref down, ref left, ref up, ref right, ref bottom, ref top);
		return OptimizeForInactiveStraightShaft(new ShaftVariant(down.AsByte(), left.AsByte(), up.AsByte(), right.AsByte(), bottom.AsByte(), top.AsByte()));
	}

	private static void OptimizeForIgnoredRotation(ref TransputRotation down, ref TransputRotation left, ref TransputRotation up, ref TransputRotation right, ref TransputRotation bottom, ref TransputRotation top)
	{
		if (down == TransputRotation.Ignored)
		{
			down = up.ReverseOrSetNormal();
		}
		if (up == TransputRotation.Ignored)
		{
			up = down.ReverseOrSetNormal();
		}
		if (right == TransputRotation.Ignored)
		{
			right = left.ReverseOrSetNormal();
		}
		if (left == TransputRotation.Ignored)
		{
			left = right.ReverseOrSetNormal();
		}
		if (bottom == TransputRotation.Ignored)
		{
			bottom = top.ReverseOrSetNormal();
		}
		if (top == TransputRotation.Ignored)
		{
			top = bottom.ReverseOrSetNormal();
		}
	}

	private ShaftVariant OptimizeForInactiveStraightShaft(ShaftVariant variant)
	{
		if (IsInactive())
		{
			if (variant.Left > 0 && variant.Right > 0 && variant.Up == 0 && variant.Down == 0 && variant.Bottom == 0 && variant.Top == 0)
			{
				return LeftRightStraightVariant;
			}
			if (variant.Up > 0 && variant.Down > 0 && variant.Left == 0 && variant.Right == 0 && variant.Bottom == 0 && variant.Top == 0)
			{
				return UpDownStraightVariant;
			}
			if (variant.Bottom > 0 && variant.Top > 0 && variant.Left == 0 && variant.Right == 0 && variant.Up == 0 && variant.Down == 0)
			{
				return BottomTopStraightVariant;
			}
		}
		return variant;
	}

	private bool IsInactive()
	{
		if (_mechanicalNode.Graph != null)
		{
			return _mechanicalNode.Graph.NumberOfGenerators == 0;
		}
		return true;
	}
}
