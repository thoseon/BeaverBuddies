using Steamworks;
using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.SteamStoreSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.SteamOSUI;

internal class SteamOnScreenKeyboardController : IVisualElementInitializer
{
	private readonly InputSettings _inputSettings;

	private readonly SteamManager _steamManager;

	private TextElement _lastFocusedElement;

	public SteamOnScreenKeyboardController(InputSettings inputSettings, SteamManager steamManager)
	{
		_inputSettings = inputSettings;
		_steamManager = steamManager;
	}

	public void InitializeVisualElement(VisualElement visualElement)
	{
		bool flag = !Application.isEditor && _steamManager.Initialized;
		if (flag)
		{
			bool flag2 = ((visualElement is TextField || visualElement is IntegerField || visualElement is FloatField) ? true : false);
			flag = flag2;
		}
		if (flag)
		{
			TextElement textElement = visualElement.Q<TextElement>();
			textElement.RegisterCallback<FocusInEvent>(OnFocusIn);
			textElement.RegisterCallback<FocusOutEvent>(OnFocusOut);
			textElement.RegisterCallback<AttachToPanelEvent>(OnTextfieldGeometryChange);
			textElement.RegisterCallback<GeometryChangedEvent>(OnTextfieldGeometryChange);
		}
	}

	private void OnFocusIn(FocusInEvent focusInEvent)
	{
		if (focusInEvent.currentTarget is TextElement textElement)
		{
			_lastFocusedElement = textElement;
			if (ElementHasValidGeometry(textElement))
			{
				TryOpenOnScreenKeyboard(textElement);
			}
		}
	}

	private void OnFocusOut(FocusOutEvent focusOutEvent)
	{
		if (focusOutEvent.currentTarget is TextElement textElement && textElement == _lastFocusedElement)
		{
			HideKeyboard();
		}
	}

	private void OnTextfieldGeometryChange(EventBase eventBase)
	{
		if (eventBase.currentTarget is TextElement textElement && textElement == _lastFocusedElement && ElementHasValidGeometry(textElement))
		{
			TryOpenOnScreenKeyboard(textElement);
		}
	}

	private void TryOpenOnScreenKeyboard(TextElement textElement)
	{
		if (ShouldOpenKeyboard())
		{
			Rect worldBound = textElement.worldBound;
			SteamUtils.ShowFloatingGamepadTextInput(EFloatingGamepadTextInputMode.k_EFloatingGamepadTextInputModeModeSingleLine, (int)worldBound.min.x, (int)worldBound.min.y, (int)worldBound.width, (int)worldBound.height);
		}
	}

	private static void HideKeyboard()
	{
		SteamUtils.DismissFloatingGamepadTextInput();
	}

	private bool ShouldOpenKeyboard()
	{
		if (!SteamDeckCheck())
		{
			return SteamBigPictureCheck();
		}
		return true;
	}

	private static bool ElementHasValidGeometry(VisualElement element)
	{
		Rect worldBound = element.worldBound;
		return worldBound.x != 0f || worldBound.y != 0f || !float.IsNaN(worldBound.width) || !float.IsNaN(worldBound.height);
	}

	private bool SteamDeckCheck()
	{
		bool flag = _steamManager.Initialized;
		if (flag)
		{
			string onScreenKeyboard = _inputSettings.OnScreenKeyboard;
			bool flag2 = ((onScreenKeyboard == "Auto" || onScreenKeyboard == "Enabled") ? true : false);
			flag = flag2;
		}
		if (flag)
		{
			return SteamUtils.IsSteamRunningOnSteamDeck();
		}
		return false;
	}

	private bool SteamBigPictureCheck()
	{
		if (_steamManager.Initialized && _inputSettings.OnScreenKeyboard == "Enabled")
		{
			return SteamUtils.IsSteamInBigPictureMode();
		}
		return false;
	}
}
