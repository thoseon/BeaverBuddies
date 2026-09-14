using Timberborn.Localization;
using Timberborn.SliderToggleSystem;
using UnityEngine.UIElements;

namespace Timberborn.MapEditorHazardousWeatherUI;

internal class HazardousWeatherToggleFactory
{
	private static readonly string TemperateWeatherClass = "hazardous-weather-toggle__icon--temperate";

	private static readonly string DroughtWeatherClass = "hazardous-weather-toggle__icon--drought";

	private static readonly string BadtideWeatherClass = "hazardous-weather-toggle__icon--badtide";

	private static readonly string TemperateWeatherLocKey = "Weather.Temperate";

	private static readonly string DroughtWeatherLocKey = "Weather.Drought";

	private static readonly string BadtideWeatherLocKey = "Weather.Badtide";

	private readonly SliderToggleFactory _sliderToggleFactory;

	private readonly ILoc _loc;

	private readonly MapEditorHazardousWeatherSetter _mapEditorHazardousWeatherSetter;

	public HazardousWeatherToggleFactory(SliderToggleFactory sliderToggleFactory, ILoc loc, MapEditorHazardousWeatherSetter mapEditorHazardousWeatherSetter)
	{
		_sliderToggleFactory = sliderToggleFactory;
		_loc = loc;
		_mapEditorHazardousWeatherSetter = mapEditorHazardousWeatherSetter;
	}

	public SliderToggle Create(VisualElement parent)
	{
		SliderToggleItem sliderToggleItem = SliderToggleItem.Create(() => _loc.T(TemperateWeatherLocKey), TemperateWeatherClass, delegate
		{
			_mapEditorHazardousWeatherSetter.SetTemperateWeather();
		}, () => _mapEditorHazardousWeatherSetter.IsTemperateWeather);
		SliderToggleItem sliderToggleItem2 = SliderToggleItem.Create(() => _loc.T(DroughtWeatherLocKey), DroughtWeatherClass, delegate
		{
			_mapEditorHazardousWeatherSetter.SetDroughtWeather();
		}, () => _mapEditorHazardousWeatherSetter.IsDroughtWeather);
		SliderToggleItem sliderToggleItem3 = SliderToggleItem.Create(() => _loc.T(BadtideWeatherLocKey), BadtideWeatherClass, delegate
		{
			_mapEditorHazardousWeatherSetter.SetBadtideWeather();
		}, () => _mapEditorHazardousWeatherSetter.IsBadtideWeather);
		return _sliderToggleFactory.Create(parent, sliderToggleItem, sliderToggleItem2, sliderToggleItem3);
	}
}
