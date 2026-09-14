using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.Automation;
using Timberborn.AutomationBuildings;
using Timberborn.AutomationUI;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityNaming;
using Timberborn.EntitySystem;
using Timberborn.InputSystem;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using Timberborn.UISound;
using UnityEngine.UIElements;

namespace Timberborn.AutomationBuildingsUI;

internal class PinnedLeversPanel : IPostLoadableSingleton, IInputProcessor
{
	private static readonly string EnableHoverClass = "hover-enabled";

	private static readonly ImmutableArray<string> PinnedLeverKeys = ImmutableArray.Create<string>("PinnedLever1", "PinnedLever2", "PinnedLever3", "PinnedLever4", "PinnedLever5", "PinnedLever6", "PinnedLever7", "PinnedLever8", "PinnedLever9", "PinnedLever10");

	private readonly VisualElementLoader _visualElementLoader;

	private readonly UILayout _uiLayout;

	private readonly EntityComponentRegistry _entityComponentRegistry;

	private readonly EventBus _eventBus;

	private readonly AutomationStateIconBuilder _automationStateIconBuilder;

	private readonly UISoundController _uiSoundController;

	private readonly InputService _inputService;

	private VisualElement _root;

	private VisualElement _leversContainer;

	private readonly Dictionary<Lever, AutomationStateIcon> _leverStateIcons = new Dictionary<Lever, AutomationStateIcon>();

	private readonly List<Lever> _levers = new List<Lever>();

	public PinnedLeversPanel(VisualElementLoader visualElementLoader, UILayout uiLayout, EntityComponentRegistry entityComponentRegistry, EventBus eventBus, AutomationStateIconBuilder automationStateIconBuilder, UISoundController uiSoundController, InputService inputService)
	{
		_visualElementLoader = visualElementLoader;
		_uiLayout = uiLayout;
		_entityComponentRegistry = entityComponentRegistry;
		_eventBus = eventBus;
		_automationStateIconBuilder = automationStateIconBuilder;
		_uiSoundController = uiSoundController;
		_inputService = inputService;
	}

	public void PostLoad()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/AutomationPins/PinnedLeversPanel");
		_leversContainer = _root.Q<VisualElement>("Levers");
		Recreate();
		_eventBus.Register(this);
		_inputService.AddInputProcessor(this);
	}

	public bool ProcessInput()
	{
		int num = Math.Min(_levers.Count, PinnedLeverKeys.Length);
		for (int i = 0; i < num; i++)
		{
			string keyId = PinnedLeverKeys[i];
			Lever lever = _levers[i];
			if ((bool)lever)
			{
				if (_inputService.IsKeyDown(keyId))
				{
					lever.Press();
				}
				else if (_inputService.IsKeyUp(keyId))
				{
					lever.Release();
				}
			}
		}
		return false;
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		_uiLayout.AddTopLeft(_root, 40);
	}

	[OnEvent]
	public void OnPinnedLeverModified(PinnedLeverModified pinnedLeverModified)
	{
		if (_leverStateIcons.TryGetValue(pinnedLeverModified.Lever, out var value))
		{
			value.Update();
		}
	}

	[OnEvent]
	public void OnLeverPinnedChanged(LeverPinnedChangedEvent leverPinnedChangedEvent)
	{
		Recreate();
	}

	[OnEvent]
	public void OnEntityInitialized(EntityInitializedEvent entityInitializedEvent)
	{
		ReacreateIfLever(entityInitializedEvent.Entity);
	}

	[OnEvent]
	public void OnEntityDeleted(EntityDeletedEvent entityDeletedEvent)
	{
		ReacreateIfLever(entityDeletedEvent.Entity);
	}

	[OnEvent]
	public void OnEntityNameChangedEvent(EntityNameChangedEvent entityNameChangedEvent)
	{
		ReacreateIfLever(entityNameChangedEvent.Entity);
	}

	private void ReacreateIfLever(BaseComponent entity)
	{
		if (entity.HasComponent<Lever>())
		{
			Recreate();
		}
	}

	private void Recreate()
	{
		_leverStateIcons.Clear();
		_leversContainer.Clear();
		_levers.Clear();
		foreach (Lever item in from lever in _entityComponentRegistry.GetAll<Lever>()
			where lever.IsPinned
			orderby lever.GetComponent<NamedEntity>().SortingKey
			select lever)
		{
			CreateLeverItem(item);
			_levers.Add(item);
		}
		_root.ToggleDisplayStyle(_leversContainer.childCount > 0);
	}

	private void CreateLeverItem(Lever lever)
	{
		VisualElement leverItem = _visualElementLoader.LoadVisualElement("Game/AutomationPins/PinnedLever");
		Label label = leverItem.Q<Label>("Name");
		label.text = lever.LeverName;
		VisualElement visualElement = leverItem.Q<VisualElement>("State");
		visualElement.RegisterCallback<MouseEnterEvent>(delegate
		{
			leverItem.RemoveFromClassList(EnableHoverClass);
		});
		visualElement.RegisterCallback<MouseLeaveEvent>(delegate
		{
			leverItem.AddToClassList(EnableHoverClass);
		});
		Image icon = leverItem.Q<Image>("StateIcon");
		AutomationStateIcon automationStateIcon = _automationStateIconBuilder.Create(icon, lever.GetComponent<Automator>).SetClickableIcon().Build();
		automationStateIcon.Update();
		_leverStateIcons.Add(lever, automationStateIcon);
		label.RegisterCallback(delegate(PointerDownEvent evt)
		{
			OnLeverDown(evt, lever);
		}, TrickleDown.TrickleDown);
		label.RegisterCallback(delegate(PointerUpEvent evt)
		{
			OnLeverUp(evt, lever);
		}, TrickleDown.TrickleDown);
		label.RegisterCallback<MouseLeaveEvent>(delegate
		{
			OnMouseLeave(lever);
		});
		_leversContainer.Add(leverItem);
	}

	private void OnLeverDown(PointerDownEvent evt, Lever lever)
	{
		if (evt.button == 0)
		{
			_uiSoundController.PlayClickSound();
			lever.Press();
		}
	}

	private static void OnLeverUp(PointerUpEvent evt, Lever lever)
	{
		if (evt.button == 0)
		{
			lever.Release();
		}
	}

	private static void OnMouseLeave(Lever lever)
	{
		if (lever.IsSpringReturn)
		{
			lever.Release();
		}
	}
}
