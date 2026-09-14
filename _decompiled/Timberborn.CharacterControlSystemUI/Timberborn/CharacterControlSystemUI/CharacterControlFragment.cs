using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.CharacterControlSystem;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.InputSystem;
using Timberborn.InputSystemUI;
using Timberborn.KeyBindingSystemUI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.CharacterControlSystemUI;

internal class CharacterControlFragment : IEntityPanelFragment, IInputProcessor
{
	private static readonly string CursorKey = "PickDestinationCursor";

	private static readonly string CharacterControlPickCoordinatesKey = "CharacterControlPickCoordinates";

	private readonly DebugFragmentFactory _debugFragmentFactory;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly InputService _inputService;

	private readonly CursorService _cursorService;

	private readonly CharacterControlDestinationPicker _characterControlDestinationPicker;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly InputBindingDescriber _inputBindingDescriber;

	private readonly BindableButtonFactory _bindableButtonFactory;

	private ControllableCharacter _controllableCharacter;

	private BehaviorManager _behaviorManager;

	private VisualElement _root;

	private Label _text;

	private Button _release;

	private Dropdown _animations;

	private Toggle _forcedWalking;

	private bool _pickingCoordinates;

	private BindableButton _moveButton;

	public CharacterControlFragment(DebugFragmentFactory debugFragmentFactory, VisualElementLoader visualElementLoader, InputService inputService, CursorService cursorService, CharacterControlDestinationPicker characterControlDestinationPicker, DropdownItemsSetter dropdownItemsSetter, InputBindingDescriber inputBindingDescriber, BindableButtonFactory bindableButtonFactory)
	{
		_debugFragmentFactory = debugFragmentFactory;
		_visualElementLoader = visualElementLoader;
		_inputService = inputService;
		_cursorService = cursorService;
		_characterControlDestinationPicker = characterControlDestinationPicker;
		_dropdownItemsSetter = dropdownItemsSetter;
		_inputBindingDescriber = inputBindingDescriber;
		_bindableButtonFactory = bindableButtonFactory;
	}

	public VisualElement InitializeFragment()
	{
		_root = _debugFragmentFactory.Create("CharacterControl");
		string elementName = "Game/EntityPanel/CharacterControlFragment";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		_root.Q<VisualElement>("Content").Add(visualElement);
		_text = _root.Q<Label>("Text");
		_animations = _root.Q<Dropdown>("Animations");
		_forcedWalking = _root.Q<Toggle>("ForcedWalking");
		_forcedWalking.RegisterValueChangedCallback(OnForcedWalkingChanged);
		Button button = visualElement.Q<Button>("MoveTo");
		button.RegisterCallback<ClickEvent>(delegate
		{
			PickCoordinates();
		});
		button.text = button.text + " [" + _inputBindingDescriber.GetInputBindingText(CharacterControlPickCoordinatesKey) + "]";
		_moveButton = _bindableButtonFactory.Create(button, CharacterControlPickCoordinatesKey, PickCoordinates);
		_release = visualElement.Q<Button>("Release");
		_release.RegisterCallback<ClickEvent>(ReleaseControl);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_controllableCharacter = entity.GetComponent<ControllableCharacter>();
		if ((bool)_controllableCharacter)
		{
			_behaviorManager = entity.GetComponent<BehaviorManager>();
			_forcedWalking.SetValueWithoutNotify(_controllableCharacter.ForcedWalking);
			InitializeAnimations();
			_inputService.AddInputProcessor(this);
			_moveButton.Bind();
		}
	}

	public void ClearFragment()
	{
		_inputService.RemoveInputProcessor(this);
		_moveButton.Unbind();
		_root.ToggleDisplayStyle(visible: false);
		_animations.ClearItems();
		_controllableCharacter = null;
		_behaviorManager = null;
		_text.text = "";
	}

	public void UpdateFragment()
	{
		if ((bool)_controllableCharacter)
		{
			bool underControl = _controllableCharacter.UnderControl;
			if (underControl && !_pickingCoordinates)
			{
				string name = _behaviorManager.RunningBehavior.Name;
				_text.text = ((name != "CharacterControlRootBehavior") ? ("Waiting for: " + name) : "Executing command");
			}
			_release.SetEnabled(underControl);
			_root.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	public bool ProcessInput()
	{
		if (_pickingCoordinates)
		{
			if (_inputService.MainMouseButtonDown && !_inputService.MouseOverUI)
			{
				Vector3? vector = _characterControlDestinationPicker.PickDestination();
				if (vector.HasValue)
				{
					Vector3 valueOrDefault = vector.GetValueOrDefault();
					_controllableCharacter.TakeControlAndMoveTo(valueOrDefault);
				}
			}
			if (_inputService.MainMouseButtonDown || _inputService.Cancel)
			{
				_pickingCoordinates = false;
				_cursorService.ResetCursor();
				return true;
			}
		}
		return false;
	}

	private void OnForcedWalkingChanged(ChangeEvent<bool> newValue)
	{
		if (newValue.newValue)
		{
			_controllableCharacter.EnableForcedWalking();
		}
		else
		{
			_controllableCharacter.DisableForcedWalking();
		}
	}

	private void PickCoordinates()
	{
		_text.text = "Click to pick destination";
		_pickingCoordinates = true;
		_cursorService.SetCursor(CursorKey);
	}

	private void ReleaseControl(ClickEvent evt)
	{
		_controllableCharacter.ReleaseControl();
		_text.text = "";
	}

	private void InitializeAnimations()
	{
		ControllableCharacterDropdownProvider component = _controllableCharacter.GetComponent<ControllableCharacterDropdownProvider>();
		_dropdownItemsSetter.SetItems(_animations, component);
		component.SetInitialAnimation();
	}
}
