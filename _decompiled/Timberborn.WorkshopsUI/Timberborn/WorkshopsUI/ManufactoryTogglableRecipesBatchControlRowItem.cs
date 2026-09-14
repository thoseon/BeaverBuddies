using Timberborn.BatchControl;
using Timberborn.SliderToggleSystem;
using UnityEngine.UIElements;

namespace Timberborn.WorkshopsUI;

internal class ManufactoryTogglableRecipesBatchControlRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem
{
	private readonly SliderToggle _sliderToggle;

	public VisualElement Root { get; }

	public ManufactoryTogglableRecipesBatchControlRowItem(VisualElement root, SliderToggle sliderToggle)
	{
		Root = root;
		_sliderToggle = sliderToggle;
	}

	public void UpdateRowItem()
	{
		_sliderToggle.Update();
	}
}
