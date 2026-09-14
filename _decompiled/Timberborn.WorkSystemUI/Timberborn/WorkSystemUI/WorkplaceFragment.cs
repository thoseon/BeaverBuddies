using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Bots;
using Timberborn.Characters;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.GameDistricts;
using Timberborn.InputSystemUI;
using Timberborn.Localization;
using Timberborn.PrioritySystemUI;
using Timberborn.SelectionSystem;
using Timberborn.TooltipSystem;
using Timberborn.WorkSystem;
using UnityEngine.UIElements;

namespace Timberborn.WorkSystemUI;

internal class WorkplaceFragment : IEntityPanelFragment
{
	private static readonly string PriorityLabelLocKey = "Work.Workplace.DisplayName";

	private static readonly string PriorityTooltipLocKey = "Work.PriorityTitle";

	private static readonly string CurrentWorkersLocKey = "Work.CurrentWorkers";

	private static readonly string IncreaseWorkersKey = "IncreaseWorkers";

	private static readonly string DecreaseWorkersKey = "DecreaseWorkers";

	private static readonly string ToggleWorkerTypeKey = "ToggleWorkerType";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly WorkerViewFactory _workerViewFactory;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly ILoc _loc;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly WorkplacePriorityToggleGroupFactory _workplacePriorityToggleGroupFactory;

	private readonly BotPopulation _botPopulation;

	private readonly WorkerTypeToggleFactory _workerTypeToggleFactory;

	private readonly BindableButtonFactory _bindableButtonFactory;

	private readonly EntityBadgeService _entityBadgeService;

	private VisualElement _root;

	private VisualElement _workplaceUsers;

	private Label _text;

	private BindableButton _increase;

	private BindableButton _decrease;

	private WorkerTypeToggle _workerTypeToggle;

	private Workplace _workplace;

	private WorkplaceDescriber _workplaceDescriber;

	private readonly List<WorkerView> _views = new List<WorkerView>();

	private PriorityToggleGroup _priorityToggleGroup;

	public WorkplaceFragment(VisualElementLoader visualElementLoader, WorkerViewFactory workerViewFactory, EntitySelectionService entitySelectionService, ILoc loc, ITooltipRegistrar tooltipRegistrar, WorkplacePriorityToggleGroupFactory workplacePriorityToggleGroupFactory, BotPopulation botPopulation, WorkerTypeToggleFactory workerTypeToggleFactory, BindableButtonFactory bindableButtonFactory, EntityBadgeService entityBadgeService)
	{
		_visualElementLoader = visualElementLoader;
		_workerViewFactory = workerViewFactory;
		_entitySelectionService = entitySelectionService;
		_loc = loc;
		_tooltipRegistrar = tooltipRegistrar;
		_workplacePriorityToggleGroupFactory = workplacePriorityToggleGroupFactory;
		_botPopulation = botPopulation;
		_workerTypeToggleFactory = workerTypeToggleFactory;
		_bindableButtonFactory = bindableButtonFactory;
		_entityBadgeService = entityBadgeService;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/WorkplaceFragment");
		_workplaceUsers = _root.Q<VisualElement>("WorkplaceUsers");
		_text = _root.Q<Label>("Text");
		_tooltipRegistrar.Register(_text, () => _workplaceDescriber.GetWorkersTooltip());
		_increase = _bindableButtonFactory.Create(_root.Q<Button>("Increase"), IncreaseWorkersKey, delegate
		{
			_workplace.IncreaseDesiredWorkers();
		});
		_decrease = _bindableButtonFactory.Create(_root.Q<Button>("Decrease"), DecreaseWorkersKey, delegate
		{
			_workplace.DecreaseDesiredWorkers();
		});
		_tooltipRegistrar.RegisterWithKeyBinding(_root.Q<Button>("Increase"), IncreaseWorkersKey);
		_tooltipRegistrar.RegisterWithKeyBinding(_root.Q<Button>("Decrease"), DecreaseWorkersKey);
		VisualElement visualElement = _root.Q<VisualElement>("HeaderWrapper");
		_priorityToggleGroup = _workplacePriorityToggleGroupFactory.Create(visualElement, PriorityLabelLocKey);
		_tooltipRegistrar.RegisterLocalizable(visualElement.Q<VisualElement>("TogglesWrapper"), PriorityTooltipLocKey);
		VisualElement parent = _root.Q<VisualElement>("WorkerTypeToggleWrapper");
		_workerTypeToggle = _workerTypeToggleFactory.CreateBindable(parent, ToggleWorkerTypeKey);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_workplace = entity.GetComponent<Workplace>();
		if ((bool)_workplace)
		{
			_workplaceDescriber = entity.GetComponent<WorkplaceDescriber>();
			WorkplaceWorkerType component = entity.GetComponent<WorkplaceWorkerType>();
			_workerTypeToggle.Show(component);
			WorkplacePriority component2 = entity.GetComponent<WorkplacePriority>();
			_priorityToggleGroup.Enable(component2);
			AddEmptyViews(component);
			_increase.Bind();
			_decrease.Bind();
		}
	}

	public void ClearFragment()
	{
		_workplace = null;
		_workplaceDescriber = null;
		_workerTypeToggle.Clear();
		_priorityToggleGroup.Disable();
		_root.ToggleDisplayStyle(visible: false);
		_increase.Unbind();
		_decrease.Unbind();
	}

	public void UpdateFragment()
	{
		if ((bool)_workplace)
		{
			_priorityToggleGroup.UpdateGroup();
			UpdateButtons();
			if (_workplace.Enabled)
			{
				UpdateViews();
			}
			_workplaceUsers.ToggleDisplayStyle(_workplace.Enabled);
			if (_botPopulation.BotCreated)
			{
				_workerTypeToggle.Update();
				_workerTypeToggle.Root.ToggleDisplayStyle(visible: true);
			}
			else
			{
				_workerTypeToggle.Root.ToggleDisplayStyle(visible: false);
			}
			_root.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	private void AddEmptyViews(WorkplaceWorkerType workplaceWorkerType)
	{
		RemoveViews();
		for (int i = 0; i < _workplace.MaxWorkers; i++)
		{
			AddEmptyView(workplaceWorkerType);
		}
	}

	private void RemoveViews()
	{
		_workplaceUsers.Clear();
		_views.Clear();
	}

	private void AddEmptyView(WorkplaceWorkerType workplaceWorkerType)
	{
		WorkerView workerView = _workerViewFactory.Create(workplaceWorkerType);
		workerView.ShowEmpty();
		_views.Add(workerView);
		_workplaceUsers.Add(workerView.Root);
	}

	private void UpdateViews()
	{
		IEnumerable<Character> enumerable = _workplace.AssignedWorkers.Select((Worker worker) => worker.GetComponent<Character>());
		int num = 0;
		foreach (Character character in enumerable)
		{
			_views[num].Fill(description: character.FirstName, user: character, onClick: delegate
			{
				_entitySelectionService.SelectAndFollow(character);
			});
			num++;
		}
		DistrictBuilding component = _workplace.GetComponent<DistrictBuilding>();
		if (_workplace.Understaffed && (bool)component.InstantDistrict)
		{
			for (; num < _workplace.DesiredWorkers; num++)
			{
				_views[num].ShowVacant();
			}
		}
		for (; num < _views.Count; num++)
		{
			_views[num].ShowEmpty();
		}
	}

	private void UpdateButtons()
	{
		int numberOfAssignedWorkers = _workplace.NumberOfAssignedWorkers;
		int desiredWorkers = _workplace.DesiredWorkers;
		int maxWorkers = _workplace.MaxWorkers;
		_text.text = _loc.T(CurrentWorkersLocKey, numberOfAssignedWorkers, desiredWorkers);
		if (desiredWorkers > 1)
		{
			_decrease.Enable();
		}
		else
		{
			_decrease.Disable();
		}
		if (desiredWorkers < maxWorkers)
		{
			_increase.Enable();
		}
		else
		{
			_increase.Disable();
		}
	}
}
