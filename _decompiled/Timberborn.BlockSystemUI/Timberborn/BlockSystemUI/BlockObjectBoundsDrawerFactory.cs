using Timberborn.BlueprintSystem;
using Timberborn.Rendering;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.BlockSystemUI;

public class BlockObjectBoundsDrawerFactory : ILoadableSingleton
{
	private readonly MeshDrawerFactory _meshDrawerFactory;

	private readonly ISpecService _specService;

	private BlockObjectBoundsDrawerFactorySpec _blockObjectBoundsDrawerFactorySpec;

	public BlockObjectBoundsDrawerFactory(MeshDrawerFactory meshDrawerFactory, ISpecService specService)
	{
		_meshDrawerFactory = meshDrawerFactory;
		_specService = specService;
	}

	public void Load()
	{
		_blockObjectBoundsDrawerFactorySpec = _specService.GetSingleSpec<BlockObjectBoundsDrawerFactorySpec>();
	}

	public BlockObjectBoundsDrawer Create(Color color)
	{
		Material asset = _blockObjectBoundsDrawerFactorySpec.Material.Asset;
		MeshDrawer blockSideMeshDrawer = _meshDrawerFactory.Create(_blockObjectBoundsDrawerFactorySpec.BlockSideMesh0010, asset, color);
		MeshDrawer blockSideMeshDrawer2 = _meshDrawerFactory.Create(_blockObjectBoundsDrawerFactorySpec.BlockSideMesh0011, asset, color);
		MeshDrawer blockSideMeshDrawer3 = _meshDrawerFactory.Create(_blockObjectBoundsDrawerFactorySpec.BlockSideMesh0111, asset, color);
		MeshDrawer blockSideMeshDrawer4 = _meshDrawerFactory.Create(_blockObjectBoundsDrawerFactorySpec.BlockSideMesh1010, asset, color);
		MeshDrawer blockSideMeshDrawer5 = _meshDrawerFactory.Create(_blockObjectBoundsDrawerFactorySpec.BlockSideMesh1111, asset, color);
		return new BlockObjectBoundsDrawer(blockSideMeshDrawer, blockSideMeshDrawer2, blockSideMeshDrawer3, blockSideMeshDrawer4, blockSideMeshDrawer5);
	}
}
