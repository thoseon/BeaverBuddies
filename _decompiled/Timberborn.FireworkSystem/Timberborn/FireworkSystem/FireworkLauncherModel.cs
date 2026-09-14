using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.FireworkSystem;

internal class FireworkLauncherModel : BaseComponent, IAwakableComponent, IPostPlacementChangeListener, IPostInitializableEntity
{
	private FireworkLauncher _fireworkLauncher;

	private Transform _turret;

	private Transform _barrel;

	public void Awake()
	{
		FireworkLauncherSpec component = GetComponent<FireworkLauncherSpec>();
		_fireworkLauncher = GetComponent<FireworkLauncher>();
		_turret = base.GameObject.FindChildTransform(component.Turret);
		_barrel = base.GameObject.FindChildTransform(component.Barrel);
		UpdateModel();
	}

	public void PostInitializeEntity()
	{
		_fireworkLauncher.AnglesChanged += delegate
		{
			UpdateModel();
		};
		UpdateModel();
	}

	public void OnPostPlacementChanged()
	{
		UpdateModel();
	}

	internal Transform GetBarrelTransform()
	{
		return _barrel.transform;
	}

	private void UpdateModel()
	{
		_turret.localRotation = Quaternion.Euler(0f, _fireworkLauncher.Heading, 0f);
		_barrel.localRotation = Quaternion.Euler(-_fireworkLauncher.Pitch - 90, 0f, 0f);
	}
}
