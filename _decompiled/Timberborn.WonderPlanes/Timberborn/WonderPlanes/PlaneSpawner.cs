using Timberborn.BaseComponentSystem;
using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.Rendering;
using Timberborn.TemplateSystem;
using UnityEngine;

namespace Timberborn.WonderPlanes;

internal class PlaneSpawner : BaseComponent, IAwakableComponent
{
	private readonly TemplateService _templateService;

	private readonly EntityService _entityService;

	private readonly MaterialColorer _materialColorer;

	private Blueprint _planeTemplate;

	private Transform _spawnPoint;

	public Vector3 SpawnPosition => _spawnPoint.position;

	public PlaneSpawner(TemplateService templateService, EntityService entityService, MaterialColorer materialColorer)
	{
		_templateService = templateService;
		_entityService = entityService;
		_materialColorer = materialColorer;
	}

	public void Awake()
	{
		_planeTemplate = _templateService.GetSingle<PlaneSpec>().Blueprint;
		string spawnPointName = GetComponent<PlaneSpawnerSpec>().SpawnPointName;
		_spawnPoint = base.GameObject.FindChildTransform(spawnPointName);
	}

	public Plane SpawnPlane(Pilot pilot)
	{
		Plane component = _entityService.Instantiate(_planeTemplate).GetComponent<Plane>();
		_materialColorer.EnableLighting(component);
		component.Initialize(_spawnPoint);
		pilot.AssignPlane(component);
		return component;
	}
}
