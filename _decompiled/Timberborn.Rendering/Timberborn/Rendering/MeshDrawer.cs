using System.Collections.Generic;
using Timberborn.Coordinates;
using UnityEngine;

namespace Timberborn.Rendering;

public class MeshDrawer
{
	private static readonly int ColorProperty = Shader.PropertyToID("_BaseColor");

	private readonly Mesh _mesh;

	private readonly Material _material;

	private readonly MaterialPropertyBlock _defaultMaterialPropertyBlock;

	private readonly MaterialPropertyBlock _modifiableMaterialPropertyBlock;

	public MeshDrawer(Mesh mesh, Material material, MaterialPropertyBlock defaultMaterialPropertyBlock, MaterialPropertyBlock modifiableMaterialPropertyBlock)
	{
		_mesh = mesh;
		_material = material;
		_defaultMaterialPropertyBlock = defaultMaterialPropertyBlock;
		_modifiableMaterialPropertyBlock = modifiableMaterialPropertyBlock;
	}

	public void DrawAtCoordinates(Vector3Int coordinates, float verticalOffset)
	{
		DrawAtCoordinates(coordinates, verticalOffset, Quaternion.identity);
	}

	public void DrawAtCoordinates(Vector3Int coordinates, float verticalOffset, Color color)
	{
		Vector3 position = GetPosition(coordinates, verticalOffset);
		DrawAtPosition(position, Quaternion.identity, color);
	}

	public void DrawAtCoordinates(Vector3Int coordinates, float verticalOffset, Quaternion rotation)
	{
		Vector3 position = GetPosition(coordinates, verticalOffset);
		DrawAtPosition(position, rotation, _defaultMaterialPropertyBlock);
	}

	public void DrawAtPosition(Vector3 position, Quaternion rotation, Color color)
	{
		_modifiableMaterialPropertyBlock.SetColor(ColorProperty, color);
		DrawAtPosition(position, rotation, _modifiableMaterialPropertyBlock);
	}

	public void DrawMultiple(List<Matrix4x4> matrices)
	{
		for (int i = 0; i < matrices.Count; i++)
		{
			Draw(matrices[i]);
		}
	}

	public void DrawMultipleInstanced(List<Matrix4x4> matrices)
	{
		Graphics.DrawMeshInstanced(_mesh, 0, _material, matrices);
	}

	public void Draw(Matrix4x4 matrix)
	{
		Draw(matrix, _defaultMaterialPropertyBlock);
	}

	private void Draw(Matrix4x4 matrix, MaterialPropertyBlock materialPropertyBlock)
	{
		Graphics.DrawMesh(_mesh, matrix, _material, Layers.UILayer, null, 0, materialPropertyBlock, castShadows: false, receiveShadows: false, useLightProbes: false);
	}

	private void DrawAtPosition(Vector3 position, Quaternion rotation, MaterialPropertyBlock materialPropertyBlock)
	{
		Graphics.DrawMesh(_mesh, position, rotation, _material, Layers.UILayer, null, 0, materialPropertyBlock, castShadows: false, receiveShadows: false, useLightProbes: false);
	}

	private static Vector3 GetPosition(Vector3Int coordinates, float verticalOffset)
	{
		return CoordinateSystem.GridToWorldCentered(coordinates) + new Vector3(0f, verticalOffset, 0f);
	}
}
