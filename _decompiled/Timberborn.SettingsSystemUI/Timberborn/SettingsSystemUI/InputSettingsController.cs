using System;
using Timberborn.InputSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.SettingsSystemUI;

internal class InputSettingsController
{
	private static readonly float MaxSliderValue = 3f;

	private static readonly float UIValueMultiplier = 2.5f;

	private readonly InputSettings _inputSettings;

	private Toggle _invertZoomToggle;

	private Toggle _swapMouseToggle;

	private Toggle _dragCameraToggle;

	private Toggle _lockCursorInWindowToggle;

	private Toggle _edgePanCameraToggle;

	private Label _edgePanCameraSpeedValueLabel;

	private Slider _edgePanCameraSpeedSlider;

	private Label _keyboardCameraMovementSpeedValueLabel;

	private Slider _keyboardCameraMovementSpeedSlider;

	private Label _keyboardCameraRotationSpeedValueLabel;

	private Slider _keyboardCameraRotationSpeedSlider;

	private Label _keyboardCameraZoomSpeedValueLabel;

	private Slider _keyboardCameraZoomSpeedSlider;

	private Label _mouseWheelCameraZoomSpeedValueLabel;

	private Slider _mouseWheelCameraZoomSpeedSlider;

	private Label _mouseCameraRotationSpeedValueLabel;

	private Slider _mouseCameraRotationSpeedSlider;

	private static float ReverseUIMultiplier => 1f / UIValueMultiplier;

	public InputSettingsController(InputSettings inputSettings)
	{
		_inputSettings = inputSettings;
	}

	public void Initialize(VisualElement root)
	{
		_invertZoomToggle = root.Q<Toggle>("InvertZoom");
		_invertZoomToggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> v)
		{
			_inputSettings.InvertZoom = v.newValue;
		});
		_swapMouseToggle = root.Q<Toggle>("SwapMouseCameraMovementWithRotation");
		_swapMouseToggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> v)
		{
			_inputSettings.SwapMouseCameraMovementWithRotation = v.newValue;
		});
		_dragCameraToggle = root.Q<Toggle>("DragCamera");
		_dragCameraToggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> v)
		{
			_inputSettings.DragCamera = v.newValue;
		});
		_lockCursorInWindowToggle = root.Q<Toggle>("LockCursorInWindow");
		_lockCursorInWindowToggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> v)
		{
			_inputSettings.LockCursorInWindow = v.newValue;
		});
		_edgePanCameraToggle = root.Q<Toggle>("EdgePanCamera");
		_edgePanCameraToggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> v)
		{
			_inputSettings.EdgePanCamera = v.newValue;
		});
		VisualElement visualElement = root.Q<VisualElement>("EdgePanCameraSpeed");
		_edgePanCameraSpeedValueLabel = visualElement.Q<Label>("Value");
		_edgePanCameraSpeedSlider = InitializeSlider(visualElement, _edgePanCameraSpeedValueLabel, delegate(float v)
		{
			_inputSettings.EdgePanCameraSpeed = v;
		}, ReverseUIMultiplier);
		VisualElement visualElement2 = root.Q<VisualElement>("KeyboardCameraMovementSpeed");
		_keyboardCameraMovementSpeedValueLabel = visualElement2.Q<Label>("Value");
		_keyboardCameraMovementSpeedSlider = InitializeSlider(visualElement2, _keyboardCameraMovementSpeedValueLabel, delegate(float v)
		{
			_inputSettings.KeyboardCameraMovementSpeed = v;
		}, ReverseUIMultiplier);
		VisualElement visualElement3 = root.Q<VisualElement>("KeyboardCameraRotationSpeed");
		_keyboardCameraRotationSpeedValueLabel = visualElement3.Q<Label>("Value");
		_keyboardCameraRotationSpeedSlider = InitializeSlider(visualElement3, _keyboardCameraRotationSpeedValueLabel, delegate(float v)
		{
			_inputSettings.KeyboardCameraRotationSpeed = v;
		}, ReverseUIMultiplier);
		VisualElement visualElement4 = root.Q<VisualElement>("KeyboardCameraZoomSpeed");
		_keyboardCameraZoomSpeedValueLabel = visualElement4.Q<Label>("Value");
		_keyboardCameraZoomSpeedSlider = InitializeSlider(visualElement4, _keyboardCameraZoomSpeedValueLabel, delegate(float v)
		{
			_inputSettings.KeyboardCameraZoomSpeed = v;
		}, ReverseUIMultiplier);
		VisualElement visualElement5 = root.Q<VisualElement>("MouseWheelCameraZoomSpeed");
		_mouseWheelCameraZoomSpeedValueLabel = visualElement5.Q<Label>("Value");
		_mouseWheelCameraZoomSpeedSlider = InitializeSlider(visualElement5, _mouseWheelCameraZoomSpeedValueLabel, delegate(float v)
		{
			_inputSettings.MouseWheelCameraZoomSpeed = v;
		}, ReverseUIMultiplier);
		VisualElement visualElement6 = root.Q<VisualElement>("MouseCameraRotationSpeed");
		_mouseCameraRotationSpeedValueLabel = visualElement6.Q<Label>("Value");
		_mouseCameraRotationSpeedSlider = InitializeSlider(visualElement6, _mouseCameraRotationSpeedValueLabel, delegate(float v)
		{
			_inputSettings.MouseCameraRotationSpeed = v;
		}, ReverseUIMultiplier);
	}

	public void Update()
	{
		_invertZoomToggle.SetValueWithoutNotify(_inputSettings.InvertZoom);
		_swapMouseToggle.SetValueWithoutNotify(_inputSettings.SwapMouseCameraMovementWithRotation);
		_dragCameraToggle.SetValueWithoutNotify(_inputSettings.DragCamera);
		_lockCursorInWindowToggle.SetValueWithoutNotify(_inputSettings.LockCursorInWindow);
		_edgePanCameraToggle.SetValueWithoutNotify(_inputSettings.EdgePanCamera);
		UpdateSlider(_edgePanCameraSpeedSlider, _edgePanCameraSpeedValueLabel, _inputSettings.EdgePanCameraSpeed);
		UpdateSlider(_keyboardCameraMovementSpeedSlider, _keyboardCameraMovementSpeedValueLabel, _inputSettings.KeyboardCameraMovementSpeed);
		UpdateSlider(_keyboardCameraRotationSpeedSlider, _keyboardCameraRotationSpeedValueLabel, _inputSettings.KeyboardCameraRotationSpeed);
		UpdateSlider(_keyboardCameraZoomSpeedSlider, _keyboardCameraZoomSpeedValueLabel, _inputSettings.KeyboardCameraZoomSpeed);
		UpdateSlider(_mouseWheelCameraZoomSpeedSlider, _mouseWheelCameraZoomSpeedValueLabel, _inputSettings.MouseWheelCameraZoomSpeed);
		UpdateSlider(_mouseCameraRotationSpeedSlider, _mouseCameraRotationSpeedValueLabel, _inputSettings.MouseCameraRotationSpeed);
	}

	private static Slider InitializeSlider(VisualElement root, TextElement valueLabel, Action<float> setter, float multiplier)
	{
		Slider slider = root.Q<Slider>("Slider");
		slider.lowValue = 0f;
		slider.highValue = MaxSliderValue;
		slider.RegisterValueChangedCallback(delegate(ChangeEvent<float> v)
		{
			setter(Mathf.Clamp(v.newValue * multiplier, 0f, MaxSliderValue));
			valueLabel.text = v.newValue.ToString("P0");
		});
		return slider;
	}

	private static void UpdateSlider(Slider slider, Label label, float value)
	{
		float num = Mathf.Clamp(value, 0f, MaxSliderValue);
		slider.SetValueWithoutNotify(num * UIValueMultiplier);
		label.text = (UIValueMultiplier * num).ToString("P0");
	}
}
