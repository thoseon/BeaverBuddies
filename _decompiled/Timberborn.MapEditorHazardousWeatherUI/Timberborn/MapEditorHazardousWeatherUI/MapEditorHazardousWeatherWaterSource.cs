using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.WaterSourceSystem;

namespace Timberborn.MapEditorHazardousWeatherUI;

internal class MapEditorHazardousWeatherWaterSource : BaseComponent, IAwakableComponent, IInitializableEntity, IWaterStrengthModifier
{
	private readonly MapEditorHazardousWeatherSetter _mapEditorHazardousWeatherSetter;

	private WaterSource _waterSource;

	private HazardousWeatherWaterSourceSpec _spec;

	public MapEditorHazardousWeatherWaterSource(MapEditorHazardousWeatherSetter mapEditorHazardousWeatherSetter)
	{
		_mapEditorHazardousWeatherSetter = mapEditorHazardousWeatherSetter;
	}

	public void Awake()
	{
		_waterSource = GetComponent<WaterSource>();
		_spec = GetComponent<HazardousWeatherWaterSourceSpec>();
	}

	public void InitializeEntity()
	{
		_waterSource.AddWaterStrengthModifier(this);
	}

	public float GetStrengthModifier()
	{
		string currentHazardousWeatherID = _mapEditorHazardousWeatherSetter.GetCurrentHazardousWeatherID();
		return _spec.ActiveInHazardousWeather.Contains(currentHazardousWeatherID) ? 1 : 0;
	}
}
