using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.EntitySystem;
using UnityEngine;

namespace Timberborn.SoilMoistureSystem;

internal class DryObjectModel : BaseComponent, IAwakableComponent, IPostInitializableEntity
{
	private GameObject _wetModel;

	private GameObject _dryModel;

	private DryObject _dryObject;

	public void Awake()
	{
		_dryObject = GetComponent<DryObject>();
		DryObjectModelSpec component = GetComponent<DryObjectModelSpec>();
		_wetModel = base.GameObject.FindChild(component.WetModelName);
		_dryModel = base.GameObject.FindChild(component.DryModelName);
	}

	public void PostInitializeEntity()
	{
		_dryObject.EnteredDryState += delegate
		{
			UpdateModel();
		};
		_dryObject.ExitedDryState += delegate
		{
			UpdateModel();
		};
		UpdateModel();
	}

	private void UpdateModel()
	{
		ToggleModel(_dryObject.IsDry);
	}

	private void ToggleModel(bool dry)
	{
		_wetModel.SetActive(!dry);
		_dryModel.SetActive(dry);
	}
}
