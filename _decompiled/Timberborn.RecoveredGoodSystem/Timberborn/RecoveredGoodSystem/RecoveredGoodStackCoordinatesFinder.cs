using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BlockSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.RecoveredGoodSystem;

internal class RecoveredGoodStackCoordinatesFinder : ILoadableSingleton
{
	private readonly struct OverridableCoordinates
	{
		public Vector3Int Coordinates { get; }

		public BlockObject OverridingObject { get; }

		public OverridableCoordinates(Vector3Int coordinates, BlockObject overridingObject)
		{
			Coordinates = coordinates;
			OverridingObject = overridingObject;
		}

		public OverridableCoordinates Move(Vector3Int offset)
		{
			return new OverridableCoordinates(Coordinates + offset, OverridingObject);
		}
	}

	private readonly IBlockService _blockService;

	private readonly BlockValidator _blockValidator;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly ITerrainService _terrainService;

	private readonly RecoveredGoodStackFactory _recoveredGoodStackFactory;

	private readonly ISpecService _specService;

	private RecoveredGoodStackCoordinatesFinderSpec _spec;

	private ImmutableArray<Vector2Int> _spiralNeighbours;

	private BlockSpec _goodStackBlockSpec;

	public RecoveredGoodStackCoordinatesFinder(IBlockService blockService, BlockValidator blockValidator, IRandomNumberGenerator randomNumberGenerator, ITerrainService terrainService, RecoveredGoodStackFactory recoveredGoodStackFactory, ISpecService specService)
	{
		_blockService = blockService;
		_blockValidator = blockValidator;
		_randomNumberGenerator = randomNumberGenerator;
		_terrainService = terrainService;
		_recoveredGoodStackFactory = recoveredGoodStackFactory;
		_specService = specService;
	}

	public void Load()
	{
		_spec = _specService.GetSingleSpec<RecoveredGoodStackCoordinatesFinderSpec>();
		_spiralNeighbours = NeighbourFinder.GetSpiralNeighboursXY(_spec.NeighboursRange).ToImmutableArray();
		_goodStackBlockSpec = _recoveredGoodStackFactory.GoodStackBlockSpec;
	}

	public bool FindValidCoordinates(Vector3Int original, out Vector3Int validCoordinates)
	{
		return FindValidCoordinates(original, null, out validCoordinates);
	}

	public bool FindValidCoordinates(Vector3Int original, BlockObject overridingObject, out Vector3Int validCoordinates)
	{
		OverridableCoordinates coordinates = new OverridableCoordinates(original, overridingObject);
		bool result = TryFindNewCoordinates(coordinates, out var movedCoordinates);
		validCoordinates = movedCoordinates.Coordinates;
		return result;
	}

	private bool TryFindNewCoordinates(OverridableCoordinates coordinates, out OverridableCoordinates movedCoordinates)
	{
		if (TryToFall(coordinates, out movedCoordinates))
		{
			return true;
		}
		foreach (OverridableCoordinates randomizedSpiralCoordinate in GetRandomizedSpiralCoordinates(coordinates))
		{
			if (AreCoordinatesValid(randomizedSpiralCoordinate))
			{
				movedCoordinates = randomizedSpiralCoordinate;
				return true;
			}
			if (TryToFall(randomizedSpiralCoordinate, out movedCoordinates) || TryMoveUp(randomizedSpiralCoordinate, out movedCoordinates))
			{
				return true;
			}
		}
		movedCoordinates = default(OverridableCoordinates);
		return false;
	}

	private bool TryMoveUp(OverridableCoordinates coordinates, out OverridableCoordinates movedCoordinates)
	{
		for (int i = 1; i < _spec.MaxUpperSearch; i++)
		{
			movedCoordinates = coordinates.Move(new Vector3Int(0, 0, i));
			if (AreCoordinatesValid(movedCoordinates))
			{
				return true;
			}
		}
		movedCoordinates = default(OverridableCoordinates);
		return false;
	}

	private bool TryToFall(OverridableCoordinates coordinates, out OverridableCoordinates movedCoordinates)
	{
		OverridableCoordinates fallCoordinates = GetFallCoordinates(coordinates);
		if (AreCoordinatesValid(fallCoordinates))
		{
			movedCoordinates = fallCoordinates;
			return true;
		}
		movedCoordinates = default(OverridableCoordinates);
		return false;
	}

	private OverridableCoordinates GetFallCoordinates(OverridableCoordinates coordinates)
	{
		while (coordinates.Coordinates.z > 0)
		{
			OverridableCoordinates overridableCoordinates = coordinates.Move(new Vector3Int(0, 0, -1));
			if (_blockService.AnyTopObjectAt(overridableCoordinates.Coordinates) || _terrainService.Underground(overridableCoordinates.Coordinates))
			{
				break;
			}
			coordinates = overridableCoordinates;
		}
		return coordinates;
	}

	private IEnumerable<OverridableCoordinates> GetRandomizedSpiralCoordinates(OverridableCoordinates coordinates)
	{
		Orientation direction = RandomizeDirection();
		foreach (Vector2Int spiralNeighbour in _spiralNeighbours)
		{
			Vector3Int offset = direction.Transform(spiralNeighbour.XYZ());
			yield return coordinates.Move(offset);
		}
	}

	private Orientation RandomizeDirection()
	{
		return _randomNumberGenerator.Range(0, 4) switch
		{
			0 => Orientation.Cw0, 
			1 => Orientation.Cw90, 
			2 => Orientation.Cw180, 
			_ => Orientation.Cw270, 
		};
	}

	private bool AreCoordinatesValid(OverridableCoordinates coordinates)
	{
		Block block = Block.From(coordinates.Coordinates, _goodStackBlockSpec);
		if ((bool)coordinates.OverridingObject && coordinates.OverridingObject.IsIntersecting(block))
		{
			return false;
		}
		return _blockValidator.BlockValidWithoutUnfinishedStackable(block);
	}
}
