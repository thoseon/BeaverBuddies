using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.MapStateSystem;
using Timberborn.PrefabOptimization;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using UnityEngine;
using UnityEngine.Rendering;

namespace Timberborn.WaterSystemRendering;

internal class WaterMesh : IWaterMesh, ILoadableSingleton, IUnloadableSingleton
{
	private static readonly float MapBorderMargin = 0.1f;

	private static readonly string WaterTileRootName = "WaterTiles";

	private static readonly int EdgeVertexBit = 1;

	private static readonly int CornerVertexBit = 2;

	private static readonly int SkirtBit = 4;

	private static readonly int LeftSkirtBit = 8;

	private static readonly int RightSkirtBit = 16;

	private static readonly int TopSkirtBit = 32;

	private static readonly int BottomSkirtBit = 64;

	private static readonly int FloorSkirtBit = 128;

	private readonly MapSize _mapSize;

	private readonly EventBus _eventBus;

	private readonly WaterOpacityService _waterOpacityService;

	private readonly ISpecService _specService;

	private readonly RootObjectProvider _rootObjectProvider;

	private readonly MeshBuilder _meshBuilder = new MeshBuilder();

	private GameObject _waterTiles;

	private IntermediateMesh _waterMesh;

	private readonly List<Mesh> _createdMeshes = new List<Mesh>();

	private readonly Dictionary<Vector3Int, MeshRenderer> _createdTiles = new Dictionary<Vector3Int, MeshRenderer>();

	private WaterMeshSpec _waterMeshSpec;

	public WaterMesh(MapSize mapSize, EventBus eventBus, WaterOpacityService waterOpacityService, ISpecService specService, RootObjectProvider rootObjectProvider)
	{
		_mapSize = mapSize;
		_eventBus = eventBus;
		_waterOpacityService = waterOpacityService;
		_specService = specService;
		_rootObjectProvider = rootObjectProvider;
	}

	public void Load()
	{
		_waterMeshSpec = _specService.GetSingleSpec<WaterMeshSpec>();
		_waterMesh = GetWaterMesh();
		_waterTiles = new GameObject(WaterTileRootName);
		GameObject gameObject = _rootObjectProvider.CreateRootObject("WaterMesh");
		_waterTiles.transform.SetParent(gameObject.transform);
		_eventBus.Register(this);
	}

	public void Unload()
	{
		_createdTiles.Clear();
		foreach (Mesh createdMesh in _createdMeshes)
		{
			if (createdMesh != null)
			{
				createdMesh.Clear();
				UnityEngine.Object.Destroy(createdMesh);
			}
		}
	}

	public void Show()
	{
		_waterTiles.SetActive(value: true);
	}

	public void Hide()
	{
		_waterTiles.SetActive(value: false);
	}

	public void EnableTile(Vector3Int tileIndex)
	{
		if (!_createdTiles.TryGetValue(tileIndex, out var value))
		{
			value = CreateTile(tileIndex);
		}
		value.enabled = true;
	}

	public void DisableAllTiles()
	{
		foreach (MeshRenderer value in _createdTiles.Values)
		{
			value.enabled = false;
		}
	}

	[OnEvent]
	public void OnWaterOpacityChanged(WaterOpacityChangedEvent waterOpacityChangedEvent)
	{
		UpdateMaterialTransparency();
	}

	private IntermediateMesh GetWaterMesh()
	{
		int num = 48;
		int[] item = new int[126]
		{
			1, 0, 4, 1, 4, 5, 5, 6, 1, 6,
			2, 1, 6, 7, 2, 7, 3, 2, 8, 9,
			4, 9, 5, 4, 9, 10, 5, 10, 6, 5,
			10, 11, 6, 11, 7, 6, 12, 13, 8, 13,
			9, 8, 13, 14, 9, 14, 10, 9, 11, 10,
			14, 11, 14, 15, 32, 16, 17, 32, 17, 33,
			33, 17, 18, 33, 18, 34, 34, 18, 35, 35,
			18, 19, 36, 21, 20, 36, 37, 21, 37, 22,
			21, 37, 38, 22, 38, 39, 22, 39, 23, 22,
			40, 24, 25, 40, 25, 41, 41, 25, 42, 42,
			25, 26, 42, 26, 43, 43, 26, 27, 47, 31,
			30, 47, 30, 46, 46, 30, 29, 46, 29, 45,
			45, 29, 44, 44, 29, 28
		};
		IntermediateMesh intermediateMesh = new IntermediateMesh();
		intermediateMesh.VertexCount = num;
		intermediateMesh.Vertices = new Vector3[num];
		intermediateMesh.Submeshes = new(NullableKey<Material>, int[])[1] { (new NullableKey<Material>(_waterMeshSpec.OpaqueMaterial.Asset), item) };
		intermediateMesh.UV0 = Enumerable.Repeat(default(Vector4), num).ToArray();
		return intermediateMesh;
	}

	private MeshRenderer CreateTile(Vector3Int index)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(_waterMeshSpec.WaterTile.Asset, _waterTiles.transform);
		Mesh mesh = BuildTileMesh(tileName: gameObject.name = $"WaterTile ({index.x}, {index.y}, {index.z})", index: index);
		Bounds bounds = mesh.bounds;
		bounds.Encapsulate(new Vector3(index.x, _mapSize.TotalSize.z, index.y));
		mesh.bounds = bounds;
		mesh.UploadMeshData(markNoLongerReadable: true);
		gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;
		MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
		component.shadowCastingMode = ShadowCastingMode.Off;
		component.sharedMaterial = GetWaterMaterial();
		_createdMeshes.Add(mesh);
		_createdTiles.Add(index, component);
		return component;
	}

	private Mesh BuildTileMesh(Vector3Int index, string tileName)
	{
		_meshBuilder.Reset(tileName);
		TileBounds2D tileBounds2D = TileBoundsLimitedToMap(index.XY());
		for (int i = tileBounds2D.MinX; i < tileBounds2D.MaxX; i++)
		{
			for (int j = tileBounds2D.MinY; j < tileBounds2D.MaxY; j++)
			{
				AppendWaterMesh(i, j, index.z);
			}
		}
		return _meshBuilder.Build(IndexFormat.UInt16).Mesh;
	}

	private TileBounds2D TileBoundsLimitedToMap(Vector2Int index)
	{
		TileBounds2D tileBounds2D = WorldTiling.TileBounds2D(index);
		Vector3Int terrainSize = _mapSize.TerrainSize;
		return new TileBounds2D(tileBounds2D.MinX, tileBounds2D.MinY, Math.Min(tileBounds2D.MaxX, terrainSize.x), Math.Min(tileBounds2D.MaxY, terrainSize.y));
	}

	private void AppendWaterMesh(int x, int y, int z)
	{
		UpdateWaterMesh(new Vector2Int(x, y), z);
		_meshBuilder.AppendIntermediateMesh(_waterMesh, new TranslationTransform(new Vector3(x, 0f, y)));
	}

	private void UpdateWaterMesh(Vector2Int tileCoordinates, int columnIndex)
	{
		_waterMesh.Vertices[0] = GetVertexPosition(tileCoordinates, 0f, 0f, columnIndex);
		_waterMesh.Vertices[1] = GetVertexPosition(tileCoordinates, 1f / 3f, 0f, columnIndex);
		_waterMesh.Vertices[2] = GetVertexPosition(tileCoordinates, 2f / 3f, 0f, columnIndex);
		_waterMesh.Vertices[3] = GetVertexPosition(tileCoordinates, 1f, 0f, columnIndex);
		_waterMesh.Vertices[4] = GetVertexPosition(tileCoordinates, 0f, 1f / 3f, columnIndex);
		_waterMesh.Vertices[5] = GetVertexPosition(tileCoordinates, 1f / 3f, 1f / 3f, columnIndex);
		_waterMesh.Vertices[6] = GetVertexPosition(tileCoordinates, 2f / 3f, 1f / 3f, columnIndex);
		_waterMesh.Vertices[7] = GetVertexPosition(tileCoordinates, 1f, 1f / 3f, columnIndex);
		_waterMesh.Vertices[8] = GetVertexPosition(tileCoordinates, 0f, 2f / 3f, columnIndex);
		_waterMesh.Vertices[9] = GetVertexPosition(tileCoordinates, 1f / 3f, 2f / 3f, columnIndex);
		_waterMesh.Vertices[10] = GetVertexPosition(tileCoordinates, 2f / 3f, 2f / 3f, columnIndex);
		_waterMesh.Vertices[11] = GetVertexPosition(tileCoordinates, 1f, 2f / 3f, columnIndex);
		_waterMesh.Vertices[12] = GetVertexPosition(tileCoordinates, 0f, 1f, columnIndex);
		_waterMesh.Vertices[13] = GetVertexPosition(tileCoordinates, 1f / 3f, 1f, columnIndex);
		_waterMesh.Vertices[14] = GetVertexPosition(tileCoordinates, 2f / 3f, 1f, columnIndex);
		_waterMesh.Vertices[15] = GetVertexPosition(tileCoordinates, 1f, 1f, columnIndex);
		_waterMesh.Vertices[16] = (_waterMesh.Vertices[32] = _waterMesh.Vertices[0]);
		_waterMesh.Vertices[17] = (_waterMesh.Vertices[33] = _waterMesh.Vertices[1]);
		_waterMesh.Vertices[18] = (_waterMesh.Vertices[34] = _waterMesh.Vertices[2]);
		_waterMesh.Vertices[19] = (_waterMesh.Vertices[35] = _waterMesh.Vertices[3]);
		_waterMesh.Vertices[20] = (_waterMesh.Vertices[36] = _waterMesh.Vertices[0]);
		_waterMesh.Vertices[21] = (_waterMesh.Vertices[37] = _waterMesh.Vertices[4]);
		_waterMesh.Vertices[22] = (_waterMesh.Vertices[38] = _waterMesh.Vertices[8]);
		_waterMesh.Vertices[23] = (_waterMesh.Vertices[39] = _waterMesh.Vertices[12]);
		_waterMesh.Vertices[24] = (_waterMesh.Vertices[40] = _waterMesh.Vertices[3]);
		_waterMesh.Vertices[25] = (_waterMesh.Vertices[41] = _waterMesh.Vertices[7]);
		_waterMesh.Vertices[26] = (_waterMesh.Vertices[42] = _waterMesh.Vertices[11]);
		_waterMesh.Vertices[27] = (_waterMesh.Vertices[43] = _waterMesh.Vertices[15]);
		_waterMesh.Vertices[28] = (_waterMesh.Vertices[44] = _waterMesh.Vertices[12]);
		_waterMesh.Vertices[29] = (_waterMesh.Vertices[45] = _waterMesh.Vertices[13]);
		_waterMesh.Vertices[30] = (_waterMesh.Vertices[46] = _waterMesh.Vertices[14]);
		_waterMesh.Vertices[31] = (_waterMesh.Vertices[47] = _waterMesh.Vertices[15]);
		SetUV0(0, 15, tileCoordinates, EdgeVertexBit);
		SetUV0(5, 6, tileCoordinates, 0);
		SetUV0(9, 10, tileCoordinates, 0);
		SetUV0(16, 19, tileCoordinates, EdgeVertexBit | SkirtBit | BottomSkirtBit);
		SetUV0(32, 35, tileCoordinates, EdgeVertexBit | SkirtBit | BottomSkirtBit | FloorSkirtBit);
		SetUV0(20, 23, tileCoordinates, EdgeVertexBit | SkirtBit | LeftSkirtBit);
		SetUV0(36, 39, tileCoordinates, EdgeVertexBit | SkirtBit | LeftSkirtBit | FloorSkirtBit);
		SetUV0(24, 27, tileCoordinates, EdgeVertexBit | SkirtBit | RightSkirtBit);
		SetUV0(40, 43, tileCoordinates, EdgeVertexBit | SkirtBit | RightSkirtBit | FloorSkirtBit);
		SetUV0(28, 31, tileCoordinates, EdgeVertexBit | SkirtBit | TopSkirtBit);
		SetUV0(44, 47, tileCoordinates, EdgeVertexBit | SkirtBit | TopSkirtBit | FloorSkirtBit);
		AppendToUV0Mask(new int[21]
		{
			0, 3, 12, 15, 20, 16, 19, 20, 23, 24,
			27, 28, 31, 32, 35, 36, 39, 40, 43, 44,
			47
		}, CornerVertexBit);
	}

	private Vector3 GetVertexPosition(Vector2Int tileCoordinates, float vertexX, float vertexY, int columnIndex)
	{
		return new Vector3(TrimCoordinate(vertexX, tileCoordinates.x, _mapSize.TotalSize.x), columnIndex, TrimCoordinate(vertexY, tileCoordinates.y, _mapSize.TotalSize.y));
	}

	private static float TrimCoordinate(float coordinate, float tilePosition, float mapSize)
	{
		if (tilePosition + coordinate < MapBorderMargin)
		{
			return MapBorderMargin;
		}
		if (tilePosition + coordinate > mapSize - MapBorderMargin)
		{
			return 1f - MapBorderMargin;
		}
		return coordinate;
	}

	private void SetUV0(int startIndex, int endIndex, Vector2Int tileCoordinates, int mask)
	{
		for (int i = startIndex; i <= endIndex; i++)
		{
			_waterMesh.UV0[i] = new Vector4(tileCoordinates.x, tileCoordinates.y, i, mask);
		}
	}

	private void AppendToUV0Mask(IReadOnlyList<int> indices, int bit)
	{
		for (int i = 0; i < indices.Count; i++)
		{
			Vector4 vector = _waterMesh.UV0[indices[i]];
			int num = (int)vector.w;
			num |= bit;
			_waterMesh.UV0[indices[i]] = new Vector4(vector.x, vector.y, vector.z, num);
		}
	}

	private void UpdateMaterialTransparency()
	{
		Material waterMaterial = GetWaterMaterial();
		foreach (MeshRenderer value in _createdTiles.Values)
		{
			value.sharedMaterial = waterMaterial;
		}
	}

	private Material GetWaterMaterial()
	{
		if (!_waterOpacityService.IsWaterTransparent)
		{
			return _waterMeshSpec.OpaqueMaterial.Asset;
		}
		return _waterMeshSpec.TransparentMaterial.Asset;
	}
}
