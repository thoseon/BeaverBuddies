using UnityEngine.UIElements;

namespace Timberborn.ModdingUI;

public interface IModManagerTooltipRegistrar
{
	void RegisterModWarning(VisualElement element, ModItem modItem);

	void RegisterModIcon(VisualElement element, ModItem modItem);

	void RegisterIncreaseButton(VisualElement element);

	void RegisterDecreaseButton(VisualElement element);
}
