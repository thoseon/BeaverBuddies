using System.Collections.Generic;
using Timberborn.BlueprintSystem;
using Timberborn.Coordinates;
using Timberborn.PrefabOptimization;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.BuildingsNavigation;

internal class PathMeshDrawerFactory : ILoadableSingleton
{
	private readonly DistanceToColorConverter _distanceToColorConverter;

	private readonly ISpecService _specService;

	private PathMeshDrawerFactorySpec _pathMeshDrawerFactorySpec;

	private readonly MeshBuilder _meshBuilder = new MeshBuilder();

	public PathMeshDrawerFactory(DistanceToColorConverter distanceToColorConverter, ISpecService specService)
	{
		_distanceToColorConverter = distanceToColorConverter;
		_specService = specService;
	}

	public void Load()
	{
		_pathMeshDrawerFactorySpec = _specService.GetSingleSpec<PathMeshDrawerFactorySpec>();
	}

	public PathMeshDrawer CreateRegularDrawer(PathMeshDrawer.ConnectionKey connectionKey)
	{
		return Create(_pathMeshDrawerFactorySpec.RegularModelVariants, connectionKey);
	}

	public PathMeshDrawer CreateStairsDrawer(PathMeshDrawer.ConnectionKey connectionKey)
	{
		return Create(_pathMeshDrawerFactorySpec.StairsModelVariants, connectionKey);
	}

	private PathMeshDrawer Create(IEnumerable<AssetRef<Mesh>> meshes, PathMeshDrawer.ConnectionKey connectionKey)
	{
		return new PathMeshDrawer(_distanceToColorConverter, connectionKey, GenerateNeighboredMeshes(meshes), _pathMeshDrawerFactorySpec.Material.Asset);
	}

	private NeighboredValues4<IntermediateMesh> GenerateNeighboredMeshes(IEnumerable<AssetRef<Mesh>> meshes)
	{
		NeighboredValues4<IntermediateMesh> neighboredValues = new NeighboredValues4<IntermediateMesh>();
		Material[] materials = new Material[1] { _pathMeshDrawerFactorySpec.Material.Asset };
		foreach (AssetRef<Mesh> mesh in meshes)
		{
			Mesh asset = mesh.Asset;
			var (b, b2, b3, b4) = VariantNameToByteKeys(asset.name);
			AddVariant(neighboredValues, asset, materials, Orientation.Cw0, b, b2, b3, b4);
			AddVariant(neighboredValues, asset, materials, Orientation.Cw90, b4, b, b2, b3);
			AddVariant(neighboredValues, asset, materials, Orientation.Cw180, b3, b4, b, b2);
			AddVariant(neighboredValues, asset, materials, Orientation.Cw270, b2, b3, b4, b);
		}
		return neighboredValues;
	}

	private static (byte down, byte left, byte up, byte right) VariantNameToByteKeys(string name)
	{
		string text = name.Substring(name.Length - 4);
		byte item = PathMeshConnectionKeys.ParseCharToByteKey(text[0]);
		byte item2 = PathMeshConnectionKeys.ParseCharToByteKey(text[1]);
		byte item3 = PathMeshConnectionKeys.ParseCharToByteKey(text[2]);
		byte item4 = PathMeshConnectionKeys.ParseCharToByteKey(text[3]);
		return (down: item, left: item2, up: item3, right: item4);
	}

	private void AddVariant(NeighboredValues4<IntermediateMesh> meshes, Mesh mesh, Material[] materials, Orientation orientation, byte down, byte left, byte up, byte right)
	{
		_meshBuilder.Reset("");
		_meshBuilder.AppendMesh(mesh, materials, new OrientationTransform(orientation));
		meshes.AddExact(_meshBuilder.BuildIntermediateMesh(), down, left, up, right);
	}
}
