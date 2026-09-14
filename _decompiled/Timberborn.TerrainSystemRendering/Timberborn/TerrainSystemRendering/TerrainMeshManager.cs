using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.LevelVisibilitySystem;
using Timberborn.MapIndexSystem;
using Timberborn.MapStateSystem;
using Timberborn.PrefabOptimization;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.TerrainSystemRendering;

internal class TerrainMeshManager : ILoadableSingleton, ILateUpdatableSingleton
{
	private readonly struct TileComponents(GameObject gameObject)
	{
		private readonly GameObject _gameObject = gameObject;

		private readonly MeshRenderer _meshRenderer = gameObject.GetComponent<MeshRenderer>();

		private readonly MeshFilter _meshFilter = gameObject.GetComponent<MeshFilter>();

		public void UpdateMesh(BuiltMesh buildMesh)
		{
			UnityEngine.Object.Destroy(_meshFilter.sharedMesh);
			_meshFilter.sharedMesh = buildMesh.Mesh;
			_meshFilter.sharedMesh.UploadMeshData(markNoLongerReadable: true);
			if (_meshRenderer.sharedMaterials == null || _meshRenderer.sharedMaterials.Length != buildMesh.Materials.Length)
			{
				_meshRenderer.sharedMaterials = buildMesh.Materials;
			}
			_gameObject.SetActive(value: true);
		}

		public void Deactivate()
		{
			_gameObject.SetActive(value: false);
		}
	}

	public static readonly float TerrainStumpHeight = 0.85f;

	private static readonly int TerrainStumpHeightProperty = Shader.PropertyToID("_TerrainStumpHeight");

	private static readonly Vector3Int[] NeighborOffsets = new Vector3Int[4]
	{
		new Vector3Int(0, 0, 0),
		new Vector3Int(0, 1, 0),
		new Vector3Int(1, 0, 0),
		new Vector3Int(1, 1, 0)
	};

	private static readonly Vector3 PrefabTranslationOffset = new Vector3(1f, 0f, 1f);

	private readonly ITerrainService _terrainService;

	private readonly TerrainBlockRepository _terrainBlockRepository;

	private readonly TerrainBlockRandomizer _terrainBlockRandomizer;

	private readonly MapIndexService _mapIndexService;

	private readonly RootObjectProvider _rootObjectProvider;

	private readonly ISpecService _specService;

	private readonly MapSize _mapSize;

	private readonly MeshBuilder _meshBuilder = new MeshBuilder();

	private readonly Dictionary<Vector3Int, TileComponents> _tiles = new Dictionary<Vector3Int, TileComponents>();

	private readonly HashSet<Vector3Int> _invalidTiles = new HashSet<Vector3Int>();

	private readonly HashSet<Vector3Int> _dirtyCodes = new HashSet<Vector3Int>();

	private GameObject _root;

	private GameObject _terrainTilePrefab;

	private byte[,,] _surfaceShapeCodes;

	public TerrainMeshManager(ITerrainService terrainService, TerrainBlockRepository terrainBlockRepository, TerrainBlockRandomizer terrainBlockRandomizer, MapIndexService mapIndexService, ILevelVisibilityService levelVisibilityService, RootObjectProvider rootObjectProvider, ISpecService specService, MapSize mapSize)
	{
		_terrainService = terrainService;
		_terrainBlockRepository = terrainBlockRepository;
		_terrainBlockRandomizer = terrainBlockRandomizer;
		_mapIndexService = mapIndexService;
		_rootObjectProvider = rootObjectProvider;
		_specService = specService;
		_mapSize = mapSize;
	}

	public void Load()
	{
		_root = _rootObjectProvider.CreateRootObject("TerrainMeshManager");
		_terrainTilePrefab = _specService.GetSingleSpec<TerrainMeshManagerSpec>().TerrainTilePrefab.Asset;
		_terrainService.TerrainHeightChanged += OnTerrainHeightChanged;
		_surfaceShapeCodes = new byte[_terrainService.Size.x + 2, _terrainService.Size.y + 2, _terrainService.Size.z + 3];
		InitializeTerrainTiles();
		Shader.SetGlobalFloat(TerrainStumpHeightProperty, TerrainStumpHeight);
	}

	public void LateUpdateSingleton()
	{
		if (_dirtyCodes.IsEmpty())
		{
			return;
		}
		foreach (Vector3Int dirtyCode in _dirtyCodes)
		{
			UpdateCodeForCoords(dirtyCode);
		}
		_dirtyCodes.Clear();
		foreach (Vector3Int invalidTile in _invalidTiles)
		{
			UpdateTile(invalidTile);
		}
		_invalidTiles.Clear();
	}

	public void ToggleVisibilityForDebugging()
	{
		_root.SetActive(!_root.activeSelf);
	}

	private void InitializeTerrainTiles()
	{
		Vector3Int size = _terrainService.Size;
		Vector3Int vector3Int = WorldTiling.TileCount3D(size.x, size.y, _mapSize.MaxGameTerrainHeight + 1);
		for (int i = -2; i < size.z + 1; i++)
		{
			for (int j = -1; j < size.y + 1; j++)
			{
				for (int k = -1; k < size.x + 1; k++)
				{
					UpdateCodeForCoords(new Vector3Int(k, j, i));
				}
			}
		}
		for (int l = 0; l < vector3Int.z; l++)
		{
			for (int m = 0; m < vector3Int.y; m++)
			{
				for (int n = 0; n < vector3Int.x; n++)
				{
					InstantiateTile(new Vector3Int(n, m, l));
				}
			}
		}
		for (int num = 0; num < vector3Int.z; num++)
		{
			for (int num2 = 0; num2 < vector3Int.y; num2++)
			{
				for (int num3 = 0; num3 < vector3Int.x; num3++)
				{
					UpdateTile(new Vector3Int(num3, num2, num));
				}
			}
		}
	}

	private void InstantiateTile(Vector3Int tileIndex)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(_terrainTilePrefab, _root.transform);
		gameObject.name = TileName(tileIndex);
		_tiles[tileIndex] = new TileComponents(gameObject);
	}

	private void UpdateTile(Vector3Int tileIndex)
	{
		_meshBuilder.Reset(TileName(tileIndex));
		CollectMeshesForTile(tileIndex);
		UpdateTileMesh(tileIndex);
	}

	private void CollectMeshesForTile(Vector3Int tileIndex)
	{
		TileBounds3D tileBounds3D = WorldTiling.TileBounds3D(tileIndex);
		int x = ((tileBounds3D.MinX > 0) ? tileBounds3D.MinX : (-1));
		int num = Math.Min(tileBounds3D.MaxX, _terrainService.Size.x);
		int y = ((tileBounds3D.MinY > 0) ? tileBounds3D.MinY : (-1));
		int num2 = Math.Min(tileBounds3D.MaxY, _terrainService.Size.y);
		int minZ = tileBounds3D.MinZ;
		int num3 = Math.Min(tileBounds3D.MaxZ, _terrainService.Size.z);
		int tileMinZ = minZ - 1;
		int tileMaxZ = num3 + 1;
		Vector3Int vector3Int = new Vector3Int
		{
			y = y
		};
		while (vector3Int.y < num2)
		{
			vector3Int.x = x;
			int z;
			while (vector3Int.x < num)
			{
				if (HasAnyColumnInside(vector3Int.XY(), tileMinZ, tileMaxZ))
				{
					vector3Int.z = minZ;
					while (vector3Int.z < num3)
					{
						CollectMeshesForCoordinates(vector3Int);
						z = vector3Int.z + 1;
						vector3Int.z = z;
					}
				}
				z = vector3Int.x + 1;
				vector3Int.x = z;
			}
			z = vector3Int.y + 1;
			vector3Int.y = z;
		}
	}

	private bool HasAnyColumnInside(Vector2Int coords, int tileMinZ, int tileMaxZ)
	{
		int terrainCount = 0;
		Vector3Int[] neighborOffsets = NeighborOffsets;
		foreach (Vector3Int value in neighborOffsets)
		{
			Vector2Int coords2 = coords + value.XY();
			if (HasTerrainInColumn(coords2, tileMinZ, tileMaxZ, ref terrainCount))
			{
				return true;
			}
		}
		return !HasColumnsWithOnlyTerrainOrAir(terrainCount);
	}

	private void CollectMeshesForCoordinates(Vector3Int coordinates)
	{
		SurfaceBlockShape shape = ExpectedShape(coordinates);
		if (shape.IsVisible)
		{
			IntermediateMesh terrainBlock = GetTerrainBlock(coordinates, shape);
			if (terrainBlock.UV1 == null || terrainBlock.UV1.Length != terrainBlock.VertexCount)
			{
				terrainBlock.UV1 = new Vector4[terrainBlock.VertexCount];
			}
			for (int i = 0; i < terrainBlock.VertexCount; i++)
			{
				Vector4 vector = terrainBlock.UV1[i];
				terrainBlock.UV1[i] = new Vector4(vector.x, coordinates.z, vector.z, vector.w);
			}
			Vector3 translation = CoordinateSystem.GridToWorld(coordinates) + PrefabTranslationOffset;
			TranslationTransform fittingTransform = new TranslationTransform(translation);
			CollectMeshesFromModel(terrainBlock, fittingTransform);
		}
	}

	private bool HasTerrainInColumn(Vector2Int coords, int tileMinZ, int tileMaxZ, ref int terrainCount)
	{
		int num = _mapIndexService.CellToIndex(coords);
		int columnCount = _terrainService.GetColumnCount(num);
		for (int i = 0; i < columnCount; i++)
		{
			int index3D = num + i * _mapIndexService.VerticalStride;
			int columnFloor = _terrainService.GetColumnFloor(index3D);
			int columnCeiling = _terrainService.GetColumnCeiling(index3D);
			if ((columnFloor >= tileMinZ && columnFloor <= tileMaxZ) || (columnCeiling >= tileMinZ && columnCeiling <= tileMaxZ))
			{
				return true;
			}
			if (columnFloor <= tileMinZ && columnCeiling >= tileMaxZ)
			{
				terrainCount++;
				return false;
			}
		}
		return false;
	}

	private static bool HasColumnsWithOnlyTerrainOrAir(int numberOfColumnsWithTerrainOnly)
	{
		if (numberOfColumnsWithTerrainOnly == 0 || numberOfColumnsWithTerrainOnly == 4)
		{
			return true;
		}
		return false;
	}

	private void CollectMeshesFromModel(IntermediateMesh intermediateMesh, TranslationTransform fittingTransform)
	{
		_meshBuilder.AppendIntermediateMesh(intermediateMesh, fittingTransform);
	}

	private void UpdateTileMesh(Vector3Int tileIndex)
	{
		TileComponents tileComponents = _tiles[tileIndex];
		if (!_meshBuilder.IsEmpty)
		{
			tileComponents.UpdateMesh(_meshBuilder.Build());
		}
		else
		{
			tileComponents.Deactivate();
		}
	}

	private IntermediateMesh GetTerrainBlock(Vector3Int coordinates, SurfaceBlockShape shape)
	{
		ImmutableArray<IntermediateMesh> terrainBlocks = _terrainBlockRepository.GetTerrainBlocks(shape);
		return _terrainBlockRandomizer.PickVariation(terrainBlocks, shape, coordinates);
	}

	private SurfaceBlockShape ExpectedShape(Vector3Int coords)
	{
		byte num = _surfaceShapeCodes[coords.x + 1, coords.y + 1, coords.z + 2];
		RelativeHeight height = (RelativeHeight)(num & 3);
		RelativeHeight height2 = (RelativeHeight)((num >> 2) & 3);
		RelativeHeight height3 = (RelativeHeight)((num >> 4) & 3);
		return new SurfaceBlockShape((RelativeHeight)((num >> 6) & 3), height3, height, height2);
	}

	private void OnTerrainHeightChanged(object sender, TerrainHeightChangeEventArgs terrainHeightChangeEventArgs)
	{
		TerrainHeightChange change = terrainHeightChangeEventArgs.Change;
		Vector2Int coordinates = change.Coordinates;
		for (int i = change.From; i <= Mathf.Max(change.To + 1, _terrainService.Size.z - 1); i++)
		{
			Vector3Int[] neighborOffsets = NeighborOffsets;
			for (int j = 0; j < neighborOffsets.Length; j++)
			{
				Vector3Int vector3Int = neighborOffsets[j];
				_dirtyCodes.Add(new Vector3Int(coordinates.x - vector3Int.x, coordinates.y - vector3Int.y, i));
			}
		}
		Vector2Int[] neighbors8AndSelfVector2Int = Deltas.Neighbors8AndSelfVector2Int;
		foreach (Vector2Int vector2Int in neighbors8AndSelfVector2Int)
		{
			Vector2Int coordinates2 = coordinates + vector2Int;
			if (_terrainService.Contains(coordinates2))
			{
				Vector3Int vector3Int2 = WorldTiling.CoordinatesToTileIndex3D(new Vector3Int(coordinates2.x, coordinates2.y, change.From));
				Vector3Int vector3Int3 = WorldTiling.CoordinatesToTileIndex3D(new Vector3Int(coordinates2.x, coordinates2.y, change.To + 2));
				if (vector3Int2.XY() != vector3Int3.XY())
				{
					throw new Exception($"Unexpected tile indices: {vector3Int2} and {vector3Int3}." + " This should never happen.");
				}
				int num = Math.Min(vector3Int2.z, vector3Int3.z);
				int num2 = Math.Max(vector3Int2.z, vector3Int3.z);
				for (int k = num; k <= num2; k++)
				{
					_invalidTiles.Add(new Vector3Int(vector3Int2.x, vector3Int2.y, k));
				}
			}
		}
	}

	private void UpdateCodeForCoords(Vector3Int coords)
	{
		byte b = 0;
		for (int i = 0; i < NeighborOffsets.Length; i++)
		{
			Vector3Int vector3Int = NeighborOffsets[i];
			Vector3Int vector3Int2 = coords + vector3Int;
			bool flag = _terrainService.Contains(vector3Int2.XY());
			b |= (byte)((flag && _terrainService.Underground(vector3Int2.Below())) ? (1 << i * 2) : 0);
			b |= (byte)((flag && _terrainService.Underground(vector3Int2)) ? (1 << i * 2 + 1) : 0);
		}
		_surfaceShapeCodes[coords.x + 1, coords.y + 1, coords.z + 2] = b;
	}

	private static string TileName(Vector3Int tileIndex)
	{
		return $"TerrainTile ({tileIndex.x}, {tileIndex.y}, {tileIndex.z})";
	}
}
