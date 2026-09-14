using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.PrefabOptimization;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.Rendering;

public class AreaTileDrawer
{
	private static readonly float YOffset = 0.02f;

	private IntermediateMesh _intermediateMesh;

	private readonly Material[] _materials;

	private readonly Vector2Int _tileCount;

	private readonly GameObject _parent;

	private readonly Dictionary<Vector2Int, GameObject> _tiles = new Dictionary<Vector2Int, GameObject>();

	private readonly Dictionary<Vector2Int, MeshBuilder> _meshBuilders = new Dictionary<Vector2Int, MeshBuilder>();

	public AreaTileDrawer(Mesh mesh, Material material, Vector2Int tileCount, GameObject parent)
	{
		_materials = new Material[1] { material };
		_tileCount = tileCount;
		_parent = parent;
		Initialize(mesh);
	}

	public void HideAllTiles()
	{
		_parent.SetActive(value: false);
	}

	public void ShowAllTiles()
	{
		_parent.SetActive(value: true);
	}

	public void UpdateArea(IEnumerable<Vector3Int> coordinates)
	{
		Clear();
		UpdateMeshBuilders(coordinates);
		UpdateTiles();
	}

	private void Initialize(Mesh mesh)
	{
		MeshBuilder meshBuilder = new MeshBuilder();
		for (int i = 0; i < _tileCount.y; i++)
		{
			for (int j = 0; j < _tileCount.x; j++)
			{
				Vector2Int key = new Vector2Int(j, i);
				InitializeMeshBuilder(key);
				InitializeTile(key, meshBuilder);
			}
		}
		meshBuilder.Reset("");
		meshBuilder.AppendMesh(mesh, _materials, default(TranslationTransform));
		_intermediateMesh = meshBuilder.BuildIntermediateMesh();
	}

	private void InitializeMeshBuilder(Vector2Int key)
	{
		_meshBuilders.Add(key, new MeshBuilder());
	}

	private void InitializeTile(Vector2Int key, MeshBuilder meshBuilder)
	{
		GameObject gameObject = new GameObject(key.ToString());
		gameObject.transform.parent = _parent.transform;
		Mesh mesh = meshBuilder.Build().Mesh;
		mesh.MarkDynamic();
		gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
		gameObject.AddComponent<MeshRenderer>().sharedMaterials = _materials;
		_tiles.Add(key, gameObject);
	}

	private void Clear()
	{
		foreach (var (vector2Int2, meshBuilder2) in _meshBuilders)
		{
			meshBuilder2.Reset(vector2Int2.ToString());
		}
		foreach (GameObject value in _tiles.Values)
		{
			value.SetActive(value: false);
		}
	}

	private void UpdateMeshBuilders(IEnumerable<Vector3Int> coordinates)
	{
		foreach (Vector3Int coordinate in coordinates)
		{
			Vector3 translation = CoordinateSystem.GridToWorldCentered(coordinate) + new Vector3(0f, YOffset, 0f);
			TranslationTransform transform = new TranslationTransform(translation);
			Vector2Int key = WorldTiling.CoordinatesToTileIndex2D(coordinate.XY());
			_meshBuilders[key].AppendIntermediateMesh(_intermediateMesh, transform);
		}
	}

	private void UpdateTiles()
	{
		foreach (Vector2Int key in _meshBuilders.Keys)
		{
			MeshBuilder meshBuilder = _meshBuilders[key];
			if (!meshBuilder.IsEmpty)
			{
				GameObject gameObject = _tiles[key];
				meshBuilder.Build(gameObject.GetComponent<MeshFilter>().sharedMesh);
				gameObject.SetActive(value: true);
			}
		}
	}
}
