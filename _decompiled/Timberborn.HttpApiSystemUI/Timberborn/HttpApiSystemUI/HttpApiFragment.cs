using System;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.HttpApiSystem;
using Timberborn.Localization;
using Timberborn.WebNavigation;
using UnityEngine.UIElements;

namespace Timberborn.HttpApiSystemUI;

internal class HttpApiFragment : IEntityPanelFragment
{
	private static readonly string StartApiLocKey = "Automation.Api.StartApi";

	private static readonly string StopApiLocKey = "Automation.Api.StopApi";

	private static readonly string UnsafeWebhooksLocKey = "Automation.Api.UnsafeWebhooksWarning";

	private static readonly string IUnderstandTheRiskLocKey = "Automation.Api.UnsafeWebhooksWarning.IUnderstandTheRisk";

	private static readonly string CancelLocKey = "Core.Cancel";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly HttpApi _httpApi;

	private readonly UrlOpener _urlOpener;

	private readonly HttpWebhookRegistry _httpWebhookRegistry;

	private readonly DialogBoxShower _dialogBoxShower;

	private VisualElement _root;

	private VisualElement _portWrapper;

	private TextField _portValue;

	private Button _startStopButton;

	private Button _openBrowserButton;

	private Label _urlLabel;

	private HttpApiController _httpApiController;

	public HttpApiFragment(VisualElementLoader visualElementLoader, ILoc loc, HttpApi httpApi, UrlOpener urlOpener, HttpWebhookRegistry httpWebhookRegistry, DialogBoxShower dialogBoxShower)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
		_httpApi = httpApi;
		_urlOpener = urlOpener;
		_httpWebhookRegistry = httpWebhookRegistry;
		_dialogBoxShower = dialogBoxShower;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/HttpApiFragment");
		_root.ToggleDisplayStyle(visible: false);
		_startStopButton = _root.Q<Button>("StartStop");
		_portWrapper = _root.Q<VisualElement>("PortWrapper");
		_portValue = _root.Q<TextField>("PortValue");
		_openBrowserButton = _root.Q<Button>("OpenBrowser");
		_urlLabel = _root.Q<Label>("Url");
		_startStopButton.RegisterCallback<ClickEvent>(ToggleServer);
		_openBrowserButton.RegisterCallback<ClickEvent>(OpenBrowser);
		_portValue.RegisterCallback<FocusOutEvent>(PortChanged);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_httpApiController = entity.GetComponent<HttpApiController>();
		UpdatePortTextField();
	}

	public void ClearFragment()
	{
		_httpApiController = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if ((bool)_httpApiController && _httpApiController.Enabled)
		{
			_root.ToggleDisplayStyle(visible: true);
			bool isRunning = _httpApi.IsRunning;
			_startStopButton.text = (isRunning ? _loc.T(StopApiLocKey) : _loc.T(StartApiLocKey));
			_portWrapper.ToggleDisplayStyle(!isRunning);
			_openBrowserButton.ToggleDisplayStyle(isRunning);
			_urlLabel.ToggleDisplayStyle(isRunning);
			if (isRunning)
			{
				_urlLabel.text = _httpApi.Url;
			}
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	private void ToggleServer(ClickEvent evt)
	{
		if (_httpApi.IsRunning)
		{
			_httpApi.Stop();
			return;
		}
		ImmutableArray<string> unsafeAddresses = _httpWebhookRegistry.FindUnsafeAddresses();
		if (unsafeAddresses.IsEmpty)
		{
			StartAndCheckError();
		}
		else
		{
			StartIfUserAcceptsUnsafeAddresses(unsafeAddresses);
		}
	}

	private void StartIfUserAcceptsUnsafeAddresses(ImmutableArray<string> unsafeAddresses)
	{
		string text = string.Join("\n", unsafeAddresses.Select((string url) => SpecialStrings.RowStarter + url));
		_dialogBoxShower.Create().SetMessage(_loc.T(UnsafeWebhooksLocKey) + "\n" + text).SetDefaultCancelButton(_loc.T(CancelLocKey))
			.SetConfirmButton(StartAndCheckError, _loc.T(IUnderstandTheRiskLocKey))
			.Show();
	}

	private void StartAndCheckError()
	{
		_httpApi.Start();
		if (!_httpApi.IsRunning)
		{
			ShowDialogWithError();
		}
	}

	private void ShowDialogWithError()
	{
		string errorMessage = _httpApi.ErrorMessage;
		if (errorMessage != null)
		{
			_dialogBoxShower.Create().SetMessage(errorMessage.Substring(0, Math.Min(errorMessage.Length, 1000))).Show();
		}
	}

	private void OpenBrowser(ClickEvent evt)
	{
		if (_httpApi.IsRunning)
		{
			_urlOpener.OpenUrl(_httpApi.Url);
		}
	}

	private void PortChanged(FocusOutEvent focusOutEvent)
	{
		if (ushort.TryParse(_portValue.value, out var result) && result > 0)
		{
			_httpApi.SetPort(result);
		}
		else
		{
			UpdatePortTextField();
		}
	}

	private void UpdatePortTextField()
	{
		_portValue.SetValueWithoutNotify(_httpApi.Port.ToString());
	}
}
