using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

public interface IPanelController
{
	VisualElement GetPanel();

	bool OnUIConfirmed();

	void OnUICancelled();
}
