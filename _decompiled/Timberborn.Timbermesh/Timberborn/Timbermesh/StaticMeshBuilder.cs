using System.Collections.Generic;
using System.Linq;
using Timberborn.TimbermeshDTO;
using UnityEngine;
using UnityEngine.Rendering;

namespace Timberborn.Timbermesh;

public class StaticMeshBuilder
{
	private static readonly int VertexLimitFor16BitIndexBuffer = 65535;

	private readonly IMaterialRepository _materialRepository;

	private readonly List<Vector2> _vector2ImportCache = new List<Vector2>();

	private readonly List<Vector3> _vector3ImportCache = new List<Vector3>();

	private readonly List<Vector4> _vector4ImportCache = new List<Vector4>();

	private readonly List<Color> _colorImportCache = new List<Color>();

	private readonly List<Material> _materialsToSetCache = new List<Material>();

	public StaticMeshBuilder(IMaterialRepository materialRepository)
	{
		_materialRepository = materialRepository;
	}

	public void BuildMesh(GameObject meshContainer, Node node)
	{
		UnityEngine.Mesh mesh = BuildMesh(node);
		if ((bool)mesh)
		{
			SetMaterials(node);
			CreateMeshComponents(mesh, meshContainer);
		}
	}

	private UnityEngine.Mesh BuildMesh(Node node)
	{
		int vertexCount = node.VertexCount;
		if (vertexCount > 0)
		{
			UnityEngine.Mesh mesh = new UnityEngine.Mesh
			{
				name = node.Name,
				indexFormat = ((vertexCount > VertexLimitFor16BitIndexBuffer) ? IndexFormat.UInt32 : IndexFormat.UInt16)
			};
			SetVertices(mesh, node);
			SetNormals(mesh, node);
			SetTangents(mesh, node);
			SetColors(mesh, node);
			SetUV(mesh, node, 0);
			SetUV(mesh, node, 1);
			SetUV(mesh, node, 2);
			SetSubmeshes(mesh, node);
			OptimizeMesh(mesh, node);
			return mesh;
		}
		return null;
	}

	private void SetVertices(UnityEngine.Mesh mesh, Node node)
	{
		node.ReadProperties("position", _vector3ImportCache);
		if (_vector3ImportCache.Any())
		{
			mesh.SetVertices(_vector3ImportCache);
		}
		_vector3ImportCache.Clear();
	}

	private void SetNormals(UnityEngine.Mesh mesh, Node node)
	{
		node.ReadProperties("normal", _vector3ImportCache);
		if (_vector3ImportCache.Any())
		{
			mesh.SetNormals(_vector3ImportCache);
		}
		_vector3ImportCache.Clear();
	}

	private void SetTangents(UnityEngine.Mesh mesh, Node node)
	{
		node.ReadProperties("tangent", _vector4ImportCache);
		if (_vector4ImportCache.Any())
		{
			mesh.SetTangents(_vector4ImportCache);
		}
		_vector4ImportCache.Clear();
	}

	private void SetColors(UnityEngine.Mesh mesh, Node node)
	{
		node.ReadProperties("color", _colorImportCache);
		if (_colorImportCache.Any())
		{
			mesh.SetColors(_colorImportCache);
		}
		_colorImportCache.Clear();
	}

	private void SetUV(UnityEngine.Mesh mesh, Node node, int channel)
	{
		node.ReadProperties($"uv{channel}", _vector2ImportCache);
		if (_vector2ImportCache.Any())
		{
			mesh.SetUVs(channel, _vector2ImportCache);
		}
		_vector2ImportCache.Clear();
	}

	private static void SetSubmeshes(UnityEngine.Mesh mesh, Node node)
	{
		mesh.subMeshCount = node.Meshes.Count;
		for (int i = 0; i < node.Meshes.Count; i++)
		{
			Timberborn.TimbermeshDTO.Mesh mesh2 = node.Meshes[i];
			mesh.SetIndices(mesh2.Indices, MeshTopology.Triangles, i);
		}
	}

	private void SetMaterials(Node node)
	{
		for (int i = 0; i < node.Meshes.Count; i++)
		{
			Timberborn.TimbermeshDTO.Mesh mesh = node.Meshes[i];
			Material material = _materialRepository.GetMaterial(mesh.Material);
			_materialsToSetCache.Add(material);
		}
	}

	private static void OptimizeMesh(UnityEngine.Mesh mesh, Node node)
	{
		if (!node.VertexAnimations.Any())
		{
			mesh.Optimize();
		}
		mesh.RecalculateBounds();
	}

	private void CreateMeshComponents(UnityEngine.Mesh mesh, GameObject meshContainer)
	{
		meshContainer.AddComponent<MeshFilter>().sharedMesh = mesh;
		meshContainer.AddComponent<MeshRenderer>().sharedMaterials = _materialsToSetCache.ToArray();
		_materialsToSetCache.Clear();
	}
}
