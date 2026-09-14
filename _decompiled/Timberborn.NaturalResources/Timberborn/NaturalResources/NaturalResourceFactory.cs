using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using UnityEngine;

namespace Timberborn.NaturalResources;

public class NaturalResourceFactory
{
	private readonly TemplateNameMapper _templateNameMapper;

	private readonly SpawnValidationService _spawnValidationService;

	private readonly BlockObjectFactory _blockObjectFactory;

	private readonly EventBus _eventBus;

	public NaturalResourceFactory(TemplateNameMapper templateNameMapper, SpawnValidationService spawnValidationService, BlockObjectFactory blockObjectFactory, EventBus eventBus)
	{
		_templateNameMapper = templateNameMapper;
		_spawnValidationService = spawnValidationService;
		_blockObjectFactory = blockObjectFactory;
		_eventBus = eventBus;
	}

	public NaturalResource SpawnIgnoringConstraintsAndRandomizePosition(string resourceId, Vector3Int coordinates)
	{
		return SpawnIgnoringConstraints(resourceId, coordinates, randomPosition: true);
	}

	public NaturalResource SpawnIgnoringConstraints(string resourceId, Vector3Int coordinates)
	{
		return SpawnIgnoringConstraints(resourceId, coordinates, randomPosition: false);
	}

	public NaturalResource SpawnNew(string resourceId, Vector3Int coordinates)
	{
		BlockObjectSpec spec = _templateNameMapper.GetTemplate(resourceId).GetSpec<BlockObjectSpec>();
		if (_spawnValidationService.CanSpawn(coordinates, spec, resourceId))
		{
			return Create(coordinates, spec, randomPosition: true);
		}
		return null;
	}

	public void PlantNew(string resourceId, Vector3Int coordinates)
	{
		BlockObjectSpec spec = _templateNameMapper.GetTemplate(resourceId).GetSpec<BlockObjectSpec>();
		Create(coordinates, spec, randomPosition: false);
		_eventBus.Post(new NaturalResourcePlantedEvent(spec));
	}

	private NaturalResource SpawnIgnoringConstraints(string resourceId, Vector3Int coordinates, bool randomPosition)
	{
		BlockObjectSpec spec = _templateNameMapper.GetTemplate(resourceId).GetSpec<BlockObjectSpec>();
		if (_spawnValidationService.CanSpawnIgnoringConstraints(coordinates, spec))
		{
			return Create(coordinates, spec, randomPosition);
		}
		return null;
	}

	private NaturalResource Create(Vector3Int coordinates, BlockObjectSpec template, bool randomPosition)
	{
		EntitySetup.Builder builder = new EntitySetup.Builder(template.Blueprint);
		if (randomPosition)
		{
			builder.AddInitComponent(new CoordinatesOffsetterInit());
		}
		return _blockObjectFactory.CreateFinished(builder, new Placement(coordinates)).GetComponent<NaturalResource>();
	}
}
