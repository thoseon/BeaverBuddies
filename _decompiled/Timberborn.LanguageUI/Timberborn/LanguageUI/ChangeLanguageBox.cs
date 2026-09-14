using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.Language;
using Timberborn.Localization;
using Timberborn.MainMenuSceneLoading;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.LanguageUI;

public class ChangeLanguageBox : IPanelController, ILoadableSingleton
{
	private class ChangeLanguageItem
	{
		public Toggle Toggle { get; }

		public string LocalizationCode { get; }

		public ChangeLanguageItem(Toggle toggle, string localizationCode)
		{
			Toggle = toggle;
			LocalizationCode = localizationCode;
		}
	}

	private static readonly string LanguageNameKey = "Settings.Language.Name";

	private static readonly string WarningLocKey = "Settings.Language.Warning";

	private static readonly string RestartLocKey = "Settings.Language.Restart";

	private static readonly string[] LanguagesWithForcedNewMarker = new string[0];

	private readonly VisualElementLoader _visualElementLoader;

	private readonly PanelStack _panelStack;

	private readonly ILoc _loc;

	private readonly LanguageSettings _languageSettings;

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly ILocalizationService _localizationService;

	private readonly MainMenuSceneLoader _mainMenuSceneLoader;

	private readonly List<ChangeLanguageItem> _items = new List<ChangeLanguageItem>();

	private bool _skipReloadConfirmation;

	private Action _closedCallback;

	private VisualElement _root;

	private bool _isShown;

	public string LocalizedCurrentLanguageName => _loc.T(LanguageNameKey);

	public ChangeLanguageBox(VisualElementLoader visualElementLoader, PanelStack panelStack, ILoc loc, LanguageSettings languageSettings, DialogBoxShower dialogBoxShower, ILocalizationService localizationService, MainMenuSceneLoader mainMenuSceneLoader)
	{
		_visualElementLoader = visualElementLoader;
		_panelStack = panelStack;
		_loc = loc;
		_languageSettings = languageSettings;
		_dialogBoxShower = dialogBoxShower;
		_localizationService = localizationService;
		_mainMenuSceneLoader = mainMenuSceneLoader;
	}

	public void ShowWithoutReloadConfirmation(Action closedCallback = null)
	{
		Show(skipReloadConfirmation: true, closedCallback);
	}

	public void ShowWithReloadConfirmation()
	{
		Show(skipReloadConfirmation: false, null);
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Options/ChangeLanguageBox");
		_root.Q<Button>("CancelButton").RegisterCallback<ClickEvent>(delegate
		{
			Close();
		});
		AddRows(_root);
		_root.Q<Button>("ConfirmButton").RegisterCallback<ClickEvent>(delegate
		{
			OnConfirmClicked();
		});
	}

	public VisualElement GetPanel()
	{
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

	private void Show(bool skipReloadConfirmation, Action closedCallback)
	{
		if (!_isShown)
		{
			_skipReloadConfirmation = skipReloadConfirmation;
			_closedCallback = closedCallback;
			_panelStack.HideAndPushOverlay(this);
			_isShown = true;
			if (!TrySetInitialToggle(_localizationService.CurrentLanguage))
			{
				TrySetInitialToggle(LocalizationCodes.Default);
			}
		}
	}

	private void AddRows(VisualElement root)
	{
		VisualElement items = root.Q<VisualElement>("Items");
		List<LanguageInfo> list = _localizationService.AvailableLanguages.ToList();
		bool flag = list.All((LanguageInfo language) => language.IsNew);
		foreach (LanguageInfo item in list)
		{
			string localizationCode = item.LocalizationCode;
			bool showAsNew = (!flag && item.IsNew) || Enumerable.Contains(LanguagesWithForcedNewMarker, localizationCode);
			AddRow(items, localizationCode, item.DisplayName, showAsNew);
		}
	}

	private void AddRow(VisualElement items, string localizationCode, string displayName, bool showAsNew)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Options/ChangeLanguageItem");
		Toggle toggle = visualElement.Q<Toggle>("Item");
		toggle.text = displayName;
		visualElement.Q<Label>("NewLanguageMarker").ToggleDisplayStyle(showAsNew);
		items.Add(visualElement);
		_items.Add(new ChangeLanguageItem(toggle, localizationCode));
		toggle.RegisterValueChangedCallback(delegate
		{
			SetValue(toggle);
		});
	}

	private void SetValue(Toggle toggle)
	{
		foreach (ChangeLanguageItem item in _items)
		{
			item.Toggle.SetValueWithoutNotify(newValue: false);
		}
		toggle.SetValueWithoutNotify(newValue: true);
	}

	private void OnConfirmClicked()
	{
		string newLanguage = _items.Single((ChangeLanguageItem item) => item.Toggle.value).LocalizationCode;
		if (newLanguage == _localizationService.CurrentLanguage)
		{
			Close();
			return;
		}
		_languageSettings.Language = newLanguage;
		if (_skipReloadConfirmation)
		{
			ChangeAndReload(newLanguage);
			return;
		}
		_dialogBoxShower.Create().SetLocalizedMessage(WarningLocKey).SetConfirmButton(Close, _loc.T(CommonLocKeys.OKKey))
			.SetCancelButton(delegate
			{
				ChangeAndReload(newLanguage);
			}, _loc.T(RestartLocKey))
			.Show();
	}

	private void ChangeAndReload(string newLanguage)
	{
		_localizationService.Load(newLanguage);
		_mainMenuSceneLoader.SaveAndOpenMainMenu();
	}

	private void Close()
	{
		_panelStack.Pop(this);
		_closedCallback?.Invoke();
		_isShown = false;
	}

	private bool TrySetInitialToggle(string localizationCode)
	{
		bool flag = false;
		foreach (ChangeLanguageItem item in _items)
		{
			bool flag2 = item.LocalizationCode == localizationCode;
			flag |= flag2;
			item.Toggle.SetValueWithoutNotify(flag2);
		}
		return flag;
	}
}
