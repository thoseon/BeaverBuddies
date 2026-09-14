using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.MapEditorHazardousWeatherUI;
using Timberborn.WaterSourceSystem;

namespace Timberborn.MapEditorWaterSourceSystemUI;

internal class BadwaterFlowStopper : BaseComponent, IAwakableComponent, IInitializableEntity, IWaterStrengthModifier
{
	private readonly MapEditorHazardousWeatherSetter _mapEditorHazardousWeatherSetter;

	private WaterSource _waterSource;

	public BadwaterFlowStopper(MapEditorHazardousWeatherSetter mapEditorHazardousWeatherSetter)
	{
		_mapEditorHazardousWeatherSetter = mapEditorHazardousWeatherSetter;
	}

	public void Awake()
	{
		_waterSource = GetComponent<WaterSource>();
	}

	public void InitializeEntity()
	{
		_waterSource.AddWaterStrengthModifier(this);
	}

	public float GetStrengthModifier()
	{
		return (!_mapEditorHazardousWeatherSetter.IsBadtideWeather) ? 1 : 0;
	}
}
