using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Explosions;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.ExplosionsUI;

internal class DynamiteFragment : IEntityPanelFragment, IInputProcessor
{
	private static readonly string ArmedLocKey = "Building.Dynamite.Armed";

	private static readonly string CantDetonateLocKey = "Building.Dynamite.CantDetonate";

	private static readonly string DetonateLocKey = "Building.Dynamite.Detonate";

	private static readonly string DetonationDelayKey = "DetonationDelay";

	private static readonly string LongDetonationDelayKey = "LongDetonationDelay";

	private static readonly string UniqueBuildingActionKey = "UniqueBuildingAction";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly InputService _inputService;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private Button _button;

	private Dynamite _dynamite;

	private VisualElement _root;

	public DynamiteFragment(VisualElementLoader visualElementLoader, ILoc loc, InputService inputService, ITooltipRegistrar tooltipRegistrar)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
		_inputService = inputService;
		_tooltipRegistrar = tooltipRegistrar;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/DynamiteFragment");
		_button = _root.Q<Button>("Button");
		_button.RegisterCallback<ClickEvent>(DetonateSelectedDynamite);
		_tooltipRegistrar.RegisterWithKeyBinding(_button, _loc.T(DetonateLocKey), UniqueBuildingActionKey);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyDown(UniqueBuildingActionKey))
		{
			DetonateSelectedDynamite();
			return true;
		}
		return false;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_dynamite = entity.GetComponent<Dynamite>();
		if ((bool)(BaseComponent)(object)_dynamite)
		{
			_inputService.AddInputProcessor(this);
		}
	}

	public void ClearFragment()
	{
		_dynamite = null;
		_root.ToggleDisplayStyle(visible: false);
		_inputService.RemoveInputProcessor(this);
	}

	public void UpdateFragment()
	{
		if ((bool)(BaseComponent)(object)_dynamite && _dynamite.IsFinished)
		{
			UpdateButton();
			_root.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	private void UpdateButton()
	{
		if (!_dynamite.IsFinished)
		{
			UpdateButton(_loc.T(CantDetonateLocKey), interactable: false);
		}
		else if (_dynamite.IsTriggered)
		{
			UpdateButton(_loc.T(ArmedLocKey), interactable: false);
		}
		else
		{
			UpdateButton(_loc.T(DetonateLocKey), interactable: true);
		}
	}

	private void UpdateButton(string text, bool interactable)
	{
		_button.text = text;
		_button.SetEnabled(interactable);
	}

	private void DetonateSelectedDynamite(ClickEvent evt)
	{
		DetonateSelectedDynamite();
	}

	private void DetonateSelectedDynamite()
	{
		if ((bool)(BaseComponent)(object)_dynamite)
		{
			if (_inputService.IsKeyHeld(DetonationDelayKey))
			{
				_dynamite.TriggerDelayed(10);
			}
			else if (_inputService.IsKeyHeld(LongDetonationDelayKey))
			{
				_dynamite.TriggerDelayed(20);
			}
			else
			{
				_dynamite.Trigger();
			}
		}
	}
}
