using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.DwellingSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.SelectionSystem;
using Timberborn.TooltipSystem;
using Timberborn.WorkSystem;
using UnityEngine.UIElements;

namespace Timberborn.BeaversUI;

internal class BeaverBuildingsBatchControlRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem
{
	private static readonly string HideDefaultClass = "beaver-buildings-batch-control-row-item__icon--empty";

	private static readonly string HomelessLocKey = "Beaver.Homeless";

	private static readonly string HouseLocKey = "Beaver.House";

	private static readonly string WorkplaceLocKey = "Beaver.Workplace";

	private static readonly string UnemployedLocKey = "Beaver.Unemployed";

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly ILoc _loc;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly Dweller _dweller;

	private readonly Button _dwellerButton;

	private readonly Image _dwellerImage;

	private readonly Worker _worker;

	private readonly Button _workerButton;

	private readonly Image _workerImage;

	public VisualElement Root { get; }

	public BeaverBuildingsBatchControlRowItem(VisualElement root, ITooltipRegistrar tooltipRegistrar, ILoc loc, EntitySelectionService entitySelectionService, Dweller dweller, Button dwellerButton, Image dwellerImage, Worker worker, Button workerButton, Image workerImage)
	{
		Root = root;
		_tooltipRegistrar = tooltipRegistrar;
		_loc = loc;
		_entitySelectionService = entitySelectionService;
		_dweller = dweller;
		_dwellerButton = dwellerButton;
		_dwellerImage = dwellerImage;
		_worker = worker;
		_workerButton = workerButton;
		_workerImage = workerImage;
	}

	public void UpdateRowItem()
	{
		UpdateHome();
		UpdateWorkplace();
	}

	public void Initialize()
	{
		_dwellerButton.ToggleDisplayStyle(_dweller);
		_workerButton.ToggleDisplayStyle((BaseComponent)(object)_worker);
		if ((bool)_dweller)
		{
			_tooltipRegistrar.Register((VisualElement)_dwellerButton, (Func<string>)GetDwellerButtonTooltip);
			_dwellerButton.RegisterCallback<ClickEvent>(SelectHome);
		}
		if ((bool)(BaseComponent)(object)_worker)
		{
			_tooltipRegistrar.Register((VisualElement)_workerButton, (Func<string>)GetWorkerButtonTooltip);
			_workerButton.RegisterCallback<ClickEvent>(SelectWorkplace);
		}
	}

	private void UpdateHome()
	{
		if ((bool)_dweller)
		{
			_dwellerButton.SetEnabled(_dweller.HasHome);
			_dwellerImage.EnableInClassList(HideDefaultClass, _dweller.HasHome);
			_dwellerImage.sprite = (_dweller.HasHome ? _dweller.Home.GetComponent<LabeledEntity>().Image : null);
		}
	}

	private void UpdateWorkplace()
	{
		if ((bool)(BaseComponent)(object)_worker)
		{
			_workerButton.SetEnabled(_worker.Employed);
			_workerImage.EnableInClassList(HideDefaultClass, _worker.Employed);
			_workerImage.sprite = (_worker.Employed ? _worker.Workplace.GetComponent<LabeledEntity>().Image : null);
		}
	}

	private string GetDwellerButtonTooltip()
	{
		if ((bool)_dweller)
		{
			if (!_dweller.HasHome)
			{
				return _loc.T(HomelessLocKey);
			}
			return _loc.T(HouseLocKey, GetDisplayName(_dweller.Home));
		}
		return null;
	}

	private string GetWorkerButtonTooltip()
	{
		if ((bool)(BaseComponent)(object)_worker)
		{
			if (!_worker.Employed)
			{
				return _loc.T(UnemployedLocKey);
			}
			return _loc.T(WorkplaceLocKey, GetDisplayName(_worker.Workplace));
		}
		return null;
	}

	private void SelectHome(ClickEvent evt)
	{
		if ((bool)_dweller)
		{
			Dwelling home = _dweller.Home;
			if (home != null)
			{
				_entitySelectionService.SelectAndFollow(home);
			}
		}
	}

	private void SelectWorkplace(ClickEvent evt)
	{
		if ((bool)(BaseComponent)(object)_worker)
		{
			Workplace workplace = _worker.Workplace;
			if (workplace != null)
			{
				_entitySelectionService.SelectAndFollow(workplace);
			}
		}
	}

	private string GetDisplayName(BaseComponent building)
	{
		return _loc.T(building.GetComponent<LabeledEntitySpec>().DisplayNameLocKey);
	}
}
