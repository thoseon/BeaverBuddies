using System.Collections.Generic;
using Timberborn.BlueprintSystem;
using Timberborn.Coordinates;
using Timberborn.MapStateSystem;
using Timberborn.PrefabOptimization;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.BuildingsNavigation;

internal class BoundsNavRangeDrawer : ILoadableSingleton
{
	private readonly BoundsNavRangeCalculator _boundsNavRangeCalculator;

	private readonly ISpecService _specService;

	private readonly NeighboredValues8<IntermediateMesh> _neighboredMeshes = new NeighboredValues8<IntermediateMesh>();

	private readonly MeshBuilder _meshBuilder = new MeshBuilder();

	private readonly BoundsMesh _boundsMesh = new BoundsMesh();

	private Material[] _materials;

	public BoundsNavRangeDrawer(BoundsNavRangeCalculator boundsNavRangeCalculator, MapSize mapSize, ISpecService specService)
	{
		_boundsNavRangeCalculator = boundsNavRangeCalculator;
		_specService = specService;
	}

	public void Load()
	{
		BoundsNavRangeDrawerSpec singleSpec = _specService.GetSingleSpec<BoundsNavRangeDrawerSpec>();
		_materials = new Material[1] { singleSpec.Material.Asset };
		foreach (AssetRef<Mesh> tileMesh in singleSpec.TileMeshes)
		{
			string name = tileMesh.Asset.name.Replace("NavRangeTile", "");
			bool down = NameToKey(name, 0);
			bool downLeft = NameToKey(name, 1);
			bool left = NameToKey(name, 2);
			bool upLeft = NameToKey(name, 3);
			bool up = NameToKey(name, 4);
			bool upRight = NameToKey(name, 5);
			bool right = NameToKey(name, 6);
			bool downRight = NameToKey(name, 7);
			AddVariants(tileMesh.Asset, down, downLeft, left, upLeft, up, upRight, right, downRight);
		}
		_boundsMesh.Initialize(singleSpec.Material.Asset);
	}

	public void UpdateArea(IReadOnlyCollection<Vector3Int> area)
	{
		_boundsMesh.Reset();
		_boundsNavRangeCalculator.Recalculate(area, _neighboredMeshes, _boundsMesh);
		_boundsMesh.Build();
	}

	public void UpdateAreaPreview(IReadOnlyCollection<Vector3Int> area)
	{
		UpdateArea(area);
	}

	public void Draw()
	{
		_boundsMesh.Draw();
	}

	private static bool NameToKey(string name, int index)
	{
		return int.Parse(name[index].ToString()) == 1;
	}

	private void AddVariants(Mesh mesh, bool down, bool downLeft, bool left, bool upLeft, bool up, bool upRight, bool right, bool downRight)
	{
		AddVariant(mesh, Orientation.Cw0, down, downLeft, left, upLeft, up, upRight, right, downRight);
		AddVariant(mesh, Orientation.Cw90, right, downRight, down, downLeft, left, upLeft, up, upRight);
		AddVariant(mesh, Orientation.Cw180, up, upRight, right, downRight, down, downLeft, left, upLeft);
		AddVariant(mesh, Orientation.Cw270, left, upLeft, up, upRight, right, downRight, down, downLeft);
	}

	private void AddVariant(Mesh mesh, Orientation orientation, bool down, bool downLeft, bool left, bool upLeft, bool up, bool upRight, bool right, bool downRight)
	{
		_meshBuilder.Reset("");
		_meshBuilder.AppendMesh(mesh, _materials, new OrientationTransform(orientation));
		_neighboredMeshes.AddExact(_meshBuilder.BuildIntermediateMesh(), down, downLeft, left, upLeft, up, upRight, right, downRight);
	}
}
