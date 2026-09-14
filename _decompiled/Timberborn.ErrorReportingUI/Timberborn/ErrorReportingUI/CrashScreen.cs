using System.Collections;
using System.Text;
using Timberborn.ApplicationLifetime;
using Timberborn.AssetSystem;
using Timberborn.CoreUI;
using Timberborn.ErrorReporting;
using Timberborn.Language;
using Timberborn.Localization;
using Timberborn.Modding;
using Timberborn.PlatformUtilities;
using Timberborn.WebNavigation;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.ErrorReportingUI;

public class CrashScreen : MonoBehaviour
{
	private class LocalizationCsvValidator : ILocalizationCsvValidator
	{
		public void Validate(TextAsset textAsset)
		{
		}
	}

	private static readonly float Delay = 3f;

	private static readonly string SendingLocKey = "CrashScreen.Sending";

	private static readonly string SendSuccessLocKey = "CrashScreen.SendSuccess";

	private static readonly string SendFailLocKey = "CrashScreen.SendFail";

	private static readonly string HowToFindReportLocKey = "CrashScreen.HowToFindReport";

	private static readonly string HowToFindReportShortLocKey = "CrashScreen.HowToFindReportShort";

	private static readonly string IntroductionLocKey = "CrashScreen.Introduction";

	private static readonly string ManualInstructionsLocKey = "CrashScreen.ManualInstructions";

	private static readonly string PrivacyPolicyAcceptLocKey = "CrashScreen.PrivacyPolicyAccept";

	private static readonly string PrivacyPolicyLinkLocKey = "CrashScreen.PrivacyPolicyLink";

	private static readonly string SendReportLocKey = "CrashScreen.SendReport";

	private static readonly string TitleLocKey = "CrashScreen.Title";

	private static readonly string CommentPlaceholderLocKey = "CrashScreen.CommentPlaceholder";

	private static readonly string EmailPlaceholderLocKey = "CrashScreen.EmailPlaceholder";

	private static readonly string ModdedIntroductionLocKey = "CrashScreen.ModdedIntroduction";

	private static readonly string ModdedInstructionsLocKey = "CrashScreen.ModdedInstructions";

	private static readonly string ExitGameLocKey = "Menu.ExitGame";

	[SerializeField]
	private UIDocument _uiDocument;

	private readonly UrlOpener _urlOpener = new UrlOpener();

	private readonly Loc _loc = new Loc();

	private readonly ExplorerOpener _explorerOpener = new ExplorerOpener();

	private Toggle _privacyPolicyToggle;

	private Button _sendReportButton;

	private TextField _commentTextField;

	private TextField _emailTextField;

	public IEnumerator Start()
	{
		yield return new WaitForSecondsRealtime(Delay);
		ErrorReporter.CreateErrorReport();
		ShowUI();
	}

	private void ShowUI()
	{
		LocalizationLoader localizationLoader = new LocalizationLoader(new LocalizationCsvValidator(), new AssetLoader(new ResourceAssetProvider[1]
		{
			new ResourceAssetProvider()
		}));
		string localizationKey = PlayerPrefs.GetString(LanguageSettings.LanguageKey, LocalizationCodes.Default);
		_loc.Initialize(localizationLoader.GetLocalization(localizationKey));
		_uiDocument.enabled = true;
		VisualElement rootVisualElement = _uiDocument.rootVisualElement;
		Button button = rootVisualElement.Q<Button>("ExitButton");
		button.RegisterCallback<ClickEvent>(delegate
		{
			GameQuitter.Quit();
		});
		button.text = _loc.T(ExitGameLocKey);
		_commentTextField = rootVisualElement.Q<TextField>("Comment");
		_commentTextField.textEdition.placeholder = _loc.T(CommentPlaceholderLocKey);
		_commentTextField.textEdition.hidePlaceholderOnFocus = true;
		_commentTextField.verticalScrollerVisibility = ScrollerVisibility.Auto;
		_emailTextField = rootVisualElement.Q<TextField>("Email");
		_emailTextField.textEdition.placeholder = _loc.T(EmailPlaceholderLocKey);
		_emailTextField.textEdition.hidePlaceholderOnFocus = true;
		_sendReportButton = rootVisualElement.Q<Button>("SendReportButton");
		_sendReportButton.RegisterCallback<ClickEvent>(OnSendReportButtonClick);
		_sendReportButton.text = _loc.T(SendReportLocKey);
		_sendReportButton.SetEnabled(value: false);
		Button button2 = rootVisualElement.Q<Button>("PrivacyPolicyButton");
		button2.RegisterCallback<ClickEvent>(delegate
		{
			_urlOpener.OpenPrivacyPolicy();
		});
		button2.text = _loc.T(PrivacyPolicyLinkLocKey);
		_privacyPolicyToggle = rootVisualElement.Q<Toggle>("PrivacyPolicyToggle");
		_privacyPolicyToggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> value)
		{
			_sendReportButton.SetEnabled(value.newValue);
		});
		_privacyPolicyToggle.text = _loc.T(PrivacyPolicyAcceptLocKey);
		Button button3 = rootVisualElement.Q<Button>("ErrorReportFolder");
		button3.RegisterCallback<ClickEvent>(delegate
		{
			_explorerOpener.OpenDirectory(ErrorReporter.ErrorReportsFolder);
		});
		button3.text = ErrorReporter.ErrorReportsFolder;
		Button button4 = rootVisualElement.Q<Button>("ErrorReportFolderModded");
		button4.RegisterCallback<ClickEvent>(delegate
		{
			_explorerOpener.OpenDirectory(ErrorReporter.ErrorReportsFolder);
		});
		button4.text = ErrorReporter.ErrorReportsFolder;
		Button button5 = rootVisualElement.Q<Button>("BugWebsite");
		button5.RegisterCallback<ClickEvent>(delegate
		{
			_urlOpener.OpenBugInfo();
		});
		button5.text = UrlOpener.BugInfoUrl;
		bool flag = !ModdedState.IsModded && !CrashSceneLoader.DevModeEnabled;
		rootVisualElement.Q<VisualElement>("VanillaInfo").ToggleDisplayStyle(flag);
		rootVisualElement.Q<VisualElement>("TamperedInfo").ToggleDisplayStyle(!flag);
		rootVisualElement.Q<VisualElement>("ModdedWarning").ToggleDisplayStyle(ModdedState.IsModded);
		Button button6 = rootVisualElement.Q<Button>("ModdedWebsite");
		button6.RegisterCallback<ClickEvent>(delegate
		{
			_urlOpener.OpenHowToRemoveMods();
		});
		button6.text = UrlOpener.HowToRemoveModsUrl;
		if (!flag)
		{
			TextField textField = rootVisualElement.Q<TextField>("Exception");
			textField.value = GetExceptionText();
			textField.verticalScrollerVisibility = ScrollerVisibility.Auto;
		}
		rootVisualElement.Q<Label>("Title").text = _loc.T(TitleLocKey);
		rootVisualElement.Q<Label>("Introduction").text = _loc.T(IntroductionLocKey);
		rootVisualElement.Q<Label>("HowToFindReport").text = _loc.T(HowToFindReportLocKey);
		rootVisualElement.Q<Label>("HowToFindReportShort").text = _loc.T(HowToFindReportShortLocKey);
		rootVisualElement.Q<Label>("ManualInstructions").text = _loc.T(ManualInstructionsLocKey);
		rootVisualElement.Q<Label>("ModdedIntroduction").text = _loc.T(ModdedIntroductionLocKey);
		rootVisualElement.Q<Label>("ModdedInstructions").text = _loc.T(ModdedInstructionsLocKey);
	}

	private void OnSendReportButtonClick(ClickEvent evt)
	{
		_sendReportButton.SetEnabled(value: false);
		_privacyPolicyToggle.SetEnabled(value: false);
		_commentTextField.SetEnabled(value: false);
		_emailTextField.SetEnabled(value: false);
		_sendReportButton.text = _loc.T(SendingLocKey);
		StartCoroutine(SendReportCoroutine(_commentTextField.value, _emailTextField.value));
	}

	private IEnumerator SendReportCoroutine(string comment, string email)
	{
		yield return null;
		if (ErrorReportSender.SendErrorReport(comment, email))
		{
			_sendReportButton.text = _loc.T(SendSuccessLocKey);
			yield break;
		}
		_sendReportButton.text = _loc.T(SendFailLocKey);
		_privacyPolicyToggle.SetEnabled(value: true);
		_sendReportButton.SetEnabled(value: true);
	}

	private static string GetExceptionText()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (!string.IsNullOrWhiteSpace(ErrorReporter.LogString))
		{
			stringBuilder.AppendLine(ErrorReporter.LogString);
		}
		if (!string.IsNullOrWhiteSpace(ErrorReporter.StackTrace))
		{
			stringBuilder.AppendLine(ErrorReporter.StackTrace);
		}
		return stringBuilder.ToString();
	}
}
