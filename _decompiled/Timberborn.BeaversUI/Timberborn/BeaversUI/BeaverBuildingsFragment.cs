using Timberborn.BaseComponentSystem;
using Timberborn.BeaverContaminationSystem;
using Timberborn.Buildings;
using Timberborn.CoreUI;
using Timberborn.DwellingSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.WorkSystem;
using UnityEngine.UIElements;

namespace Timberborn.BeaversUI;

public class BeaverBuildingsFragment : IEntityPanelFragment
{
	private static readonly string HomelessLocKey = "Beaver.Homeless";

	private static readonly string HouseLocKey = "Beaver.House";

	private static readonly string WorkplaceLocKey = "Beaver.Workplace";

	private static readonly string UnemployedLocKey = "Beaver.Unemployed";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly BeaverBuildingViewFactory _beaverBuildingViewFactory;

	private VisualElement _root;

	private BeaverBuildingView _home;

	private BeaverBuildingView _workplace;

	private Dweller _dweller;

	private Worker _worker;

	private Contaminable _contaminable;

	private bool IsWorkplaceVisible
	{
		get
		{
			if ((bool)(BaseComponent)(object)_worker)
			{
				return !(_contaminable?.IsContaminated ?? false);
			}
			return false;
		}
	}

	public BeaverBuildingsFragment(VisualElementLoader visualElementLoader, ILoc loc, BeaverBuildingViewFactory beaverBuildingViewFactory)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
		_beaverBuildingViewFactory = beaverBuildingViewFactory;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/BeaverBuildingsFragment");
		_home = _beaverBuildingViewFactory.Create(_root.Q<Button>("Home"));
		_workplace = _beaverBuildingViewFactory.Create(_root.Q<Button>("Workplace"));
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_dweller = entity.GetComponent<Dweller>();
		_worker = entity.GetComponent<Worker>();
		_contaminable = entity.GetComponent<Contaminable>();
		_root.ToggleDisplayStyle((bool)_dweller || IsWorkplaceVisible);
		_home.Root.ToggleDisplayStyle(_dweller);
	}

	public void ClearFragment()
	{
		_root.ToggleDisplayStyle(visible: false);
		_dweller = null;
		_worker = null;
		_contaminable = null;
	}

	public void UpdateFragment()
	{
		UpdateHomePanel();
		UpdateWorkplacePanel();
	}

	private void UpdateHomePanel()
	{
		if ((bool)_dweller)
		{
			if (_dweller.HasHome)
			{
				Building component = _dweller.Home.GetComponent<Building>();
				_home.SetBuilding(component, _loc.T(HouseLocKey, GetDisplayName(component)));
			}
			else
			{
				_home.SetDescriptionOnly(_loc.T(HomelessLocKey));
			}
		}
	}

	private void UpdateWorkplacePanel()
	{
		if (IsWorkplaceVisible)
		{
			_workplace.Root.ToggleDisplayStyle(visible: true);
			if ((bool)_worker.Workplace)
			{
				Building component = _worker.Workplace.GetComponent<Building>();
				_workplace.SetBuilding(component, _loc.T(WorkplaceLocKey, GetDisplayName(component)));
			}
			else
			{
				_workplace.SetDescriptionOnly(_loc.T(UnemployedLocKey));
			}
		}
		else
		{
			_workplace.Root.ToggleDisplayStyle(visible: false);
		}
	}

	private string GetDisplayName(Building building)
	{
		return _loc.T(building.GetComponent<LabeledEntitySpec>().DisplayNameLocKey);
	}
}
