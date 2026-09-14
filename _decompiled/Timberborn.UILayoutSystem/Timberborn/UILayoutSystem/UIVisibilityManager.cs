using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.Rendering;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.UILayoutSystem;

public class UIVisibilityManager : IInputProcessor, ILoadableSingleton
{
	private static readonly string ToggleGUIKey = "ToggleGUI";

	private static readonly string CancelKey = "Cancel";

	private readonly InputService _inputService;

	private readonly EventBus _eventBus;

	public bool GUIVisible { get; private set; } = true;

	public UIVisibilityManager(InputService inputService, EventBus eventBus)
	{
		_inputService = inputService;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
		_inputService.AddInputProcessor(this);
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyDown(ToggleGUIKey))
		{
			ToggleGUIVisibility();
			return true;
		}
		if (!GUIVisible && _inputService.IsKeyDown(CancelKey))
		{
			ToggleGUIVisibility();
			return true;
		}
		return false;
	}

	[OnEvent]
	public void OnPanelShown(PanelShownEvent panelShownEvent)
	{
		if (!GUIVisible)
		{
			ToggleGUIVisibility();
		}
	}

	private void ToggleGUIVisibility()
	{
		GUIVisible = !GUIVisible;
		Camera main = Camera.main;
		int cullingMask = main.cullingMask;
		LayerMask uIMask = Layers.UIMask;
		if (GUIVisible)
		{
			main.cullingMask = cullingMask | (int)uIMask;
		}
		else
		{
			main.cullingMask = cullingMask & ~(int)uIMask;
		}
		_eventBus.Post(new UIVisibilityChangedEvent(GUIVisible));
	}
}
