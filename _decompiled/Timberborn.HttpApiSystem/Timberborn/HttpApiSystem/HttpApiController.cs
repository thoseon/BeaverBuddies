using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.StatusSystem;

namespace Timberborn.HttpApiSystem;

public class HttpApiController : BaseComponent, IAwakableComponent, IInitializableEntity, IFinishedStateListener
{
	private static readonly string ApiStoppedLocKey = "Status.Automation.ApiStopped";

	private static readonly string ApiStoppedShortLocKey = "Status.Automation.ApiStopped.Short";

	private readonly HttpApi _httpApi;

	private readonly ILoc _loc;

	private StatusToggle _statusToggle;

	public HttpApiController(HttpApi httpApi, ILoc loc)
	{
		_httpApi = httpApi;
		_loc = loc;
	}

	public void Awake()
	{
		_statusToggle = StatusToggle.CreatePriorityStatusWithAlertAndFloatingIcon("ApiStopped", _loc.T(ApiStoppedLocKey), _loc.T(ApiStoppedShortLocKey));
		DisableComponent();
	}

	public void InitializeEntity()
	{
		GetComponent<StatusSubject>().RegisterStatus(_statusToggle);
		UpdateStatus();
	}

	public void OnEnterFinishedState()
	{
		UpdateStatus();
		_httpApi.IsRunningChanged += OnIsRunningChanged;
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		_httpApi.IsRunningChanged -= OnIsRunningChanged;
		DisableComponent();
	}

	private void OnIsRunningChanged(object sender, EventArgs e)
	{
		UpdateStatus();
	}

	private void UpdateStatus()
	{
		_statusToggle.Toggle(!_httpApi.IsRunning);
	}
}
