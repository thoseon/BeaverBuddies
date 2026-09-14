using Timberborn.BaseComponentSystem;

namespace Timberborn.PlantingUI;

public class PlantablePreview : BaseComponent
{
	public bool IsShown => base.GameObject.activeSelf;

	public void Show()
	{
		base.GameObject.SetActive(value: true);
	}

	public void Hide()
	{
		base.GameObject.SetActive(value: false);
	}
}
