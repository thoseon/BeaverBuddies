using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.DwellingSystem;
using Timberborn.EntityNaming;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.SelectionSystem;
using Timberborn.Wellbeing;
using UnityEngine.UIElements;

namespace Timberborn.DwellingSystemUI;

internal class DwellingUserFragment : IEntityPanelFragment
{
	private static readonly string DwellersLocKey = "Dwelling.Dwellers";

	private readonly ILoc _loc;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly DwellerViewFactory _dwellerViewFactory;

	private readonly EntityBadgeService _entityBadgeService;

	private VisualElement _root;

	private VisualElement _buttons;

	private Label _header;

	private Dwelling _dwelling;

	private readonly List<DwellerView> _views = new List<DwellerView>();

	public DwellingUserFragment(ILoc loc, VisualElementLoader visualElementLoader, EntitySelectionService entitySelectionService, DwellerViewFactory dwellerViewFactory, EntityBadgeService entityBadgeService)
	{
		_loc = loc;
		_visualElementLoader = visualElementLoader;
		_entitySelectionService = entitySelectionService;
		_dwellerViewFactory = dwellerViewFactory;
		_entityBadgeService = entityBadgeService;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/DwellingUserFragment");
		_buttons = _root.Q<VisualElement>("Buttons");
		_header = _root.Q<Label>("Header");
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_dwelling = entity.GetComponent<Dwelling>();
		if ((bool)_dwelling)
		{
			InitializeUserViews();
		}
	}

	public void ClearFragment()
	{
		_dwelling = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if ((bool)_dwelling && _dwelling.Enabled)
		{
			_root.ToggleDisplayStyle(visible: true);
			UpdateHeader();
			UpdateViews(_views, _dwelling.AdultDwellers, _dwelling.ChildDwellers);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	private void InitializeUserViews()
	{
		RemoveAllUserViews();
		AddEmptyViewsForAllSlots();
	}

	private void RemoveAllUserViews()
	{
		foreach (DwellerView view in _views)
		{
			_buttons.Remove(view.Root);
		}
		_views.Clear();
	}

	private void AddEmptyViewsForAllSlots()
	{
		for (int i = 0; i < _dwelling.AdultSlots; i++)
		{
			CreateView().SetAsAdult();
		}
		for (int j = 0; j < _dwelling.ChildSlots; j++)
		{
			CreateView().SetAsChild();
		}
	}

	private DwellerView CreateView()
	{
		DwellerView dwellerView = _dwellerViewFactory.Create();
		_views.Add(dwellerView);
		_buttons.Add(dwellerView.Root);
		return dwellerView;
	}

	private void UpdateHeader()
	{
		string text = $"{_dwelling.NumberOfDwellers} / {_dwelling.MaxBeavers}";
		_header.text = _loc.T(DwellersLocKey) + ": " + text;
	}

	private void UpdateViews(IReadOnlyList<DwellerView> views, IEnumerable<Dweller> adults, IEnumerable<Dweller> children)
	{
		int num = 0;
		foreach (Dweller beaver in adults.Concat(children))
		{
			views[num].Fill(wellbeing: beaver.GetComponent<WellbeingTracker>().Wellbeing, user: beaver, onClick: delegate
			{
				_entitySelectionService.SelectAndFollow(beaver);
			}, name: beaver.GetComponent<NamedEntity>().EntityName, subtitle: _entityBadgeService.GetEntitySubtitle(beaver));
			num++;
		}
		for (; num < views.Count; num++)
		{
			views[num].Reset();
		}
	}
}
