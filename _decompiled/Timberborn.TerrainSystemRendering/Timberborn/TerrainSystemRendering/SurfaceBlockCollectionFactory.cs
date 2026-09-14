using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.Coordinates;
using Timberborn.PrefabOptimization;
using UnityEngine;

namespace Timberborn.TerrainSystemRendering;

public class SurfaceBlockCollectionFactory
{
	private readonly MeshBuilder _meshBuilder = new MeshBuilder();

	public SurfaceBlockCollection CreateFromModels(IEnumerable<GameObject> models)
	{
		Dictionary<SurfaceBlockShape, List<IntermediateMesh>> dictionary = new Dictionary<SurfaceBlockShape, List<IntermediateMesh>>();
		Dictionary<SurfaceBlockShape, List<GameObject>> dictionary2 = new Dictionary<SurfaceBlockShape, List<GameObject>>();
		foreach (GameObject model in models)
		{
			SurfaceBlockShape surfaceBlockShape = SurfaceBlockShape.FromModelName(model.name);
			dictionary2.GetOrAdd(surfaceBlockShape).Add(model);
			AddAllVariations(surfaceBlockShape, model, dictionary);
		}
		return new SurfaceBlockCollection(dictionary);
	}

	private void AddAllVariations(SurfaceBlockShape baseShape, GameObject model, Dictionary<SurfaceBlockShape, List<IntermediateMesh>> multimap)
	{
		foreach (Orientation item2 in OrientationExtensions.AllValues())
		{
			SurfaceBlockShape key = baseShape.Rotate(item2);
			Orientation orientation = item2.Flip();
			string name = $"{model.name}-{item2}";
			_meshBuilder.Reset(name);
			Mesh sharedMesh = model.GetComponent<MeshFilter>().sharedMesh;
			Material[] sharedMaterials = model.GetComponent<MeshRenderer>().sharedMaterials;
			_meshBuilder.AppendMesh(sharedMesh, sharedMaterials, new OrientationTransform(orientation));
			IntermediateMesh item = _meshBuilder.BuildIntermediateMesh();
			multimap.GetOrAdd(key).Add(item);
		}
	}
}
