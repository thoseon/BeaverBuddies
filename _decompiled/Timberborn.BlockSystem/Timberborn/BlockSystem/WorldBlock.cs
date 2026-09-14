using System;
using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.Coordinates;

namespace Timberborn.BlockSystem;

public struct WorldBlock
{
	private static readonly ReadOnlyList<BlockObject> EmptyList = new List<BlockObject>().AsReadOnlyList();

	private List<BlockObject> _blockObjects;

	private BlockOccupations _blockOccupations;

	public BlockObject Floor { get; private set; }

	public BlockObject Bottom { get; private set; }

	public BlockObject Top { get; private set; }

	public BlockObject Corners { get; private set; }

	public BlockObject Path { get; private set; }

	public BlockObject Middle { get; private set; }

	public BlockObject Underground { get; private set; }

	public Directions2D Entrances { get; private set; }

	public BlockOccupations NonOverridableBlockOccupations { get; private set; }

	public bool HasAnyObject
	{
		get
		{
			if (_blockOccupations == BlockOccupations.None)
			{
				return Underground;
			}
			return true;
		}
	}

	public ReadOnlyList<BlockObject> BlockObjects => _blockObjects?.AsReadOnlyList() ?? EmptyList;

	public void SetBlockObject(BlockObject blockObject, Block block)
	{
		BlockOccupations occupation = block.Occupation;
		if ((occupation != BlockOccupations.None || block.Underground) && (_blockObjects == null || !_blockObjects.Contains(blockObject)))
		{
			if (_blockObjects == null)
			{
				_blockObjects = new List<BlockObject>();
			}
			_blockObjects.Add(blockObject);
		}
		_blockOccupations |= occupation;
		if (!blockObject.Overridable)
		{
			NonOverridableBlockOccupations |= occupation;
		}
		SetBlockObject(blockObject, block, null);
	}

	public void UnsetBlockObject(BlockObject blockObject, Block block)
	{
		_blockObjects?.Remove(blockObject);
		BlockOccupations occupation = block.Occupation;
		_blockOccupations &= ~occupation;
		if (!blockObject.Overridable)
		{
			NonOverridableBlockOccupations &= ~occupation;
		}
		SetBlockObject(null, block, blockObject);
	}

	public void AddEntrance(Direction2D direction2D)
	{
		Directions2D directions2D = direction2D.ToDirections();
		Entrances |= directions2D;
	}

	public void DeleteEntrance(Direction2D direction2D)
	{
		Directions2D directions2D = direction2D.ToDirections();
		Entrances &= ~directions2D;
	}

	public void GetIntersectingObjects(BlockOccupations occupations, List<BlockObject> result)
	{
		bool flag = occupations.Intersects(BlockOccupations.Top) && (bool)Top;
		if (flag)
		{
			result.Add(Top);
		}
		bool flag2 = occupations.Intersects(BlockOccupations.Bottom) && (bool)Bottom && (!flag || Bottom != Top);
		if (flag2)
		{
			result.Add(Bottom);
		}
		bool flag3 = occupations.Intersects(BlockOccupations.Floor) && (bool)Floor && (!flag || Floor != Top) && (!flag2 || Floor != Bottom);
		if (flag3)
		{
			result.Add(Floor);
		}
		bool flag4 = occupations.Intersects(BlockOccupations.Corners) && (bool)Corners && (!flag || Corners != Top) && (!flag2 || Corners != Bottom) && (!flag3 || Corners != Floor);
		if (flag4)
		{
			result.Add(Corners);
		}
		bool flag5 = occupations.Intersects(BlockOccupations.Path) && (bool)Path && (!flag || Path != Top) && (!flag2 || Path != Bottom) && (!flag3 || Path != Floor) && (!flag4 || Path != Corners);
		if (flag5)
		{
			result.Add(Path);
		}
		if (occupations.Intersects(BlockOccupations.Middle) && (bool)Middle && (!flag || Middle != Top) && (!flag2 || Middle != Bottom) && (!flag3 || Middle != Floor) && (!flag4 || Middle != Corners) && (!flag5 || Middle != Path))
		{
			result.Add(Middle);
		}
	}

	private void SetBlockObject(BlockObject newBlockObject, Block block, BlockObject expectedBlockObject)
	{
		BlockOccupations occupation = block.Occupation;
		if (occupation.Intersects(BlockOccupations.Top))
		{
			ValidateSetBlockObject(newBlockObject, BlockOccupations.Top, expectedBlockObject, Top);
			Top = newBlockObject;
		}
		if (occupation.Intersects(BlockOccupations.Bottom))
		{
			ValidateSetBlockObject(newBlockObject, BlockOccupations.Bottom, expectedBlockObject, Bottom);
			Bottom = newBlockObject;
		}
		if (occupation.Intersects(BlockOccupations.Floor))
		{
			ValidateSetBlockObject(newBlockObject, BlockOccupations.Floor, expectedBlockObject, Floor);
			Floor = newBlockObject;
		}
		if (occupation.Intersects(BlockOccupations.Corners))
		{
			ValidateSetBlockObject(newBlockObject, BlockOccupations.Corners, expectedBlockObject, Corners);
			Corners = newBlockObject;
		}
		if (occupation.Intersects(BlockOccupations.Path))
		{
			ValidateSetBlockObject(newBlockObject, BlockOccupations.Path, expectedBlockObject, Path);
			Path = newBlockObject;
		}
		if (occupation.Intersects(BlockOccupations.Middle))
		{
			ValidateSetBlockObject(newBlockObject, BlockOccupations.Middle, expectedBlockObject, Middle);
			Middle = newBlockObject;
		}
		if (block.Underground)
		{
			ValidateSetBlockObjectUnderground(newBlockObject, expectedBlockObject);
			Underground = newBlockObject;
		}
	}

	private static void ValidateSetBlockObject(BlockObject newBlockObject, BlockOccupations blockOccupations, BlockObject expectedBlockObject, BlockObject currentBlockObject)
	{
		if (currentBlockObject != expectedBlockObject)
		{
			throw new ArgumentException($"Can't set {FormatBlockObject(newBlockObject)} on {blockOccupations}, " + "there should be " + FormatBlockObject(expectedBlockObject) + ", but is " + FormatBlockObject(currentBlockObject));
		}
	}

	private void ValidateSetBlockObjectUnderground(BlockObject newBlockObject, BlockObject expectedBlockObject)
	{
		if (Underground != expectedBlockObject)
		{
			throw new ArgumentException("Can't set " + FormatBlockObject(newBlockObject) + " on Underground, there should be " + FormatBlockObject(expectedBlockObject) + ", but is " + FormatBlockObject(Underground));
		}
	}

	private static string FormatBlockObject(BlockObject blockObject)
	{
		if (!blockObject)
		{
			return "null";
		}
		return blockObject.ToString();
	}
}
