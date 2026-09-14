using Timberborn.BlueprintSystem;
using Timberborn.Rendering;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.AreaSelectionSystem;

public class RectangleBoundsDrawerFactory : ILoadableSingleton
{
	private readonly MeshDrawerFactory _meshDrawerFactory;

	private readonly ISpecService _specService;

	private RectangleBoundsDrawerFactorySpec _rectangleBoundsDrawerFactorySpec;

	public RectangleBoundsDrawerFactory(MeshDrawerFactory meshDrawerFactory, ISpecService specService)
	{
		_meshDrawerFactory = meshDrawerFactory;
		_specService = specService;
	}

	public void Load()
	{
		_rectangleBoundsDrawerFactorySpec = _specService.GetSingleSpec<RectangleBoundsDrawerFactorySpec>();
	}

	public RectangleBoundsDrawer Create(Color tileColor, Color blockSideColor)
	{
		return new RectangleBoundsDrawer(CreateSide(_rectangleBoundsDrawerFactorySpec.BlockSideMesh0010, blockSideColor), CreateSide(_rectangleBoundsDrawerFactorySpec.BlockSideMesh0011, blockSideColor), CreateSide(_rectangleBoundsDrawerFactorySpec.BlockSideMesh0111, blockSideColor), CreateSide(_rectangleBoundsDrawerFactorySpec.BlockSideMesh1010, blockSideColor), CreateSide(_rectangleBoundsDrawerFactorySpec.BlockSideMesh1111, blockSideColor), _meshDrawerFactory.Create(_rectangleBoundsDrawerFactorySpec.BlockBottomMesh, _rectangleBoundsDrawerFactorySpec.BlockBottomMaterial.Asset, tileColor));
	}

	private MeshDrawer CreateSide(AssetRef<Mesh> blockSideMesh0010, Color blockSideColor)
	{
		return _meshDrawerFactory.Create(blockSideMesh0010, _rectangleBoundsDrawerFactorySpec.BlockSideMaterial.Asset, blockSideColor);
	}
}
