using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.AutomationBuildings;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityNaming;
using Timberborn.EntitySystem;
using Timberborn.Illumination;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace Timberborn.AutomationBuildingsUI;

public class PinnedIndicatorsPanel : IPostLoadableSingleton
{
	private record IndicatorItem(VisualElement Root, Image StateIcon);

	private static readonly string StateIconOnClass = "automation-state-icon--on";

	private static readonly string StateIconUnfinishedClass = "automation-state-icon--unfinished";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly UILayout _uiLayout;

	private readonly EntityComponentRegistry _entityComponentRegistry;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly EventBus _eventBus;

	private VisualElement _root;

	private VisualElement _indicatorsContainer;

	private readonly Dictionary<Indicator, IndicatorItem> _indicatorItems = new Dictionary<Indicator, IndicatorItem>();

	public PinnedIndicatorsPanel(VisualElementLoader visualElementLoader, UILayout uiLayout, EntityComponentRegistry entityComponentRegistry, EntitySelectionService entitySelectionService, EventBus eventBus)
	{
		_visualElementLoader = visualElementLoader;
		_uiLayout = uiLayout;
		_entityComponentRegistry = entityComponentRegistry;
		_entitySelectionService = entitySelectionService;
		_eventBus = eventBus;
	}

	public void PostLoad()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/AutomationPins/PinnedIndicatorsPanel");
		_indicatorsContainer = _root.Q<VisualElement>("Indicators");
		Recreate();
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		_uiLayout.AddTopLeft(_root, 40);
	}

	[OnEvent]
	public void OnIndicatorPinnedModeChanged(IndicatorPinnedModeChangedEvent indicatorPinnedModeChangedEvent)
	{
		Recreate();
	}

	[OnEvent]
	public void OnEntityInitialized(EntityInitializedEvent entityInitializedEvent)
	{
		RecreateIfIndicator(entityInitializedEvent.Entity);
	}

	[OnEvent]
	public void OnEntityDeleted(EntityDeletedEvent entityDeletedEvent)
	{
		Indicator component = entityDeletedEvent.Entity.GetComponent<Indicator>();
		if (component != null)
		{
			UnsubscribeFromModification(component);
			Recreate();
		}
	}

	[OnEvent]
	public void OnEntityNameChangedEvent(EntityNameChangedEvent entityNameChangedEvent)
	{
		RecreateIfIndicator(entityNameChangedEvent.Entity);
	}

	private void RecreateIfIndicator(BaseComponent entity)
	{
		if (entity.HasComponent<Indicator>())
		{
			Recreate();
		}
	}

	private void Recreate()
	{
		_indicatorsContainer.Clear();
		_indicatorItems.Clear();
		foreach (Indicator item in from indicator in _entityComponentRegistry.GetAll<Indicator>()
			where indicator.PinnedMode != IndicatorPinnedMode.Never
			orderby indicator.GetComponent<NamedEntity>().SortingKey
			select indicator)
		{
			CreateIndicatorItem(item);
		}
		_root.ToggleDisplayStyle(_indicatorsContainer.childCount > 0);
	}

	private void CreateIndicatorItem(Indicator indicator)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/AutomationPins/PinnedIndicator");
		visualElement.Q<Label>("Name").text = indicator.IndicatorName;
		Image stateIcon = visualElement.Q<Image>("StateIcon");
		visualElement.RegisterCallback<ClickEvent>(delegate
		{
			OnIndicatorClicked(indicator);
		});
		IndicatorItem indicatorItem = new IndicatorItem(visualElement, stateIcon);
		UpdateIndicatorItem(indicator, indicatorItem);
		_indicatorItems.Add(indicator, indicatorItem);
		_indicatorsContainer.Add(visualElement);
		SubscribeToModification(indicator);
	}

	private static void UpdateIndicatorItem(Indicator indicator, IndicatorItem indicatorItem)
	{
		indicatorItem.Root.ToggleDisplayStyle(indicator.State || indicator.PinnedMode == IndicatorPinnedMode.Always);
		indicatorItem.StateIcon.style.unityBackgroundImageTintColor = indicator.GetComponent<CustomizableIlluminator>().IconColor;
		indicatorItem.StateIcon.EnableInClassList(StateIconOnClass, indicator.State);
		indicatorItem.StateIcon.EnableInClassList(StateIconUnfinishedClass, !indicator.Enabled);
	}

	private void OnIndicatorClicked(Indicator indicator)
	{
		_entitySelectionService.SelectAndFocusOn(indicator);
	}

	private void SubscribeToModification(Indicator indicator)
	{
		UnsubscribeFromModification(indicator);
		indicator.PinnedIndicatorModified += OnPinnedIndicatorModified;
	}

	private void UnsubscribeFromModification(Indicator indicator)
	{
		indicator.PinnedIndicatorModified -= OnPinnedIndicatorModified;
	}

	private void OnPinnedIndicatorModified(object sender, EventArgs e)
	{
		Indicator indicator = (Indicator)sender;
		if (_indicatorItems.TryGetValue(indicator, out var value))
		{
			UpdateIndicatorItem(indicator, value);
		}
	}
}
