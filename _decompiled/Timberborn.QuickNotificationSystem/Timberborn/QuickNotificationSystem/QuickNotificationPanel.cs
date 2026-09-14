using Timberborn.BlueprintSystem;
using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.QuickNotificationSystem;

internal class QuickNotificationPanel : ILoadableSingleton, IUpdatableSingleton
{
	private static readonly string WarningClass = "square-large--red";

	private static readonly string NormalClass = "square-large--green";

	private readonly UILayout _uiLayout;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly QuickNotificationService _quickNotificationService;

	private readonly ISpecService _specService;

	private QuickNotificationSpec _quickNotificationSpec;

	private float _hideTime;

	private Label _alert;

	private float _duration;

	private float? _sendTime;

	public QuickNotificationPanel(UILayout uiLayout, VisualElementLoader visualElementLoader, QuickNotificationService quickNotificationService, ISpecService specService)
	{
		_uiLayout = uiLayout;
		_visualElementLoader = visualElementLoader;
		_quickNotificationService = quickNotificationService;
		_specService = specService;
	}

	public void Load()
	{
		_quickNotificationSpec = _specService.GetSingleSpec<QuickNotificationSpec>();
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Common/QuickNotificationPanel");
		_alert = visualElement.Q<Label>("Alert");
		_alert.ToggleDisplayStyle(visible: false);
		_uiLayout.AddAbsoluteItem(visualElement);
		_quickNotificationService.AlertSent += OnNotificationSent;
	}

	public void UpdateSingleton()
	{
		if (Time.unscaledTime > _sendTime)
		{
			_sendTime = null;
			_hideTime = Time.unscaledTime + _duration;
			_alert.ToggleDisplayStyle(visible: true);
		}
		if (Time.unscaledTime > _hideTime)
		{
			_alert.ToggleDisplayStyle(visible: false);
		}
	}

	private void OnNotificationSent(object sender, QuickNotificationEventArgs e)
	{
		_alert.text = e.Text;
		if (e.IsWarning)
		{
			_alert.AddToClassList(WarningClass);
			_alert.RemoveFromClassList(NormalClass);
		}
		else
		{
			_alert.AddToClassList(NormalClass);
			_alert.RemoveFromClassList(WarningClass);
		}
		_duration = (e.IsWarning ? _quickNotificationSpec.ExtendedDuration : _quickNotificationSpec.Duration);
		_sendTime = Time.unscaledTime;
	}
}
