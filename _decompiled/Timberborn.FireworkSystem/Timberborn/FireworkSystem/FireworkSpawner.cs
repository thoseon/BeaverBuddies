using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.FireworkSystem;

internal class FireworkSpawner
{
	private readonly FireworkSpecService _fireworkSpecService;

	private readonly EntityService _entityService;

	private GameObject _prefab;

	public FireworkSpawner(FireworkSpecService fireworkSpecService, EntityService entityService)
	{
		_fireworkSpecService = fireworkSpecService;
		_entityService = entityService;
	}

	public void SpawnFirework(FireworkLauncher fireworkLauncher)
	{
		FireworkSpec fireworkSpec = _fireworkSpecService.GetFireworkSpec(fireworkLauncher.FireworkId);
		EntityComponent entityComponent = _entityService.Instantiate(fireworkSpec.Blueprint);
		Transform barrelTransform = fireworkLauncher.GetComponent<FireworkLauncherModel>().GetBarrelTransform();
		entityComponent.GameObject.SetActive(value: true);
		entityComponent.GetComponent<Firework>().Launch(barrelTransform.position, barrelTransform.rotation, fireworkLauncher.FlightDistance);
	}
}
