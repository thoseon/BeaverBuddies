using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using Timberborn.SliderToggleSystem;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace Timberborn.MapEditorHazardousWeatherUI;

internal class MapEditorHazardousWeatherPanel : ILoadableSingleton, IUpdatableSingleton
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly UILayout _uiLayout;

	private readonly HazardousWeatherToggleFactory _hazardousWeatherToggleFactory;

	private SliderToggle _sliderToggle;

	public MapEditorHazardousWeatherPanel(VisualElementLoader visualElementLoader, UILayout uiLayout, HazardousWeatherToggleFactory hazardousWeatherToggleFactory)
	{
		_visualElementLoader = visualElementLoader;
		_uiLayout = uiLayout;
		_hazardousWeatherToggleFactory = hazardousWeatherToggleFactory;
	}

	public void Load()
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("MapEditor/MapEditorHazardousWeatherPanel");
		_sliderToggle = _hazardousWeatherToggleFactory.Create(visualElement);
		_uiLayout.AddTopRight(visualElement, 3);
	}

	public void UpdateSingleton()
	{
		_sliderToggle.Update();
	}
}
