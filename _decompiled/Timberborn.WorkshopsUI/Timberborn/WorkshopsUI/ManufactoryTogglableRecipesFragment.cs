using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.SliderToggleSystem;
using Timberborn.Workshops;
using UnityEngine.UIElements;

namespace Timberborn.WorkshopsUI;

internal class ManufactoryTogglableRecipesFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ManufactoryRecipeSliderToggleFactory _manufactoryRecipeSliderToggleFactory;

	private readonly ILoc _loc;

	private VisualElement _root;

	private Label _toggleLabel;

	private VisualElement _toggleWrapper;

	private SliderToggle _sliderToggle;

	public ManufactoryTogglableRecipesFragment(VisualElementLoader visualElementLoader, ManufactoryRecipeSliderToggleFactory manufactoryRecipeSliderToggleFactory, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_manufactoryRecipeSliderToggleFactory = manufactoryRecipeSliderToggleFactory;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Game/EntityPanel/ManufactoryTogglableRecipesFragment";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_toggleLabel = _root.Q<Label>("ToggleLabel");
		_toggleWrapper = _root.Q<VisualElement>("ToggleWrapper");
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		ManufactoryTogglableRecipes component = entity.GetComponent<ManufactoryTogglableRecipes>();
		if ((bool)component)
		{
			Manufactory component2 = entity.GetComponent<Manufactory>();
			_sliderToggle = _manufactoryRecipeSliderToggleFactory.Create(_toggleWrapper, component2);
			_toggleLabel.text = _loc.T(component.LabelLocKey);
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	public void ClearFragment()
	{
		_sliderToggle = null;
		_toggleWrapper.Clear();
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		_sliderToggle?.Update();
	}
}
