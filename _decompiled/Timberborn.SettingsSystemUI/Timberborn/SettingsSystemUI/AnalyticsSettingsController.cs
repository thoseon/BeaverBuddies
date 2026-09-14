using Timberborn.Analytics;
using UnityEngine.UIElements;

namespace Timberborn.SettingsSystemUI;

internal class AnalyticsSettingsController
{
	private readonly AnalyticsConsent _analyticsConsent;

	private Toggle _analyticsEnabled;

	public AnalyticsSettingsController(AnalyticsConsent analyticsConsent)
	{
		_analyticsConsent = analyticsConsent;
	}

	public void Initialize(VisualElement root)
	{
		_analyticsEnabled = root.Q<Toggle>("AnalyticsEnabled");
		_analyticsEnabled.RegisterValueChangedCallback(delegate(ChangeEvent<bool> v)
		{
			ToggleConsent(v.newValue);
		});
	}

	public void Update()
	{
		_analyticsEnabled.SetValueWithoutNotify(_analyticsConsent.IsConsentGiven);
	}

	private void ToggleConsent(bool newValue)
	{
		if (newValue)
		{
			_analyticsConsent.GiveConsent();
		}
		else
		{
			_analyticsConsent.RemoveConsent();
		}
	}
}
