using UnityEngine.UIElements;

namespace Timberborn.AlertPanelSystem;

public interface IAlertFragment
{
	void InitializeAlertFragment(VisualElement root);

	void UpdateAlertFragment();
}
