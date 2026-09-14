using Timberborn.BaseComponentSystem;
using Timberborn.MapEditorTickSystem;
using Timberborn.TickSystem;
using Timberborn.WaterSourceSystem;

namespace Timberborn.MapEditorHazardousWeatherUI;

[MapEditorTickable]
internal class MapEditorWaterContaminationController : TickableComponent, IAwakableComponent
{
	private readonly MapEditorHazardousWeatherSetter _mapEditorHazardousWeatherSetter;

	private WaterSourceContamination _waterSourceContamination;

	public MapEditorWaterContaminationController(MapEditorHazardousWeatherSetter mapEditorHazardousWeatherSetter)
	{
		_mapEditorHazardousWeatherSetter = mapEditorHazardousWeatherSetter;
	}

	public void Awake()
	{
		_waterSourceContamination = GetComponent<WaterSourceContamination>();
	}

	public override void Tick()
	{
		if (_mapEditorHazardousWeatherSetter.IsBadtideWeather)
		{
			_waterSourceContamination.SetContamination(1f);
		}
		else
		{
			_waterSourceContamination.ResetContamination();
		}
	}
}
