using System;
using System.Collections.Generic;
using Timberborn.Common;
using UnityEngine;
using UnityEngine.Rendering;

namespace Timberborn.PrefabOptimization;

public class MeshBuilder
{
	private static readonly int InitialCapacity = 6000;

	private static readonly int CapacityGrowthRate = 2;

	private static readonly Color32 White = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	private string _name;

	private int _vertexCount;

	private int _vertexCapacity;

	private Vector3[] _vertices;

	private Vector3[] _normals;

	private Vector4[] _tangents;

	private Color32[] _colors;

	private Vector4[] _uv0;

	private Vector4[] _uv1;

	private Vector4[] _uv2;

	private Dictionary<NullableKey<Material>, List<int>> _indices;

	private readonly List<Vector4> _uv0Cache = new List<Vector4>();

	private readonly List<Vector4> _uv1Cache = new List<Vector4>();

	private readonly List<Vector4> _uv2Cache = new List<Vector4>();

	public bool IsEmpty => _vertexCount == 0;

	public MeshBuilder()
	{
		_name = "";
	}

	public MeshBuilder(string name)
	{
		_name = name;
	}

	public void Reset(string name)
	{
		_name = name;
		_vertexCount = 0;
		if (_indices == null)
		{
			return;
		}
		foreach (List<int> value in _indices.Values)
		{
			value.Clear();
		}
	}

	public void AppendMesh<TTransform>(BuiltMesh meshAndMaterials, TTransform transform) where TTransform : ITransform
	{
		AppendMesh(meshAndMaterials.Mesh, meshAndMaterials.Materials, transform);
	}

	public void AppendMesh<TTransform>(Mesh sourceMesh, Material[] materials, TTransform transform) where TTransform : ITransform
	{
		int vertexCount = _vertexCount;
		sourceMesh.GetUVs(0, _uv0Cache);
		sourceMesh.GetUVs(1, _uv1Cache);
		sourceMesh.GetUVs(2, _uv2Cache);
		AppendVertexData(transform, sourceMesh.vertexCount, sourceMesh.vertices, sourceMesh.normals, sourceMesh.tangents, sourceMesh.colors32, _uv0Cache.ToArray(), _uv1Cache.ToArray(), _uv2Cache.ToArray());
		_uv0Cache.Clear();
		_uv1Cache.Clear();
		_uv2Cache.Clear();
		AppendIndices(sourceMesh, materials, vertexCount);
	}

	public void AppendIntermediateMesh<TTransform>(IntermediateMesh sourceMesh, TTransform transform) where TTransform : ITransform
	{
		int vertexCount = _vertexCount;
		AppendVertexData(transform, sourceMesh.VertexCount, sourceMesh.Vertices, sourceMesh.Normals, sourceMesh.Tangents, sourceMesh.Colors, sourceMesh.UV0, sourceMesh.UV1, sourceMesh.UV2);
		AppendIndices(sourceMesh, vertexCount);
	}

	public BuiltMesh Build(IndexFormat indexFormat = IndexFormat.UInt32)
	{
		Mesh mesh = new Mesh
		{
			name = _name,
			indexFormat = indexFormat
		};
		BuildInternal(out var materials, mesh);
		return new BuiltMesh(mesh, materials);
	}

	public void Build(Mesh mesh)
	{
		mesh.Clear();
		BuildInternal(out var _, mesh);
	}

	public IntermediateMesh BuildIntermediateMesh()
	{
		(NullableKey<Material>, int[])[] array = new(NullableKey<Material>, int[])[CountNonEmptySubmeshes()];
		int num = 0;
		foreach (KeyValuePair<NullableKey<Material>, List<int>> index in _indices)
		{
			NullableKey<Material> key = index.Key;
			List<int> value = index.Value;
			if (!value.IsEmpty())
			{
				array[num] = (key, value.ToArray());
				num++;
			}
		}
		return new IntermediateMesh
		{
			VertexCount = _vertexCount,
			Vertices = CloneArrayOrEmpty(_vertices, _vertexCount),
			Normals = CloneArrayOrEmpty(_normals, _vertexCount),
			Tangents = CloneArrayOrEmpty(_tangents, _vertexCount),
			Colors = CloneArrayOrEmpty(_colors, _vertexCount),
			UV0 = CloneArrayOrEmpty(_uv0, _vertexCount),
			UV1 = CloneArrayOrEmpty(_uv1, _vertexCount),
			UV2 = CloneArrayOrEmpty(_uv2, _vertexCount),
			Submeshes = array
		};
	}

	private void AppendVertexData<TTransform>(TTransform transform, int sourceVertexCount, Vector3[] sourceVertices, Vector3[] sourceNormals, Vector4[] sourceTangents, Color32[] sourceColors, Vector4[] sourceUV0, Vector4[] sourceUV1, Vector4[] sourceUV2) where TTransform : ITransform
	{
		int num = _vertexCount + sourceVertexCount;
		if (_vertexCapacity < num)
		{
			_vertexCapacity = ((_vertexCapacity == 0) ? Math.Max(InitialCapacity, num) : Math.Max(_vertexCapacity * CapacityGrowthRate, num));
			CreateOrResize(ref _vertices, _vertexCapacity);
		}
		if (!_normals.IsNullOrEmpty() || !sourceNormals.IsNullOrEmpty())
		{
			CreateOrResize(ref _normals, _vertexCapacity, Vector3.up);
		}
		if (!_tangents.IsNullOrEmpty() || !sourceTangents.IsNullOrEmpty())
		{
			CreateOrResize(ref _tangents, _vertexCapacity);
		}
		if (!_colors.IsNullOrEmpty() || !sourceColors.IsNullOrEmpty())
		{
			CreateOrResize(ref _colors, _vertexCapacity, White);
		}
		if (!_uv0.IsNullOrEmpty() || !sourceUV0.IsNullOrEmpty())
		{
			CreateOrResize(ref _uv0, _vertexCapacity);
		}
		if (!_uv1.IsNullOrEmpty() || !sourceUV1.IsNullOrEmpty())
		{
			CreateOrResize(ref _uv1, _vertexCapacity);
		}
		if (!_uv2.IsNullOrEmpty() || !sourceUV2.IsNullOrEmpty())
		{
			CreateOrResize(ref _uv2, _vertexCapacity);
		}
		int vertexCount = _vertexCount;
		Vector3[] vertices = _vertices;
		transform.MultiplyPoints(sourceVertices, vertices, vertexCount, sourceVertexCount);
		if (!sourceNormals.IsNullOrEmpty())
		{
			Vector3[] normals = _normals;
			transform.MultiplyNormals(sourceNormals, normals, vertexCount, sourceVertexCount);
		}
		if (!sourceTangents.IsNullOrEmpty())
		{
			Vector4[] tangents = _tangents;
			transform.MultiplyTangents(sourceTangents, tangents, vertexCount, sourceVertexCount);
		}
		if (!sourceColors.IsNullOrEmpty())
		{
			Array.Copy(sourceColors, 0, _colors, vertexCount, sourceVertexCount);
		}
		if (!sourceUV0.IsNullOrEmpty())
		{
			Array.Copy(sourceUV0, 0, _uv0, vertexCount, sourceVertexCount);
		}
		if (!sourceUV1.IsNullOrEmpty())
		{
			Array.Copy(sourceUV1, 0, _uv1, vertexCount, sourceVertexCount);
		}
		if (!sourceUV2.IsNullOrEmpty())
		{
			Array.Copy(sourceUV2, 0, _uv2, vertexCount, sourceVertexCount);
		}
		_vertexCount += sourceVertexCount;
	}

	private void AppendIndices(Mesh sourceMesh, Material[] materials, int baseVertexIndex)
	{
		int subMeshCount = sourceMesh.subMeshCount;
		if (subMeshCount > 0)
		{
			AllocateIndices();
			for (int i = 0; i < subMeshCount; i++)
			{
				int[] indices = sourceMesh.GetIndices(i);
				Material key = materials[i];
				List<int> orAdd = _indices.GetOrAdd(new NullableKey<Material>(key));
				AppendSubmeshIndices(baseVertexIndex, indices, orAdd);
			}
		}
	}

	private void AppendIndices(IntermediateMesh sourceMesh, int baseVertexIndex)
	{
		(NullableKey<Material>, int[])[] submeshes = sourceMesh.Submeshes;
		int num = submeshes.Length;
		if (num > 0)
		{
			AllocateIndices();
			for (int i = 0; i < num; i++)
			{
				(NullableKey<Material>, int[]) tuple = submeshes[i];
				NullableKey<Material> item = tuple.Item1;
				int[] item2 = tuple.Item2;
				List<int> orAdd = _indices.GetOrAdd(item);
				AppendSubmeshIndices(baseVertexIndex, item2, orAdd);
			}
		}
	}

	private void AllocateIndices()
	{
		if (_indices == null)
		{
			_indices = new Dictionary<NullableKey<Material>, List<int>>();
		}
	}

	private static void AppendSubmeshIndices(int baseVertexIndex, int[] sourceIndices, ICollection<int> targetIndices)
	{
		for (int i = 0; i < sourceIndices.Length; i++)
		{
			targetIndices.Add(baseVertexIndex + sourceIndices[i]);
		}
	}

	private void BuildInternal(out Material[] materials, Mesh mesh)
	{
		mesh.SetVertices(_vertices, 0, _vertexCount);
		if (!_normals.IsNullOrEmpty())
		{
			mesh.SetNormals(_normals, 0, _vertexCount);
		}
		if (!_tangents.IsNullOrEmpty())
		{
			mesh.SetTangents(_tangents, 0, _vertexCount);
		}
		if (!_colors.IsNullOrEmpty())
		{
			mesh.SetColors(_colors, 0, _vertexCount);
		}
		if (!_uv0.IsNullOrEmpty())
		{
			mesh.SetUVs(0, _uv0, 0, _vertexCount);
		}
		if (!_uv1.IsNullOrEmpty())
		{
			mesh.SetUVs(1, _uv1, 0, _vertexCount);
		}
		if (!_uv2.IsNullOrEmpty())
		{
			mesh.SetUVs(2, _uv2, 0, _vertexCount);
		}
		List<Material> list = new List<Material>();
		if (_indices != null)
		{
			int num = 0;
			mesh.subMeshCount = CountNonEmptySubmeshes();
			foreach (KeyValuePair<NullableKey<Material>, List<int>> index in _indices)
			{
				NullableKey<Material> key = index.Key;
				List<int> value = index.Value;
				if (!value.IsEmpty())
				{
					list.Add(key.Key);
					mesh.SetIndices(value, MeshTopology.Triangles, num);
					num++;
				}
			}
		}
		mesh.RecalculateBounds();
		materials = list.ToArray();
	}

	private int CountNonEmptySubmeshes()
	{
		int num = 0;
		foreach (List<int> value in _indices.Values)
		{
			if (!value.IsEmpty())
			{
				num++;
			}
		}
		return num;
	}

	private static void CreateOrResize<T>(ref T[] array, int newSize)
	{
		if (array == null)
		{
			array = new T[newSize];
		}
		if (array.Length != newSize)
		{
			Array.Resize(ref array, newSize);
		}
	}

	private static void CreateOrResize<T>(ref T[] array, int newSize, T value)
	{
		T[] obj = array;
		int num = ((obj != null) ? obj.Length : 0);
		CreateOrResize(ref array, newSize);
		for (int i = num; i < newSize; i++)
		{
			array[i] = value;
		}
	}

	private static T[] CloneArrayOrEmpty<T>(T[] source, int length)
	{
		if (source == null)
		{
			return Array.Empty<T>();
		}
		T[] array = new T[length];
		Array.Copy(source, array, length);
		return array;
	}
}
