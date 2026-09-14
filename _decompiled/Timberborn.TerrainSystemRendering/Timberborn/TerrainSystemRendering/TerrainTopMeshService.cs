using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.LevelVisibilitySystem;
using Timberborn.PrefabOptimization;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using UnityEngine;
using UnityEngine.Rendering;

namespace Timberborn.TerrainSystemRendering;

internal class TerrainTopMeshService : ILoadableSingleton
{
	private static readonly int VertexLimitFor16BitIndexBuffer = 65535;

	private readonly ITerrainService _terrainService;

	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly ISpecService _specService;

	private readonly EventBus _eventBus;

	private readonly MeshBuilder _meshBuilder = new MeshBuilder("TerrainTopMesh");

	private GameObject _topLayerObject;

	public TerrainTopMeshService(ITerrainService terrainService, ILevelVisibilityService levelVisibilityService, ISpecService specService, EventBus eventBus)
	{
		_terrainService = terrainService;
		_levelVisibilityService = levelVisibilityService;
		_specService = specService;
		_eventBus = eventBus;
	}

	public void Load()
	{
		InitializeTopLayerObject();
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnMaxVisibleTerrainLevelChanged(MaxVisibleLevelChangedEvent e)
	{
		_topLayerObject.SetActive(!_levelVisibilityService.TerrainLevelIsAtMax);
	}

	private void InitializeTopLayerObject()
	{
		AssetRef<GameObject> layerToolTopMeshPrefab = _specService.GetSingleSpec<TerrainMeshManagerSpec>().LayerToolTopMeshPrefab;
		_topLayerObject = Object.Instantiate(layerToolTopMeshPrefab.Asset);
		_topLayerObject.transform.position = Vector3.zero;
		_topLayerObject.gameObject.SetActive(value: false);
		Mesh mesh = GenerateTopMesh(_terrainService.Size.XY());
		Vector3 vector = new Vector3(_terrainService.Size.x, 0f, _terrainService.Size.y);
		mesh.bounds = new Bounds(vector / 2f, vector);
		_topLayerObject.GetComponent<MeshFilter>().sharedMesh = mesh;
	}

	private Mesh GenerateTopMesh(Vector2Int size)
	{
		Mesh mesh = new Mesh();
		int num = size.x * size.y;
		int num2 = num * 4;
		mesh.indexFormat = ((num2 > VertexLimitFor16BitIndexBuffer) ? IndexFormat.UInt32 : IndexFormat.UInt16);
		IntermediateMesh intermediateMesh = new IntermediateMesh();
		intermediateMesh.VertexCount = num2;
		intermediateMesh.Vertices = new Vector3[num2];
		intermediateMesh.Normals = new Vector3[num2];
		intermediateMesh.UV0 = new Vector4[num2];
		intermediateMesh.Submeshes = new(NullableKey<Material>, int[])[1] { (default(NullableKey<Material>), new int[num * 6]) };
		GenerateVertices(size, intermediateMesh);
		GenerateIndices(size, intermediateMesh);
		TranslationTransform transform = new TranslationTransform(Vector3.zero);
		_meshBuilder.AppendIntermediateMesh(intermediateMesh, transform);
		_meshBuilder.Build(mesh);
		mesh.RecalculateBounds();
		return mesh;
	}

	private static void GenerateVertices(Vector2Int size, IntermediateMesh intermediateMesh)
	{
		Vector3[] vertices = intermediateMesh.Vertices;
		Vector3[] normals = intermediateMesh.Normals;
		Vector4[] uV = intermediateMesh.UV0;
		for (int i = 0; i < size.y; i++)
		{
			int num = i * size.x * 4;
			for (int j = 0; j < size.x; j++)
			{
				int num2 = j * 4 + num;
				int num3 = j * 4 + num + 1;
				int num4 = j * 4 + num + 2;
				int num5 = j * 4 + num + 3;
				vertices[num2] = new Vector3(j, 0f, i);
				vertices[num3] = new Vector3(j + 1, 0f, i);
				vertices[num4] = new Vector3(j + 1, 0f, i + 1);
				vertices[num5] = new Vector3(j, 0f, i + 1);
				normals[num2] = Vector3.up;
				normals[num3] = Vector3.up;
				normals[num4] = Vector3.up;
				normals[num5] = Vector3.up;
				Vector2 vector = new Vector2((float)j + 0.5f, (float)i + 0.5f);
				uV[num2] = vector;
				uV[num3] = vector;
				uV[num4] = vector;
				uV[num5] = vector;
			}
		}
	}

	private static void GenerateIndices(Vector2Int size, IntermediateMesh intermediateMesh)
	{
		int[] item = intermediateMesh.Submeshes[0].Item2;
		for (int i = 0; i < size.y; i++)
		{
			int num = i * size.x * 4;
			int num2 = i * size.x * 6;
			for (int j = 0; j < size.x; j++)
			{
				int num3 = j * 4 + num;
				int num4 = j * 6 + num2;
				int num5 = num3 + 1;
				int num6 = num3 + 2;
				int num7 = num3 + 3;
				item[num4] = num3;
				item[num4 + 1] = num5;
				item[num4 + 2] = num6;
				item[num4 + 3] = num3;
				item[num4 + 4] = num6;
				item[num4 + 5] = num7;
			}
		}
	}
}
