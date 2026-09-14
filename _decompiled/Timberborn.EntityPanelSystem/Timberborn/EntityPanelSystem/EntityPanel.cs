using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.CoreUI;
using Timberborn.EntityNaming;
using Timberborn.EntityNamingUI;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.TooltipSystem;
using Timberborn.UILayoutSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.EntityPanelSystem;

internal class EntityPanel : ILoadableSingleton, IUpdatableSingleton, IEntityPanel
{
	private static readonly string ShowLocKey = "EntityPanel.Show";

	private static readonly string HideLocKey = "EntityPanel.Hide";

	private static readonly string RenameLocKey = "EntityPanel.Rename";

	private static readonly string DescriptionHiddenClass = "entity-panel__description--hidden";

	private static readonly string HiderShowClass = "entity-panel__description-hider--show-icon";

	private static readonly string DescriptionNoneClass = "entity-panel__description--none";

	private static readonly string HiderNoneClass = "entity-panel__description-hider--none";

	private readonly UILayout _uiLayout;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly EventBus _eventBus;

	private readonly EntityNameDialog _entityNameDialog;

	private readonly EntityBadgeService _entityBadgeService;

	private readonly ILoc _loc;

	private readonly EntityDescriptionService _entityDescriptionService;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly DiagnosticFragmentController _diagnosticFragmentController;

	private readonly ImmutableArray<EntityPanelModule> _entityPanelModules;

	private readonly List<IEntityPanelFragment> _entityPanelFragments = new List<IEntityPanelFragment>();

	private VisualElement _root;

	private VisualElement _entityAvatar;

	private VisualElement _entityDescription;

	private EntityComponent _shownEntity;

	private Button _entityDescriptionHider;

	private Button _entityNameButton;

	private Label _entityNameText;

	private VisualElement _entityNameHint;

	private Label _entitySubtitle;

	private Button _entityClickableSubtitleButton;

	private Image _entityClickableSubtitleWarningIcon;

	private ClickableSubtitle _entityClickableSubtitle;

	private VisualElement _leftHeaderButtons;

	private VisualElement _rightHeaderButtons;

	private VisualElement _middleHeaderButtons;

	private VisualElement _sideFragments;

	private bool DescriptionVisible => _entityDescription.IsDisplayed();

	private string DescriptionHiderTooltipText
	{
		get
		{
			if (!DescriptionVisible)
			{
				return _loc.T(ShowLocKey);
			}
			return _loc.T(HideLocKey);
		}
	}

	public EntityPanel(IEnumerable<EntityPanelModule> entityPanelModules, UILayout uiLayout, VisualElementLoader visualElementLoader, EventBus eventBus, EntityNameDialog entityNameDialog, EntityBadgeService entityBadgeService, ILoc loc, EntityDescriptionService entityDescriptionService, ITooltipRegistrar tooltipRegistrar, DiagnosticFragmentController diagnosticFragmentController)
	{
		_entityPanelModules = entityPanelModules.ToImmutableArray();
		_uiLayout = uiLayout;
		_visualElementLoader = visualElementLoader;
		_eventBus = eventBus;
		_entityNameDialog = entityNameDialog;
		_entityBadgeService = entityBadgeService;
		_loc = loc;
		_entityDescriptionService = entityDescriptionService;
		_tooltipRegistrar = tooltipRegistrar;
		_diagnosticFragmentController = diagnosticFragmentController;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Common/EntityPanel/EntityPanel");
		InitializeFields();
		InitializeModules();
		_eventBus.Register(this);
		_uiLayout.AddAbsoluteItem(_root);
		_root.ToggleDisplayStyle(visible: false);
		InitializeButtons();
	}

	public void UpdateSingleton()
	{
		if ((bool)_shownEntity)
		{
			UpdateEntityBadge();
			UpdateFragments();
		}
	}

	[OnEvent]
	public void OnSelectableObjectSelected(SelectableObjectSelectedEvent selectableObjectSelectedEvent)
	{
		if (selectableObjectSelectedEvent.SelectableObject.TryGetComponent<EntityComponent>(out var component))
		{
			Show(component);
		}
	}

	[OnEvent]
	public void OnSelectableObjectUnselectedEvent(SelectableObjectUnselectedEvent selectableObjectUnselectedEvent)
	{
		Hide();
	}

	public void ReloadDescription(EntityComponent entity)
	{
		if (entity == _shownEntity)
		{
			_entityDescription.Clear();
			UpdateEntityDescription();
		}
	}

	private void InitializeFields()
	{
		_entityAvatar = _root.Q<VisualElement>("EntityAvatar");
		_entityDescription = _root.Q<VisualElement>("EntityDescription");
		_entityDescriptionHider = _root.Q<Button>("EntityDescriptionHider");
		_entityNameButton = _root.Q<Button>("EntityName");
		_entityNameText = _root.Q<Label>("EntityNameText");
		_entityNameHint = _root.Q<VisualElement>("EntityNameHint");
		_entitySubtitle = _root.Q<Label>("EntitySubtitle");
		_entityClickableSubtitleButton = _root.Q<Button>("EntityClickableSubtitle");
		_entityClickableSubtitleWarningIcon = _root.Q<Image>("SubtitleWarning");
		_leftHeaderButtons = _root.Q<VisualElement>("LeftButtons");
		_rightHeaderButtons = _root.Q<VisualElement>("RightButtons");
		_middleHeaderButtons = _root.Q<VisualElement>("MiddleButtons");
		_sideFragments = _root.Q<VisualElement>("SideFragments");
	}

	private void InitializeButtons()
	{
		_entityNameButton.RegisterCallback<ClickEvent>(OnEntityNameClicked);
		_entityDescriptionHider.RegisterCallback<ClickEvent>(ToggleEntityDescription);
		_entityClickableSubtitleButton.RegisterCallback<ClickEvent>(PerformSubtitleButtonClickAction);
		_tooltipRegistrar.Register(_entityDescriptionHider, () => DescriptionHiderTooltipText);
		_tooltipRegistrar.Register(_root.Q<VisualElement>("ClickableSubtitleWrapper"), () => _entityClickableSubtitle.TooltipText);
		_tooltipRegistrar.Register((VisualElement)_entityNameButton, (Func<string>)GetEntityNameTooltip);
	}

	private void InitializeModules()
	{
		AddFragments(Order(_entityPanelModules.SelectMany((EntityPanelModule module) => module.LeftHeaderFragments)), _leftHeaderButtons);
		AddFragments(Order(_entityPanelModules.SelectMany((EntityPanelModule module) => module.RightHeaderFragments)), _rightHeaderButtons);
		AddFragments(_entityPanelModules.SelectMany((EntityPanelModule module) => module.MiddleHeaderFragments), _middleHeaderButtons);
		AddFragments(_entityPanelModules.SelectMany((EntityPanelModule module) => module.SideFragments), _sideFragments);
		AddFragments(Order(_entityPanelModules.SelectMany((EntityPanelModule module) => module.ContentFragments)), _root.Q<VisualElement>("Fragments"));
		_diagnosticFragmentController.Initialize(_entityPanelModules.SelectMany((EntityPanelModule module) => module.DiagnosticFragments), _root);
	}

	private void AddFragments(IEnumerable<IEntityPanelFragment> fragments, VisualElement parent)
	{
		foreach (IEntityPanelFragment fragment in fragments)
		{
			parent.Add(fragment.InitializeFragment());
			_entityPanelFragments.Add(fragment);
		}
	}

	private void Show(EntityComponent entity)
	{
		_shownEntity = entity;
		UpdateEntityBadge();
		UpdateEntityDescription();
		ShowFragments(entity);
		UpdateFragments();
		_root.ToggleDisplayStyle(visible: true);
	}

	private void Hide()
	{
		_root.ToggleDisplayStyle(visible: false);
		ClearFragments();
		_shownEntity = null;
	}

	private void UpdateEntityBadge()
	{
		NamedEntity component = _shownEntity.GetComponent<NamedEntity>();
		string entityName = component.EntityName;
		string entitySubtitle = _entityBadgeService.GetEntitySubtitle(_shownEntity);
		_entityClickableSubtitle = _entityBadgeService.GetEntityClickableSubtitle(_shownEntity);
		Sprite entityAvatar = _entityBadgeService.GetEntityAvatar(_shownEntity);
		_entityNameText.text = entityName;
		_entitySubtitle.text = entitySubtitle;
		_entitySubtitle.ToggleDisplayStyle(!string.IsNullOrEmpty(entitySubtitle));
		_entityClickableSubtitleButton.text = _entityClickableSubtitle.Subtitle;
		_entityClickableSubtitleButton.ToggleDisplayStyle(_entityClickableSubtitle.HasAction);
		_entityClickableSubtitleWarningIcon.ToggleDisplayStyle(_entityClickableSubtitle.HasWarning);
		_entityAvatar.style.backgroundImage = new StyleBackground(entityAvatar);
		_entityAvatar.ToggleDisplayStyle(entityAvatar != null);
		_entityNameButton.SetEnabled(component.IsEditable);
		_entityNameHint.ToggleDisplayStyle(component.IsEditable);
	}

	private void UpdateEntityDescription()
	{
		_entityDescriptionService.DescribeAsSingleSection(_shownEntity, _entityDescription);
		bool flag = _entityDescription.childCount == 0;
		_entityDescription.EnableInClassList(DescriptionNoneClass, flag);
		_entityDescriptionHider.EnableInClassList(HiderNoneClass, flag);
		_entityDescriptionHider.SetEnabled(!flag);
	}

	private void ShowFragments(EntityComponent entity)
	{
		foreach (IEntityPanelFragment entityPanelFragment in _entityPanelFragments)
		{
			entityPanelFragment.ShowFragment(entity);
		}
		_diagnosticFragmentController.ShowFragments(entity);
	}

	private void ClearFragments()
	{
		foreach (IEntityPanelFragment entityPanelFragment in _entityPanelFragments)
		{
			entityPanelFragment.ClearFragment();
		}
		_entityDescription.Clear();
		_diagnosticFragmentController.ClearFragments();
	}

	private void UpdateFragments()
	{
		foreach (IEntityPanelFragment entityPanelFragment in _entityPanelFragments)
		{
			entityPanelFragment.UpdateFragment();
		}
		_diagnosticFragmentController.UpdateFragments();
	}

	private void ToggleEntityDescription(ClickEvent evt)
	{
		_entityDescription.EnableInClassList(DescriptionHiddenClass, DescriptionVisible);
		_entityDescriptionHider.EnableInClassList(HiderShowClass, DescriptionVisible);
	}

	private string GetEntityNameTooltip()
	{
		if (_shownEntity?.GetComponent<NamedEntity>()?.IsEditable != true)
		{
			return null;
		}
		return _loc.T(RenameLocKey);
	}

	private void OnEntityNameClicked(ClickEvent evt)
	{
		_entityNameDialog.Show(_shownEntity.GetComponent<NamedEntity>());
	}

	private void PerformSubtitleButtonClickAction(ClickEvent evt)
	{
		if (_entityClickableSubtitle.HasAction)
		{
			_entityClickableSubtitle.ClickAction();
		}
	}

	private static IEnumerable<IEntityPanelFragment> Order(IEnumerable<OrderedEntityPanelFragment> input)
	{
		return from fragment in input
			orderby fragment.Order
			select fragment.Fragment;
	}
}
