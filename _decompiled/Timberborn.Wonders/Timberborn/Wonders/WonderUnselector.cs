using System;
using Timberborn.BaseComponentSystem;
using Timberborn.SelectionSystem;
using UnityEngine;

namespace Timberborn.Wonders;

internal class WonderUnselector : BaseComponent, IAwakableComponent, IUpdatableComponent
{
	private static readonly float UnselectionDelay = 0.5f;

	private readonly EntitySelectionService _entitySelectionService;

	private SelectableObject _selectableObject;

	private float _unselectionTime;

	public WonderUnselector(EntitySelectionService entitySelectionService)
	{
		_entitySelectionService = entitySelectionService;
	}

	public void Awake()
	{
		GetComponent<Wonder>().WonderActivated += OnWonderActivated;
		_selectableObject = GetComponent<SelectableObject>();
		DisableComponent();
	}

	public void Update()
	{
		if (Time.time >= _unselectionTime)
		{
			if ((bool)_selectableObject && _entitySelectionService.SelectedObject == _selectableObject)
			{
				_entitySelectionService.Unselect();
			}
			DisableComponent();
		}
	}

	private void OnWonderActivated(object sender, EventArgs e)
	{
		_unselectionTime = Time.time + UnselectionDelay;
		EnableComponent();
	}
}
