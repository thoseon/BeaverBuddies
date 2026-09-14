using System.Collections.Immutable;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.PrefabOptimization;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.TerrainSystemRendering;

internal class TerrainBlockRandomizer : ILoadableSingleton
{
	private readonly struct SelectedVariation
	{
		public IntermediateMesh IntermediateMesh { get; }

		public SurfaceBlockShape SurfaceBlockShape { get; }

		public SelectedVariation(IntermediateMesh intermediateMesh, SurfaceBlockShape surfaceBlockShape)
		{
			IntermediateMesh = intermediateMesh;
			SurfaceBlockShape = surfaceBlockShape;
		}
	}

	private static readonly int MaxAttempts = 3;

	private readonly ITerrainService _terrainService;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private Vector3Int _size;

	private SelectedVariation?[,,] _selectedVariations;

	public TerrainBlockRandomizer(ITerrainService terrainService, IRandomNumberGenerator randomNumberGenerator)
	{
		_terrainService = terrainService;
		_randomNumberGenerator = randomNumberGenerator;
	}

	public void Load()
	{
		_size = _terrainService.Size + Vector3Int.one;
		_selectedVariations = new SelectedVariation?[_size.x, _size.y, _size.z];
	}

	public IntermediateMesh PickVariation(ImmutableArray<IntermediateMesh> variations, SurfaceBlockShape surfaceBlockShape, Vector3Int coordinates)
	{
		Vector3Int coordinatesWithOffset = coordinates + Vector3Int.one;
		SelectedVariation? selectedVariation = _selectedVariations[coordinatesWithOffset.x, coordinatesWithOffset.y, coordinatesWithOffset.z];
		if (selectedVariation.HasValue && selectedVariation.Value.SurfaceBlockShape == surfaceBlockShape)
		{
			return selectedVariation.Value.IntermediateMesh;
		}
		SelectedVariation value = PickRandomVariation(variations, surfaceBlockShape, coordinatesWithOffset);
		_selectedVariations[coordinatesWithOffset.x, coordinatesWithOffset.y, coordinatesWithOffset.z] = value;
		return value.IntermediateMesh;
	}

	private SelectedVariation PickRandomVariation(ImmutableArray<IntermediateMesh> variations, SurfaceBlockShape surfaceBlockShape, Vector3Int coordinatesWithOffset)
	{
		int length = variations.Length;
		int num = 0;
		IntermediateMesh intermediateMesh;
		do
		{
			int index = _randomNumberGenerator.Range(0, length);
			intermediateMesh = variations[index];
		}
		while (length > 1 && num++ < MaxAttempts && AnyNeighborIsTheSame(coordinatesWithOffset, intermediateMesh));
		return new SelectedVariation(intermediateMesh, surfaceBlockShape);
	}

	private bool AnyNeighborIsTheSame(Vector3Int coordinatesWithOffset, IntermediateMesh intermediateMesh)
	{
		if (coordinatesWithOffset.x > 0 && coordinatesWithOffset.x < _size.x - 1 && coordinatesWithOffset.y > 0 && coordinatesWithOffset.y < _size.y - 1)
		{
			Vector3Int[] neighbors4Vector3Int = Deltas.Neighbors4Vector3Int;
			foreach (Vector3Int vector3Int in neighbors4Vector3Int)
			{
				Vector3Int vector3Int2 = coordinatesWithOffset + vector3Int;
				if (_selectedVariations[vector3Int2.x, vector3Int2.y, vector3Int2.z]?.IntermediateMesh == intermediateMesh)
				{
					return true;
				}
			}
		}
		return false;
	}
}
