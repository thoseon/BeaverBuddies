using System;
using Timberborn.Coordinates;
using Timberborn.Navigation;
using Timberborn.PrefabOptimization;
using Timberborn.Rendering;
using UnityEngine;

namespace Timberborn.BuildingsNavigation;

internal class PathMeshDrawer
{
	public delegate byte ConnectionKey(Vector3Int coordinates, Vector3Int direction);

	private static readonly float VerticalOffset = 0.03f;

	private readonly DistanceToColorConverter _distanceToColorConverter;

	private readonly ConnectionKey _connectionKey;

	private readonly NeighboredValues4<IntermediateMesh> _meshes;

	private readonly Material _material;

	private readonly MeshBuilder _meshBuilder = new MeshBuilder();

	private readonly Mesh _builtMesh;

	public PathMeshDrawer(DistanceToColorConverter distanceToColorConverter, ConnectionKey connectionKey, NeighboredValues4<IntermediateMesh> meshes, Material material)
	{
		_distanceToColorConverter = distanceToColorConverter;
		_connectionKey = connectionKey;
		_meshes = meshes;
		_material = material;
		_builtMesh = _meshBuilder.Build().Mesh;
		_builtMesh.MarkDynamic();
	}

	public void Reset()
	{
		_meshBuilder.Reset("");
	}

	public void Draw()
	{
		if (_builtMesh.vertexCount > 0)
		{
			Graphics.DrawMesh(_builtMesh, Vector3.zero, Quaternion.identity, _material, Layers.UILayer, null, 0, null, castShadows: false, receiveShadows: false, useLightProbes: false);
		}
	}

	public void Build()
	{
		_meshBuilder.Build(_builtMesh);
	}

	public void Add(WeightedCoordinates node)
	{
		Vector3Int coordinates = node.Coordinates;
		byte down = _connectionKey(coordinates, Vector3Int.down);
		byte left = _connectionKey(coordinates, Vector3Int.left);
		byte up = _connectionKey(coordinates, Vector3Int.up);
		byte right = _connectionKey(coordinates, Vector3Int.right);
		if (_meshes.TryGetMatch(down, left, up, right, out var value))
		{
			IntermediateMesh value2 = value.Value;
			Vector3 translation = CoordinateSystem.GridToWorldCentered(coordinates) + new Vector3(0f, VerticalOffset, 0f);
			Array.Fill<Color32>(value2.Colors, _distanceToColorConverter.DistanceToColor(node.Distance));
			_meshBuilder.AppendIntermediateMesh(value2, new TranslationTransform(translation));
		}
		else
		{
			Debug.LogWarning($"Couldn't find an appropriate path marker mesh at {node.Coordinates}." + "Please report this.");
		}
	}
}
