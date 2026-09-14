using Timberborn.BaseComponentSystem;
using UnityEngine.UIElements;

namespace Timberborn.EntityPanelSystem;

public interface IEntityPanelFragment
{
	VisualElement InitializeFragment();

	void ShowFragment(BaseComponent entity);

	void ClearFragment();

	void UpdateFragment();
}
