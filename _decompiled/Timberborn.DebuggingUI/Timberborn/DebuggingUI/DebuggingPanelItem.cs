using Timberborn.CoreUI;
using Timberborn.SettingsSystem;
using UnityEngine.UIElements;

namespace Timberborn.DebuggingUI;

internal class DebuggingPanelItem
{
	private readonly ISettings _settings;

	private readonly IDebuggingPanel _debuggingPanel;

	private readonly VisualElement _root;

	private readonly string _title;

	private Label _infoLabel;

	private Button _showButton;

	private Button _hideButton;

	private bool _isVisible;

	public DebuggingPanelItem(ISettings settings, IDebuggingPanel debuggingPanel, VisualElement root, string title)
	{
		_settings = settings;
		_debuggingPanel = debuggingPanel;
		_root = root;
		_title = title;
	}

	public void Initialize()
	{
		_root.Q<Label>("Title").text = _title;
		_infoLabel = _root.Q<Label>("Info");
		_showButton = _root.Q<Button>("Show");
		_showButton.RegisterCallback<ClickEvent>(delegate
		{
			TogglePanelVisibility(isVisible: true);
		});
		_hideButton = _root.Q<Button>("Hide");
		_hideButton.RegisterCallback<ClickEvent>(delegate
		{
			TogglePanelVisibility(isVisible: false);
		});
		_isVisible = _settings.GetBool(GetKey(_title));
		UpdateElementsVisibility();
	}

	public void UpdateText()
	{
		if (_isVisible)
		{
			string text = _debuggingPanel.GetText();
			if (text != null)
			{
				_infoLabel.text = text;
			}
		}
	}

	private void TogglePanelVisibility(bool isVisible)
	{
		_settings.SetBool(GetKey(_title), _isVisible = isVisible);
		UpdateElementsVisibility();
	}

	private void UpdateElementsVisibility()
	{
		_showButton.ToggleDisplayStyle(!_isVisible);
		_hideButton.ToggleDisplayStyle(_isVisible);
		_infoLabel.ToggleDisplayStyle(_isVisible);
	}

	private static string GetKey(string title)
	{
		return "DebuggingPanel." + title;
	}
}
