using System;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.FileSystem;
using Timberborn.Localization;
using Timberborn.PlatformUtilities;
using Timberborn.SingletonSystem;
using Timberborn.WebNavigation;
using UnityEngine.UIElements;

namespace Timberborn.MainMenuModdingUI;

public class CreateModBox : IPanelController, ILoadableSingleton
{
	private static readonly string InvalidNameLocKey = "Saving.InvalidName";

	private static readonly string ModCreatedMessageLocKey = "Modding.ModCreatedMessage";

	private static readonly string ModNameTakenLocKey = "Modding.ModNameTaken";

	private static readonly string DocumentationButtonLocKey = "Modding.DocumentationButton";

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly IExplorerOpener _explorerOpener;

	private readonly UrlOpener _urlOpener;

	private readonly ILoc _loc;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly ModTemplateDropdownProvider _modTemplateDropdownProvider;

	private readonly PanelStack _panelStack;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly HyperlinkInitializer _hyperlinkInitializer;

	private readonly ModCreator _modCreator;

	private VisualElement _root;

	private TextField _modNameField;

	private VisualElement _languageCodeWrapper;

	private TextField _languageCodeField;

	private Button _createModButton;

	public CreateModBox(DialogBoxShower dialogBoxShower, IExplorerOpener explorerOpener, UrlOpener urlOpener, ILoc loc, DropdownItemsSetter dropdownItemsSetter, ModTemplateDropdownProvider modTemplateDropdownProvider, PanelStack panelStack, VisualElementLoader visualElementLoader, HyperlinkInitializer hyperlinkInitializer, ModCreator modCreator)
	{
		_dialogBoxShower = dialogBoxShower;
		_explorerOpener = explorerOpener;
		_urlOpener = urlOpener;
		_loc = loc;
		_dropdownItemsSetter = dropdownItemsSetter;
		_modTemplateDropdownProvider = modTemplateDropdownProvider;
		_panelStack = panelStack;
		_visualElementLoader = visualElementLoader;
		_hyperlinkInitializer = hyperlinkInitializer;
		_modCreator = modCreator;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Modding/CreateModBox");
		_root.Q<Button>("CloseButton").RegisterCallback<ClickEvent>(delegate
		{
			OnUICancelled();
		});
		Dropdown dropdown = _root.Q<Dropdown>("TemplateDropdown");
		_dropdownItemsSetter.SetItems(dropdown, _modTemplateDropdownProvider);
		dropdown.ValueChanged += delegate
		{
			UpdateControlsState();
		};
		_modNameField = _root.Q<TextField>("ModNameField");
		_modNameField.RegisterValueChangedCallback(delegate
		{
			UpdateControlsState();
		});
		_languageCodeWrapper = _root.Q<VisualElement>("LanguageCodeWrapper");
		_languageCodeField = _root.Q<TextField>("LanguageCodeField");
		_languageCodeField.RegisterValueChangedCallback(delegate
		{
			UpdateControlsState();
		});
		_createModButton = _root.Q<Button>("CreateModButton");
		_createModButton.RegisterCallback<ClickEvent>(CreateTemplate);
	}

	public VisualElement GetPanel()
	{
		return _root;
	}

	public bool OnUIConfirmed()
	{
		if (_createModButton.enabledSelf)
		{
			CreateTemplate();
			return true;
		}
		return false;
	}

	public void OnUICancelled()
	{
		_panelStack.Pop(this);
	}

	public void Open()
	{
		_panelStack.HideAndPushOverlay(this);
		UpdateControlsState();
	}

	private void UpdateControlsState()
	{
		bool localizationTemplateChosen = _modTemplateDropdownProvider.LocalizationTemplateChosen;
		_languageCodeWrapper.ToggleDisplayStyle(localizationTemplateChosen);
		bool flag = string.IsNullOrEmpty(_modNameField.value) || (localizationTemplateChosen && string.IsNullOrEmpty(_languageCodeField.value));
		_createModButton.SetEnabled(!flag);
	}

	private void CreateTemplate(ClickEvent evt)
	{
		CreateTemplate();
	}

	private void CreateTemplate()
	{
		string modName = _modNameField.text.Trim();
		string localizationCode = (_modTemplateDropdownProvider.LocalizationTemplateChosen ? _languageCodeField.text.Trim() : null);
		string destinationPath;
		switch (_modCreator.CreateMod(modName, localizationCode, out destinationPath))
		{
		case DirectoryCreationResult.OK:
			ShowModCreatedMessage(destinationPath);
			break;
		case DirectoryCreationResult.NameTaken:
			ShowDialogBox(ModNameTakenLocKey);
			break;
		case DirectoryCreationResult.NameInvalid:
			ShowDialogBox(InvalidNameLocKey);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private void ShowModCreatedMessage(string destinationPath)
	{
		_panelStack.Pop(this);
		string text = _loc.T(ModCreatedMessageLocKey, destinationPath);
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Modding/ModCreatedMessage");
		Label label = visualElement.Q<Label>("Message");
		label.text = text;
		_hyperlinkInitializer.Initialize(label, delegate
		{
			_explorerOpener.OpenDirectory(destinationPath);
		});
		_dialogBoxShower.Create().AddContent(visualElement).SetInfoButton(delegate
		{
			_urlOpener.OpenModdingDocumentation();
		}, _loc.T(DocumentationButtonLocKey))
			.HideTopAndShow();
	}

	private void ShowDialogBox(string textLocKey)
	{
		_dialogBoxShower.Create().SetLocalizedMessage(textLocKey).Show();
	}
}
