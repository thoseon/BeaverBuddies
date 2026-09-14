using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.Rendering;

public class MeshDrawerFactory
{
	private static readonly int ColorProperty = Shader.PropertyToID("_BaseColor");

	public MeshDrawer Create(AssetRef<Mesh> mesh, Material material, Color color)
	{
		return Create(mesh.Asset, material, CreateColorizedMaterialPropertyBlock(color));
	}

	public MeshDrawer Create(Mesh mesh, Material material)
	{
		return Create(mesh, material, CreateBlankMaterialPropertyBlock());
	}

	private static MeshDrawer Create(Mesh mesh, Material material, MaterialPropertyBlock defaultMaterialPropertyBlock)
	{
		return new MeshDrawer(mesh, material, defaultMaterialPropertyBlock, CreateBlankMaterialPropertyBlock());
	}

	private static MaterialPropertyBlock CreateColorizedMaterialPropertyBlock(Color tileColor)
	{
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.SetColor(ColorProperty, tileColor);
		return materialPropertyBlock;
	}

	private static MaterialPropertyBlock CreateBlankMaterialPropertyBlock()
	{
		return new MaterialPropertyBlock();
	}
}
