using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.WaterSourceSystem;

namespace Timberborn.MapEditorHazardousWeatherUI;

internal class MapEditorWaterStrengthModifier : BaseComponent, IInitializableEntity, IWaterStrengthModifier
{
	private readonly MapEditorHazardousWeatherSetter _mapEditorHazardousWeatherSetter;

	public MapEditorWaterStrengthModifier(MapEditorHazardousWeatherSetter mapEditorHazardousWeatherSetter)
	{
		_mapEditorHazardousWeatherSetter = mapEditorHazardousWeatherSetter;
	}

	public void InitializeEntity()
	{
		GetComponent<WaterSource>().AddWaterStrengthModifier(this);
	}

	public float GetStrengthModifier()
	{
		if (!_mapEditorHazardousWeatherSetter.IsDroughtWeather)
		{
			return 1f;
		}
		return 0f;
	}
}
