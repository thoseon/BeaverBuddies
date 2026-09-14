using System;
using System.Collections.Generic;
using Bindito.Unity;
using Timberborn.AssetSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.ConstructionSites;
using Timberborn.Coordinates;
using Timberborn.DeconstructionSystem;
using Timberborn.EntitySystem;
using Timberborn.TemplateSystem;
using Timberborn.TerrainLevelValidation;
using Timberborn.TerrainSystem;
using UnityEngine;

namespace Timberborn.Explosions;

internal class Tunnel : BaseComponent, IAwakableComponent, IFinishedStateListener, ITerrainRemovingEntity, IBottomLevelProvider
{
	private readonly EntityService _entityService;

	private readonly IAssetLoader _assetLoader;

	private readonly ITerrainService _terrainService;

	private readonly IInstantiator _instantiator;

	private readonly TemplateNameMapper _templateNameMapper;

	private readonly ConstructionFactory _constructionFactory;

	private readonly IBlockService _blockService;

	private readonly ExplosionSoundPlayer _explosionSoundPlayer;

	private readonly BlockValidator _blockValidator;

	private BlockObject _blockObject;

	private Deconstructible _deconstructible;

	private GameObject _explosionPrefab;

	private BlockObjectSpec _tunnelSupportTemplate;

	public int BottomLevel => _blockObject.Coordinates.z + _blockObject.Blocks.Size.z;

	public Tunnel(EntityService entityService, IAssetLoader assetLoader, ITerrainService terrainService, IInstantiator instantiator, TemplateNameMapper templateNameMapper, ConstructionFactory constructionFactory, IBlockService blockService, ExplosionSoundPlayer explosionSoundPlayer, BlockValidator blockValidator)
	{
		_entityService = entityService;
		_assetLoader = assetLoader;
		_terrainService = terrainService;
		_instantiator = instantiator;
		_templateNameMapper = templateNameMapper;
		_constructionFactory = constructionFactory;
		_blockService = blockService;
		_explosionSoundPlayer = explosionSoundPlayer;
		_blockValidator = blockValidator;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_deconstructible = GetComponent<Deconstructible>();
		TunnelSpec component = GetComponent<TunnelSpec>();
		_explosionPrefab = _assetLoader.Load<GameObject>(component.ExplosionPrefabPath);
		_tunnelSupportTemplate = _templateNameMapper.GetTemplate(component.TunnelSupportTemplateName).GetSpec<BlockObjectSpec>();
		GetComponent<DeleteOnFinishConstructionSite>().Deleted += OnDeleted;
	}

	public void OnEnterFinishedState()
	{
		Explode();
	}

	public void OnExitFinishedState()
	{
	}

	public bool RemovesTerrainAt(Vector3Int coordinates)
	{
		return _blockObject.Coordinates == coordinates;
	}

	private void Explode()
	{
		SpawnParticles();
		_terrainService.UnsetTerrain(_blockObject.Coordinates);
		_deconstructible.DisableDeconstruction();
		DestroyGroundOnlyObjectsAbove();
	}

	private void OnDeleted(object sender, EventArgs e)
	{
		Placement placement = _blockObject.Placement;
		if (_blockValidator.BlocksValid(_tunnelSupportTemplate, placement))
		{
			EntitySetup.Builder entitySetupBuilder = new EntitySetup.Builder(_tunnelSupportTemplate.Blueprint);
			_constructionFactory.CreateAsFinished(entitySetupBuilder, placement);
		}
	}

	private void SpawnParticles()
	{
		GameObject gameObject = _instantiator.Instantiate(_explosionPrefab, null);
		gameObject.transform.position = _blockObject.GetComponent<BlockObjectCenter>().WorldCenter;
		_explosionSoundPlayer.Play(gameObject);
	}

	private void DestroyGroundOnlyObjectsAbove()
	{
		List<BlockObject> list = new List<BlockObject>();
		Vector3Int coordinates = _blockObject.Coordinates.Above();
		foreach (BlockObject item in _blockService.GetObjectsAt(coordinates))
		{
			if (item.PositionedBlocks.GetBlock(coordinates).MatterBelow == MatterBelow.Ground)
			{
				list.Add(item);
			}
		}
		foreach (BlockObject item2 in list)
		{
			_entityService.Delete(item2);
		}
	}
}
