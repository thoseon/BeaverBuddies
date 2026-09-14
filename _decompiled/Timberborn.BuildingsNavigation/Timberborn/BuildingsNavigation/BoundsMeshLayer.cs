using Timberborn.PrefabOptimization;
using Timberborn.Rendering;
using UnityEngine;

namespace Timberborn.BuildingsNavigation;

internal class BoundsMeshLayer
{
	private readonly Material _material;

	private readonly MeshBuilder _meshBuilder;

	private readonly Mesh _mesh;

	private BoundsMeshLayer(Material material, MeshBuilder meshBuilder, Mesh mesh)
	{
		_material = material;
		_meshBuilder = meshBuilder;
		_mesh = mesh;
	}

	public static BoundsMeshLayer Create(Material baseMaterial, int layerIndex)
	{
		Material material = new Material(baseMaterial)
		{
			renderQueue = baseMaterial.renderQueue + layerIndex
		};
		MeshBuilder meshBuilder = new MeshBuilder();
		Mesh mesh = meshBuilder.Build().Mesh;
		mesh.MarkDynamic();
		return new BoundsMeshLayer(material, meshBuilder, mesh);
	}

	public void Reset()
	{
		_meshBuilder.Reset(string.Empty);
	}

	public void Build()
	{
		_meshBuilder.Build(_mesh);
	}

	public void AppendMesh(IntermediateMesh mesh, TranslationTransform translation)
	{
		_meshBuilder.AppendIntermediateMesh(mesh, translation);
	}

	public void Draw()
	{
		if (_mesh.vertexCount > 0)
		{
			Graphics.DrawMesh(_mesh, Vector3.zero, Quaternion.identity, _material, Layers.UILayer, null, 0, null, castShadows: false, receiveShadows: false, useLightProbes: false);
		}
	}
}
