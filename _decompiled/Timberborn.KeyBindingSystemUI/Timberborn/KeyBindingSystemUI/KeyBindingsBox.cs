using System.Collections.Immutable;
using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.KeyBindingSystem;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.KeyBindingSystemUI;

public class KeyBindingsBox : IPanelController, ILoadableSingleton
{
	private static readonly string ResetToDefaultMessageLocKey = "KeyBindingBox.ResetToDefaultMessage";

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly KeyBindingSpecService _keyBindingSpecService;

	private readonly KeyBindingRowFactory _keyBindingRowFactory;

	private readonly PanelStack _panelStack;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly DevModeManager _devModeManager;

	private VisualElement _root;

	private ScrollView _content;

	private ImmutableArray<KeyBindingGroup> _keyBindingGroups;

	public KeyBindingsBox(DialogBoxShower dialogBoxShower, KeyBindingSpecService keyBindingSpecService, KeyBindingRowFactory keyBindingRowFactory, PanelStack panelStack, VisualElementLoader visualElementLoader, DevModeManager devModeManager)
	{
		_dialogBoxShower = dialogBoxShower;
		_keyBindingSpecService = keyBindingSpecService;
		_keyBindingRowFactory = keyBindingRowFactory;
		_panelStack = panelStack;
		_visualElementLoader = visualElementLoader;
		_devModeManager = devModeManager;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Options/KeyBindingsBox");
		_content = _root.Q<ScrollView>("Content");
		_keyBindingGroups = _keyBindingRowFactory.CreateAll().ToImmutableArray();
		foreach (KeyBindingGroup keyBindingGroup in _keyBindingGroups)
		{
			_content.Add(keyBindingGroup.Root);
		}
		_root.Q<Button>("CloseButton").RegisterCallback<ClickEvent>(delegate
		{
			Close();
		});
		_root.Q<Button>("ResetToDefault").RegisterCallback<ClickEvent>(ShowResetDialogBox);
	}

	public VisualElement GetPanel()
	{
		UpdateGroupsVisibility();
		return _root;
	}

	public bool OnUIConfirmed()
	{
		return false;
	}

	public void OnUICancelled()
	{
		Close();
	}

	private void UpdateGroupsVisibility()
	{
		foreach (KeyBindingGroup keyBindingGroup in _keyBindingGroups)
		{
			Timberborn.CoreUI.VisualElementExtensions.ToggleDisplayStyle(visible: !keyBindingGroup.IsHidden || _devModeManager.Enabled, visualElement: keyBindingGroup.Root);
		}
	}

	private void ShowResetDialogBox(ClickEvent evt)
	{
		_dialogBoxShower.Create().SetLocalizedMessage(ResetToDefaultMessageLocKey).SetConfirmButton(ResetToDefault)
			.SetDefaultCancelButton()
			.Show();
	}

	private void ResetToDefault()
	{
		_keyBindingSpecService.ResetToDefault();
	}

	private void Close()
	{
		_content.scrollOffset = Vector2.zero;
		_panelStack.Pop(this);
	}
}
